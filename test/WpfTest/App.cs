using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.WPF.Services;
using System.Windows;

using WpfTest1.ViewModels;
using WpfTest1.Views;

namespace WpfTest1
{
    internal class App : CoreMvvmApp
    {
        
        protected override void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<TestDialogViewModel>();
        }
        /// <summary>
        /// Regin 등록 예정
        /// </summary>
        /// <param name="serviceProvider">ioc 서비스</param>
        protected override void ConfigureServiceProvider(IServiceContainer serviceProvider)
        {
            
        }
        /// <summary>
        /// Dialog View 등록
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.DialogService.RegisterDialog<TestDialogView>();
        }
        protected override Window CreateWindow(IServiceContainer serviceProvider)
        {            
            return new MainWindowView();
        }
    }
}
