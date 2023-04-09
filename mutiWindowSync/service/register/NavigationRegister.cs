using mutiWindowSync.Const;
using mutiWindowSync.ViewModels;
using mutiWindowSync.Views;
using Prism.Ioc;
using Prism.Regions;

namespace mutiWindowSync.service
{
    public static class NavigationRegister
    {
        public static void AddNavigationRegister(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HandleShow, HandleShowViewModel>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            
            
        }

        public static void AddNavigationProvider(this IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.HandleShowArea,typeof(HandleShow));
        }
    }
}