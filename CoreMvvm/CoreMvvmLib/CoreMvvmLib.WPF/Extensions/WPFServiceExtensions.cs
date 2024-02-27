using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.Core.Services.DialogService;
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
            ViewModelLocator.ViewModelLocator.SetServiceProvider(services);
            return services;
        }

    }
}
