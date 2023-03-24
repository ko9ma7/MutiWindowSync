using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using ImTools;
using mutiWindowSync.Entity;
using mutiWindowSync.Events;
using mutiWindowSync.service;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace mutiWindowSync.ViewModels
{
    public class HandleShowViewModel : BaseViewModel
    {
        
        private readonly IEventAggregator _eventAggregator;

        private SynHandleService synHandleService;

        private ObservableCollection<HandleInfo> _handleInfos = new ObservableCollection<HandleInfo>();
        public ObservableCollection<HandleInfo> ScheduleBaseDataGrids
        {
            get => _handleInfos;
            set => SetProperty(ref _handleInfos, value);
        }
        
        public HandleShowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AddHandleEvent>().Subscribe(AddHandleEvent);
            _eventAggregator.GetEvent<StarAllHandleEvent>().Subscribe(StarAllHandle);
        }

        private void StarAllHandle()
        {
            var mainHandle = ScheduleBaseDataGrids.First();
            if (mainHandle == null)
            {
                MessageBox.Show("未找到主控端！");
                return;
            }
            foreach (var handleInfo in ScheduleBaseDataGrids)
            {
                handleInfo.State = "运行中！！！";
            }
            // 找出除主控端外的所有句柄id
            var handles = ScheduleBaseDataGrids.Where(x => x.HandleId != mainHandle.HandleId).Select(x => x.Handle).ToList();
            // 使用lambda找出除主控端外的所有句柄，将其状态设置为“运行中”
            
            // 创建异步线程，将耗时任务丢入线程处理
            SyncHandle(mainHandle.Handle, handles);




        }
        
        private void SyncHandle(IntPtr mainHandle, List<IntPtr> handles)
        {
            // 监听主控端句柄鼠标按键操作，将操作同步到其他句柄
            synHandleService = new SynHandleService(mainHandle, handles);
            synHandleService.SyncHandleAsync();
            GC.KeepAlive(synHandleService);
            
        }

        private void AddHandleEvent(HandleInfo obj)
        {
            if (obj!=null)
            {
                // 是否存在ScheduleBaseDataGrids.id==obj.id的对象
                bool ifExist = false;
                foreach (var item in ScheduleBaseDataGrids)
                {
                    if (item.HandleId == obj.HandleId)
                    {
                        ifExist = true;
                        break;
                    }
                }
                if (ifExist)
                {
                    MessageBox.Show($"已经存在该句柄！Title:{obj.Title}");
                    return;
                }
                ScheduleBaseDataGrids.Add(obj);
            }
        }


        protected override  void ExecuteLoadingCommandAsync()
        {
            Debug.WriteLine("载入页面！");
        }
    }
}