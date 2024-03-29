using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.WPF.Services;
using RegionTest.View;
using RegionTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RegionTest
{
    internal class App : CoreMvvmApp
    {
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.RegionManager.RegisterAddView<AView>();
        }
        protected override void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<AViewModel>();
        }
        protected override void ConfigureServiceProvider(IServiceContainer serviceProvider)
        {
            
        }
        protected override Window CreateWindow(IServiceContainer serviceProvider)
        {
            return new MainWindowView();
        }
    }
}
