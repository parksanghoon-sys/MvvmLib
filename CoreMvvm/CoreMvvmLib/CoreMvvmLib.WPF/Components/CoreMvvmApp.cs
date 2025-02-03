using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Extensions;
using System.Windows;

namespace CoreMvvmLib.WPF.Components
{
    public abstract class CoreMvvmApp : Application
    {
        public IServiceContainer Services { get; }
        public CoreMvvmApp()
        {
            Services = ConfigureService();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = CreateWindow(Services);
            window.Show();
        }
        protected virtual Window CreateWindow(IServiceContainer serviceProvider)
        {
            var window = new Window();
            return window;
        }
        private IServiceContainer? ConfigureService()
        {
            var services = ServiceCollection.Create();
            ContainerProvider.Initialize(services);
            // TODO : 추후 필요 서비스 추가
            services.AddDialogService();
            services.AddRegionManager();

            ConfigureServiceCollection(services);
            var serviceProvider = services.CreateContainer();

            // ServiceLocator 준비
            ConfigureServiceLocator();

            serviceProvider.AddDialogServiceLocator();
            serviceProvider.AddViewModelLocator();
            
            //Service 초기화 
            ConfigureServiceProvider(serviceProvider);

            return serviceProvider;
        }

        protected virtual void ConfigureServiceProvider(IServiceContainer serviceProvider)
        {            
        }

        protected virtual void ConfigureServiceLocator()
        {            
        }
        /// <summary>
        /// Service 등록 ex) viewmodel
        /// </summary>
        /// <param name="services"></param>
        protected virtual void ConfigureServiceCollection(IServiceCollection services)
        {            
        }
    }
}
