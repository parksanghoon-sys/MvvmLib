using CoreMvvmLib.Core.IOC;
using System.Windows;

namespace CoreMvvmLib.WPF.Components
{
    public class CoreMvvmApp : Application
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
            // TODO : 추후 필요 서비스 추가
            ConfigureServiceCollection(services);
            var serviceProvider = services.CreateContainer();

            // ServiceLocator 준비
            ConfigureServiceLocator();

            //Service 초기화 
            ConfigureServiceProvider(serviceProvider);

            return serviceProvider;
        }

        private void ConfigureServiceProvider(Core.IOC.IServiceContainer serviceProvider)
        {            
        }

        protected virtual void ConfigureServiceLocator()
        {            
        }

        protected virtual void ConfigureServiceCollection(IServiceCollection services)
        {            
        }
    }
}
