using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Domain.Services.DirectoryServices;

namespace wpfCodeCheck.Main.Services
{
    public class CompareFactoryService
    {
        private readonly ISettingService _settingService;
        private readonly ICsvHelper _csvHelper;
        private readonly IFileCheckSum _fileCheckSum;
        private readonly ICompare _compare;

        public CompareFactoryService(ISettingService settingService, ICsvHelper csvHelper, IFileCheckSum fileCheckSum)
        {
            _settingService = settingService;
            _csvHelper = csvHelper;
            _fileCheckSum = fileCheckSum;
            _compare = CreateCompareService();
        }
        public ICompare CreateCompareService()
        {
            var inputType = _settingService.GeneralSetting!.CompareType;

            switch (inputType)
            {
                case EType.NONE:
                    return null;
                case EType.SW_CODE:
                    return new CodeCompareService(_csvHelper);
                case EType.FILE:
                    return new FileCompareService(_csvHelper, _settingService);
                default:
                    break;
            }
            return null;
        }
        public IDirectoryCompare CreateIDirectoryCompareService()
        {
            var inputType = _settingService.GeneralSetting!.CompareType;

            switch (inputType)
            {
                case EType.NONE:
                    return null;
                case EType.SW_CODE:
                    return new SourceDirectoryService(_fileCheckSum);
                case EType.FILE:
                    return new FileDirectoryService(_fileCheckSum);
                default:
                    break;
            }
            return null;
        }
    }
}