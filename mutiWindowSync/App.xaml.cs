using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using mutiWindowSync.service;
using Prism.DryIoc;
using Prism.Ioc;

namespace mutiWindowSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.AddServicesRegister();
            containerRegistry.AddNavigationRegister();
        }

        protected override Window CreateShell()
        {
            // throw new NotImplementedException();
            return Container.Resolve<MainWindow>();
        }
    }
}