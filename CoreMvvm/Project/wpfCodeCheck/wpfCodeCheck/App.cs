using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Components;
using System.Windows;
using wpfCodeCheck.Forms.UI.Views;

namespace wpfCodeCheck
{
    internal class App : CoreMvvmApp
    {
        /// <summary>
        /// Service 등록 ex) viewmodel
        /// </summary>
        /// <param name="services"></param>
        protected override void ConfigureServiceCollection(IServiceCollection services)
        {
            base.ConfigureServiceCollection(services);
        }
        /// <summary>
        /// Regin등록
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected override void ConfigureServiceProvider(IServiceContainer serviceProvider)
        {
            base.ConfigureServiceProvider(serviceProvider);
        }
        /// <summary>
        /// Dialog View 등록
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
        }
        protected override Window CreateWindow(IServiceContainer serviceProvider)
        {
            return new MainWindowView();            
        }
    }
}
