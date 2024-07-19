using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.Core.Services.RegionManager;
using CoreMvvmLib.WPF.Services;

namespace CoreMvvmLib.WPF.Extensions
{
    internal static class WPFServiceExtensions
    {
        internal static IServiceCollection AddDialogService(this IServiceCollection services)
        {
            services.AddSingleton<IDialogService>(ServiceLocator.DialogService);
            return services;
        }
        internal static IServiceContainer AddDialogServiceLocator(this IServiceContainer services)
        {
            ServiceLocator.DialogService.SetServiceProvider(services);
            return services;
        }
        internal static IServiceContainer AddViewModelLocator(this IServiceContainer services)
        {
            ViewModelLocator.SetServiceProvider(services);
            return services;
        }
        internal static IServiceCollection AddRegionManager(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRegionManager>(ServiceLocator.RegionManager);
            return serviceCollection;
        }

    }
}
