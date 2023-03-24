using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mutiWindowSync.Const;
using mutiWindowSync.Entity;
using mutiWindowSync.Events;
using mutiWindowSync.service;
using mutiWindowSync.ViewModels;
using mutiWindowSync.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

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

        public MainWindow(IContainerProvider containerProvider, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            
            
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.HandleShowArea,typeof(HandleShow));
        }
        
        private  HandleShowViewModel _dataContext;
        private HandleShowViewModel MDataContext
        {
            get { return _dataContext ??= (HandleShowViewModel) DataContext; }
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
            var handleInfo = new HandleInfo();
            if (handleInfo.Id ==1)
            {
                handleInfo.Type = "主控端";
            }
            handleInfo.HandleId = e.Handle.ToString();
            handleInfo.Handle = e.Handle;
            handleInfo.Title = e.Caption;
            handleInfo.Size = e.Area.Width.ToString() + "*" + e.Area.Height.ToString();
            // Application.Exit();
            _eventAggregator.GetEvent<AddHandleEvent>().Publish(handleInfo);
        }

        private void Button_starSync(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<StarAllHandleEvent>().Publish();
        }
    }
}