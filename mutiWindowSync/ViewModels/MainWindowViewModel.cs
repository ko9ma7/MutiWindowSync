using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using mutiWindowSync.Entity;
using mutiWindowSync.service;

namespace mutiWindowSync.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        public MainWindowViewModel()
        {
        }

        ObservableCollection<Handle> _handles = new ObservableCollection<Handle>();
        public ObservableCollection<Handle> Handles
        {
            get => _handles;
            set => SetProperty(ref _handles, value);
        }
        
    }
}