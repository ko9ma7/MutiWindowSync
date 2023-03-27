using mutiWindowSync.service.IService;
using Prism.Ioc;

namespace mutiWindowSync.service
{
    public static class ServicesRegister
    {
        public static void AddServicesRegister(this IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDmService, DmService>();
        }
    }
}