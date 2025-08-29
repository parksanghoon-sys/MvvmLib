using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.WPF.Services;
using System.Windows;
using wpfCodeCheck.ProjectChangeTracker.Local.ViewModels;
using wpfCodeCheck.ProjectChangeTracker.UI.Views;
using wpfCodeCheck.Forms.Local.ViewModels;
using wpfCodeCheck.Forms.UI.Views;
using wpfCodeCheck.Main.Local.ViewModels;
using wpfCodeCheck.Main.UI.Views;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.ProjectChangeTracker.Local.Services;
using wpfCodeCheck.Main.Local.Servies.CheckSumService;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Services;
using wpfCodeCheck.Domain.Services.DirectoryServices;
using wpfCodeCheck.SDDExport.UI.Views;
using wpfCodeCheck.SDDExport.Local.ViewModels;
using wpfCodeCheck.Domain.Services.Interfaces;

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
            services.AddSingleton<ComparisonResultsViewModel>();
            services.AddSingleton<ComparisonResultsViewModel>();
            services.AddSingleton<DirectoryCompareViewModel>();

            services.AddTransient<SddExportViewModel>();
            services.AddTransient<FolderListViewModel>();            

            services.AddTransient<IFileCheckSum, FileCheckSumCRC32>();

            //services.AddTransient<CodeCompareService>();
            services.AddTransient<CompareFactoryService>();
            services.AddTransient<IDirectoryCompare, SourceDirectoryService>();

            services.AddTransient<ICsvHelper, CsvHelper>();
            services.AddTransient<IExcelPaser, InteropExcelParsser>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<ISettingService, SettingService>();

            base.ConfigureServiceCollection(services);
        }
        /// <summary>
        /// Regin등록
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected override void ConfigureServiceProvider(IServiceContainer serviceProvider)
        {
            //ServiceLocator.RegionManager.RegisterAddView<TestView>();            
            //ServiceLocator.RegionManager.RegisterAddView<Test2View>();            
            //ServiceLocator.RegionManager.RegisterAddView<Test3View>();            
            ServiceLocator.RegionManager.RegisterAddView<FolderCompareView>();            
            ServiceLocator.RegionManager.RegisterAddView<ComparisonResultsView>();   
            ServiceLocator.RegionManager.RegisterAddView<DirectoryCompareView>();   
            ServiceLocator.RegionManager.RegisterAddView<SddExportView>();   
            
        }
        /// <summary>
        /// Dialog View 등록
        /// </summary>       
        protected override void ConfigureServiceLocator()
        {            
            ServiceLocator.DialogService.RegisterSingleDialog<LoadingDialogView>(true);            
        }      
        protected override Window CreateWindow(IServiceContainer serviceProvider)
        {          
            return new MainWindowView();            
        }
        //public T GetService<T>() where T : class
        //{
        //    return Services.GetService<T>();
        //}
        protected override void OnExit(ExitEventArgs e)
        {            
            base.OnExit(e);
        }
    }
}
