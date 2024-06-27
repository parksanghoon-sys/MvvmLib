using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.WPF.Services;
using System.Windows;
using wpfCodeCheck.Forms.Local.ViewModels;
using wpfCodeCheck.Forms.Themes.Views;
using wpfCodeCheck.Forms.UI.Views;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;
using wpfCodeCheck.Main.Local.ViewModels;
using wpfCodeCheck.Main.UI.Views;
using wpfCodeCheck.Sub.UI.Views;

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
            
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<TestViewModel>();
            services.AddSingleton<FolderCompareViewModel>();

            services.AddTransient<FolderListViewModel>();

            services.AddTransient<IFileCheckSum, Crc32FileChecSum>();
            services.AddTransient<IDierctoryFileInfoService, DirectoryManager>();

            base.ConfigureServiceCollection(services);
        }
        /// <summary>
        /// Regin등록
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected override void ConfigureServiceProvider(IServiceContainer serviceProvider)
        {
            ServiceLocator.RegionManager.RegisterAddView<TestView>();            
            ServiceLocator.RegionManager.RegisterAddView<Test2View>();            
            ServiceLocator.RegionManager.RegisterAddView<Test3View>();            
            ServiceLocator.RegionManager.RegisterAddView<FolderCompareView>();            
            
        }
        /// <summary>
        /// Dialog View 등록
        /// </summary>
        protected override void ConfigureServiceLocator()
        {            
            ServiceLocator.DialogService.RegisterDialog<LoadingDialogView>();
        }
        protected override Window CreateWindow(IServiceContainer serviceProvider)
        {
            return new MainWindowView();            
        }
    }
}
