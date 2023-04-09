using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gma.System.MouseKeyHook;
using mutiWindowSync.Const;
using mutiWindowSync.Entity;
using mutiWindowSync.Events;
using mutiWindowSync.service.hook;
using mutiWindowSync.service;
using mutiWindowSync.service.IService;
using mutiWindowSync.ViewModels;
using mutiWindowSync.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using KeyboardHook = mutiWindowSync.service.hook.KeyboardHook;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;
using MessageBox = System.Windows.MessageBox;

namespace mutiWindowSync
{
    
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private readonly IRegionManager _regionManager;
        private IRegion _manageContent;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDmService _dmService;

        public MainWindow(IContainerProvider containerProvider, IEventAggregator eventAggregator, IDmService dmService)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _dmService = dmService;
            
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.HandleShowArea,typeof(HandleShow));
        }
        
        private  MainWindowViewModel _dataContext;
        private MainWindowViewModel MDataContext
        {
            get { return _dataContext ??= (MainWindowViewModel) DataContext; }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var agent = new SpyAgent();
            agent.SpiedWindowSelected += agent_SpiedWindowSelected;
            MessageBox.Show("Press Shift + S to select highlighted window.");
            agent.BeginSpying();
            // Application.Run();
        }

        private void agent_SpiedWindowSelected(object sender, SpiedWindow e)
        {
            var agent = (sender as SpyAgent);
            agent.EndSpying();
            // MessageBox.Show(string.Format("Caption:'{0}', Handle:'{1}', area:'{2}' selected.", e.Caption,e.Handle, e.Area));
            var spiedWindows = e.GetChildren();
            // 定义一个字典，用于存储句柄和父句柄的对应关系 <句柄，父句柄>
            var hwnds = new Dictionary<SpiedWindow,SpiedWindow>();
            foreach (var window in spiedWindows)
            {
                var win = e.GetParentWindow(window.Handle);
                hwnds.Add(window,win);
            }
            var parent = new Handle(e);
            translator(parent, hwnds);
            MDataContext.Handles.Clear();
            MDataContext.Handles.Add(parent);
        }

        void translator(Handle pa, Dictionary<SpiedWindow,SpiedWindow> ch)
        {
            foreach (var ptr in ch)
            {
                if (ptr.Value.Handle == pa.ParentHwnd.Handle) // 如果父句柄等于当前句柄
                {
                    var handle = new Handle(ptr.Key);
                    pa.ChiHwnds ??= new ObservableCollection<Handle>();
                    pa.ChiHwnds.Add(handle);
                    translator(handle, ch);
                }
            }
        }

                    
        private KeyEventHandler myKeyEventHandeler = null;//按键钩子
        private KeyboardHook k_hook = new KeyboardHook();
        private IKeyboardMouseEvents m_GlobalHook;
        private void Button_starSync(object sender, RoutedEventArgs e)
        {
            // _dmService.capture();
            // _eventAggregator.GetEvent<StarAllHandleEvent>().Publish();

            // myKeyEventHandeler= new KeyEventHandler(hook_KeyDown);
            // k_hook.KeyDownEvent += myKeyEventHandeler;//钩住键按下
            // k_hook.Start();//安装键盘钩子

            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }
        
        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            var events = sender as  Gma.System.MouseKeyHook.Implementation.GlobalKeyListener;
            Debug.WriteLine("按下按键" + e.KeyChar);
        }
        
        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            Debug.WriteLine("按下按键" + e.Button);
        }
    
    private void hook_KeyDown(object sender, KeyEventArgs e)
    {
        //  获取句柄键盘事件的句柄

        Debug.WriteLine("按下按键" + e.KeyValue);
        // writeLog("按下按键" + e.KeyValue);
    }

        private void MenuItem_OnClick(object sender, RoutedEventArgs t)
        {
            var h = treeview.SelectedItem as Handle;
            var e = h?.ParentHwnd;

            var handleInfo = new HandleInfo();
            handleInfo.HandleId = e.Handle.ToString();
            handleInfo.Handle = e.Handle;
            handleInfo.Title = e.Caption;
            handleInfo.Size = e.Area.Width.ToString() + "*" + e.Area.Height.ToString();
            // Application.Exit();
            _eventAggregator.GetEvent<AddHandleEvent>().Publish(handleInfo);
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
        
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);
 
            return source;
        }

        private void Button_ClearSync(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ClearHandleEvent>().Publish();
            
        }
    }
}