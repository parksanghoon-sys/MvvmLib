using System.Diagnostics;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Local.Helpers;


namespace wpfCodeCheck.Domain.Services.CompareServices
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
                            _code2.Add(item2);
                            AddSwDetailItem(null, item2);
                            AddCompareEntityList(null, item2, diffReulstModel);
                        }
                        else
                        {
                            // 하위 항목 비교
                            if(item2 is not null)
                                diffReulstModel.AddRange(await CompareModelCollections(null, item2.Children!));
                        }

                    }
                }
            }

            return diffReulstModel;
        }
        private void AddCompareEntityList(FileCompareModel item1, FileCompareModel item2, List<CompareEntity> diffReulstModel)
        {
            if(item1 is not null && item2 is not null)
            {
                diffReulstModel.Add(new CompareEntity
                {
                    InputFileName = item1.FileName,
                    InputFilePath = item1.FilePath,
                    OutoutFilePath = item2.FilePath,
                    OutoutFileName = item2.FileName,
                });
            }
            else if(item1 is null && item2 is not null)
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
    }
}
