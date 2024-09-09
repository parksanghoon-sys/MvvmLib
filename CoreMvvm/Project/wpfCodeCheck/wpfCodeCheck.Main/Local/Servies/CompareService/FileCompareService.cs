using System.Diagnostics;
using System.IO;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.CompareServices;


namespace wpfCodeCheck.Main.Local.Servies
{
    public class FileCompareService : CompareBaseService
    {
        private readonly ISettingService _settingService;

        public FileCompareService(ICsvHelper csvHelper, ISettingService settingService)
            : base(csvHelper)
        {
            _settingService = settingService;
        }

        private string GetRelativePath(string fullPath, string basePath)
        {
            // basePath 이후의 경로 추출
            if (fullPath.StartsWith(basePath))
            {
                return fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
            }
            return fullPath;
        }

        public override async Task<List<CompareEntity>> CompareModelCollections(IList<FileCompareModel> inputItems, IList<FileCompareModel> outputItems)
        {
            List<CompareEntity> diffReulstModel = new();
            List<string> allPaths;

            var inputBasePath = _settingService.GeneralSetting!.InputPath;
            var outputBasePath = _settingService.GeneralSetting!.OutputPath;

            var dictInput = inputItems?.ToDictionary(item => GetRelativePath(item.FilePath, inputBasePath)) ?? new Dictionary<string, FileCompareModel>();
            var dictOutput = outputItems?.ToDictionary(item => GetRelativePath(item.FilePath, outputBasePath)) ?? new Dictionary<string, FileCompareModel>();
            allPaths = dictInput?.Keys.Union(dictOutput?.Keys).ToList();

            //if (dictOutput is not null && dictInput is not null )            

            //else            
            //    allPaths = dictInput.Keys.ToList() ?? dictOutput.Keys.ToList();            



            foreach (var path in allPaths)
            {
                if (dictInput.TryGetValue(path, out var item1) && dictOutput.TryGetValue(path, out var item2))
                {
                    if (item1 is not null && item2 is not null)
                    {
                        SWDetailedItem detailedItem = new SWDetailedItem();
                        // 두 항목이 같은지 비교
                        if (item1.Equals(item2))
                        {
                            item1.IsComparison = item2.IsComparison = true;
                        }
                        else
                        {
                            if (item1.Checksum != "" && item2.Checksum != "")
                            {
                                item1.IsComparison = item2.IsComparison = false;
                                item1.Description = SetDescription(item1.FilePath, item1.FileName);
                                item2.Description = SetDescription(item2.FilePath, item2.FileName);
                                 
                                _code1.Add(item1);
                                _code2.Add(item2);
                                AddSwDetailItem(item1, item2);
                                AddCompareEntityList(item1, item2, diffReulstModel);

                            }
                        }

                        if (item1.Children != null && item2.Children != null)
                        {
                            // 하위 항목 비교
                            diffReulstModel.AddRange(await CompareModelCollections(item1.Children, item2.Children));
                        }
                    }
                }
                else
                {
                    // 한쪽에만 파일이나 폴더가 있는 경우 false
                    if (dictInput.TryGetValue(path, out item1))
                    {
                        if (item1.Checksum != "")
                        {
                            item1.IsComparison = false;
                            item1.Description = SetDescription(item1.FilePath, item1.FileName);

                            _code1.Add(item1);
                            AddSwDetailItem(item1, null);
                            AddCompareEntityList(item1, null, diffReulstModel);
                        }
                        else
                        {
                            if (item1.Children is not null)
                                // 하위 항목 비교
                                diffReulstModel.AddRange(await CompareModelCollections(item1.Children!, null));
                        }
                    }
                    if (dictOutput.TryGetValue(path, out item2))
                    {
                        if (item2.Checksum != "")
                        {
                            item2.IsComparison = false;
                            item2.Description = SetDescription(item2.FilePath, item2.FileName);
                            _code2.Add(item2);
                            AddSwDetailItem(null, item2);
                            AddCompareEntityList(null, item2, diffReulstModel);
                        }
                        else
                        {
                            // 하위 항목 비교
                            if (item2 is not null)
                                diffReulstModel.AddRange(await CompareModelCollections(null, item2.Children!));
                        }

                    }
                }
            }

            return diffReulstModel;
        }
        private void AddCompareEntityList(FileCompareModel item1, FileCompareModel item2, List<CompareEntity> diffReulstModel)
        {
            if (item1 is not null && item2 is not null)
            {
                diffReulstModel.Add(new CompareEntity
                {
                    InputFileName = item1.FileName,
                    InputFilePath = item1.FilePath,
                    OutoutFilePath = item2.FilePath,
                    OutoutFileName = item2.FileName,
                });
            }
            else if (item1 is null && item2 is not null)
            {
                diffReulstModel.Add(new CompareEntity
                {
                    InputFileName = string.Empty,
                    InputFilePath = string.Empty,
                    OutoutFilePath = item2.FilePath,
                    OutoutFileName = item2.FileName,
                });

            }
            else
            {
                diffReulstModel.Add(new CompareEntity
                {
                    InputFileName = item1.FileName,
                    InputFilePath = item1.FilePath,
                    OutoutFilePath = string.Empty,
                    OutoutFileName = string.Empty,
                });
            }
        }
        private string SetDescription(string filePath, string fileName)
        {
            string descriptionExe = "";
            if (filePath.Contains("B.Settings\\Config"))
            {
                descriptionExe = "실행 파일 관련 설정 파일";
            }
            if (filePath.Contains("B.Settings\\Csv"))
            {
                if (filePath.Contains("AutoSatProfile"))
                {
                    descriptionExe = "위성 프로파일";
                }
                if (!filePath.Contains("CheckCsv"))
                {
                    descriptionExe = !filePath.Contains("StatusCsv") ? (!filePath.Contains("UvhfTadCsv") ? "비행통제 설정 정보 파일" : "비행통제 통신 관련 설정 파일") : "비행통제 정보 상세보기 파일";
                }

            }
            if (!filePath.Contains("B.Settings\\Data") && !filePath.Contains("B.Settings\\Init"))
            {
                if (filePath.Contains("B.Settings\\Mission"))
                {
                    descriptionExe = !filePath.Contains("WarSymbol") ? "지도 정보 파일" : "지도 표적 이미지 파일";
                }
                if (filePath.Contains("B.Settings\\MouseCursor"))
                {
                    descriptionExe = "TM 마우스 커서 파일";
                }
                if (filePath.Contains("B.Settings\\Wav"))
                {
                    descriptionExe = "미디어 파일";
                }
                if (filePath.Contains("B.Settings\\Xlsx") || filePath.Contains("B.Settings\\Xml"))
                {
                    descriptionExe = "ICD 정보 관련 파일";
                }

            }

            if (fileName.Contains("DevExpress"))
            {
                descriptionExe = "해외구매/UI구성라이브러리";

            }
            if (fileName.Contains("ndds"))
            {
                descriptionExe = "해외구매/RTIDDS통신관련라이브러리";

            }
            if (fileName.Contains("NX"))
            {
                descriptionExe = "국내구매 / 지도관련 엔진  라이브러리";

            }
            if (fileName.Contains("Module.dll") || fileName.Contains("Model_type.dll") || fileName.Contains("stdatamodel.dll"))
            {
                descriptionExe = "연구개발/ICD관련공통모듈";
            }
            if (fileName.Contains("Soletop."))
            {
                descriptionExe = "연구개발 / 공통  라이브러리";
            }
            if (((IEnumerable<string>)fileName.Split('.')).Last<string>() == "exe")
            {
                descriptionExe = "연구개발/실행 Exe 파일";
            }
            descriptionExe = !(((IEnumerable<string>)fileName.Split('.')).Last<string>() == "dll") ? "연구개발/ 환경파일" : "연구개발/실행및라이브러리 DLL파일";

            return descriptionExe;

        }
    }
}
