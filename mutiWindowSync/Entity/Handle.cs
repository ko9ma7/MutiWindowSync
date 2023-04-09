using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using mutiWindowSync.service;

namespace mutiWindowSync.Entity
{
    public class Handle : BaseDto
    {
        public Handle()
        {
        }
        public Handle(SpiedWindow parentHwnd)
        {
            ParentHwnd = parentHwnd;
            handleId = ParentHwnd.Handle.ToString();
            title = ParentHwnd.Caption;
        }

        private SpiedWindow _parentHwnd;
        
        public SpiedWindow ParentHwnd
        {
            get => _parentHwnd;
            set
            {
                SetProperty(ref _parentHwnd, value);
                RaisePropertyChanged();
            }
        }
        
        private ObservableCollection<Handle> _chiHwnds = new ObservableCollection<Handle>();
        
        public ObservableCollection<Handle> ChiHwnds
        {
            get => _chiHwnds;
            set
            {
                SetProperty(ref _chiHwnds, value);
                RaisePropertyChanged();
            }
        }

        
        /**
         * 句柄ID
         */
        private string handleId;

        public string HandleId
        {
            get => handleId;
            set
            {
                value ??= String.Empty;
                SetProperty(ref handleId, value);
                RaisePropertyChanged();
            }
        }
        
        /**
         * 句柄标题
         */
        private string title;
        
        public string Title
        {
            get => title;
            set
            {
                value ??= String.Empty;
                SetProperty(ref title, value);
                RaisePropertyChanged();
            }
        }
    }
    
}