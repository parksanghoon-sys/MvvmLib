//using wpfCodeCheck.Domain.Local.Helpers;
//using wpfCodeCheck.Domain.Enums;
//using wpfCodeCheck.Main.Local.Servies;
//using wpfCodeCheck.Main.Local.Services.CompareService;
//using wpfCodeCheck.Domain.Services.Interfaces;
//using wwpfCodeCheck.Domain.Services.Interfaces;

//namespace wpfCodeCheck.Main.Services
//{
//    public interface ICompareFactoryService
//    {
//        ICompareService CreateCompareService();
//        ICompareService CreteDirectoryService();
//    }
//    public class CompareFactoryService : ICompareFactoryService
//    {
//        private readonly ISettingService _settingService;
//        private readonly ICsvHelper _csvHelper;
//        private readonly IFileCheckSum _fileCheckSum;
//        private readonly ICompareService _compareService;

//        public CompareFactoryService(ISettingService settingService, ICsvHelper csvHelper, IFileCheckSum fileCheckSum)
//        {
//            _settingService = settingService;
//            _csvHelper = csvHelper;
//            _fileCheckSum = fileCheckSum;
//            _compareService = CreateCompareService();
//        }
//        public ICompareService CreateCompareService()
//        {
//            var inputType = _settingService.GeneralSetting!.CompareType;

//            switch (inputType)
//            {
//                case ECompareType.NONE:
//                    return null;
//                case ECompareType.SOURCECODE:
//                    return new CodeCompareService(_csvHelper);
//                case ECompareType.FILE:
//                    return new FileCompareService(_csvHelper, _settingService);
//                default:
//                    break;
//            }
//            return null;
//        }
//        public ICompareService CreteDirectoryService()
//        {
//            var inputType = _settingService.GeneralSetting!.CompareType;

//            switch (inputType)
//            {
//                case ECompareType.NONE:
//                    return null;
//                case ECompareType.SOURCECODE:
//                    return new SourceDirectoryService(_fileCheckSum);
//                case ECompareType.FILE:
//                    return new FileDirectoryService(_fileCheckSum);
//                default:
//                    break;
//            }
//            return null;
//        }
//    }
//}