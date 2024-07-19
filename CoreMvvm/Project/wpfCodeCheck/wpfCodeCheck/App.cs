using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.WPF.Services;
using System.Windows;
using wpfCodeCheck.ConfigurationChange.Local.ViewModels;
using wpfCodeCheck.ConfigurationChange.UI.Views;
using wpfCodeCheck.Forms.Local.ViewModels;
using wpfCodeCheck.Forms.UI.Views;
using wpfCodeCheck.Main.Local.Helpers.CsvHelper;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;
using wpfCodeCheck.Main.Local.ViewModels;
using wpfCodeCheck.Main.UI.Views;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.Local.Models;

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

            services.AddTransient<FolderListViewModel>();            

            services.AddTransient<IFileCheckSum, Crc32FileChecSum>();
            
            services.AddTransient<IDierctoryFileInfoService<CodeInfoModel>, DirectoryManager>();
            services.AddTransient<ICsvHelper, CsvHelper>();

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
            //// 실행 파일의 디렉토리 경로를 설정
            //_settingService.UserAppDataPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //// 어셈블리 이름을 가져와 프로그램 이름으로 사용
            //string programName = Assembly.GetExecutingAssembly().GetName().Name;


            //// UserAppDataPath를 설정하고, 마지막 8자를 제거
            //string userAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //var settingPath = Path.Combine(userAppDataPath.Remove(userAppDataPath.Length - 8), programName);

            //Settings.UserAppDataPath = settingPath;

            //if (Directory.Exists(Settings.UserAppDataPath) == false)
            //{
            //    Settings.UserAppDataPath = settingPath;
            //}

            //Settings.LoadSettings();
            return new MainWindowView();            
        }
        public T GetService<T>() where T : class
        {
            return Services.GetService<T>();
        }
        protected override void OnExit(ExitEventArgs e)
        {            
            base.OnExit(e);
        }
    }
}
