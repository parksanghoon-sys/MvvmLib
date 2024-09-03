using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Services.CompareServices;

namespace wpfCodeCheck.Domain.Services
{
    public class CompareFactoryService
    {
        private readonly ISettingService _settingService;
        private readonly ICsvHelper _csvHelper;
        private readonly ICompare _compare;

        public CompareFactoryService(ISettingService settingService, ICsvHelper csvHelper)
        {
            _settingService = settingService;
            _csvHelper = csvHelper;
            _compare = CreateCompareService();
        }
        public ICompare CreateCompareService()
        {
            var inputType = _settingService.GeneralSetting!.CompareType;

            switch (inputType)
            {
                case Enums.EType.NONE:
                    return null;
                case Enums.EType.SW_CODE:
                    return new FileCompareService(_csvHelper, _settingService);
                case Enums.EType.FILE:
                    return new FileCompareService(_csvHelper, _settingService);
            }
            return null;
        }        
    }
}