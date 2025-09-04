//using wpfCodeCheck.Domain.Models;
//using wwpfCodeCheck.Domain.Services.Interfaces;
//using wpfCodeCheck.Main.Local.Services.FileDescription;
//using wpfCodeCheck.Domain.Local.Helpers;
//using System.IO;


//namespace wpfCodeCheck.Main.Local.Services.CompareService
//{
//    public class FileCompareService : ICompareService
//    {
//        private readonly ICsvHelper _csvHelper;
//        private readonly ISettingService _settingService;
//        private readonly IFileDescriptionService _fileDescriptionService;
        
//        // 누락된 필드들 추가
//        private readonly List<FileTreeModel> _code1 = new();
//        private readonly List<FileTreeModel> _code2 = new();

//        public FileCompareService(ICsvHelper csvHelper, ISettingService settingService, IFileDescriptionService fileDescriptionService)            
//        {
//            _csvHelper = csvHelper;
//            _settingService = settingService;
//            _fileDescriptionService = fileDescriptionService;
//        }

//        private string GetRelativePath(string fullPath, string basePath)
//        {
//            // basePath 이후의 경로 추출
//            if (fullPath.StartsWith(basePath))
//            {
//                return fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
//            }
//            return fullPath;
//        }

//        public async Task<List<FileTreeModel>> CompareFileTreesAsync(IList<FileTreeModel> inputTree, IList<FileTreeModel> outputTree)
//        {
//            var differenceFiles = new List<FileTreeModel>();
//            await CompareModelCollections(inputTree, outputTree, differenceFiles);
//            return differenceFiles;
//        }

//        private async Task CompareModelCollections(IList<FileTreeModel> inputItems, IList<FileTreeModel> outputItems, List<FileTreeModel> differenceFiles)
//        {
//            List<CompareEntity> diffReulstModel = new();
//            List<string> allPaths;

//            var inputBasePath = _settingService.GeneralSetting!.InputPath;
//            var outputBasePath = _settingService.GeneralSetting!.OutputPath;

//            var dictInput = inputItems?.ToDictionary(item => GetRelativePath(item.FilePath, inputBasePath)) ?? new Dictionary<string, FileTreeModel>();
//            var dictOutput = outputItems?.ToDictionary(item => GetRelativePath(item.FilePath, outputBasePath)) ?? new Dictionary<string, FileTreeModel>();
//            allPaths = dictInput?.Keys.Union(dictOutput?.Keys).ToList();

//            //if (dictOutput is not null && dictInput is not null )            

//            //else            
//            //    allPaths = dictInput.Keys.ToList() ?? dictOutput.Keys.ToList();            



//            foreach (var path in allPaths)
//            {
//                if (dictInput.TryGetValue(path, out var item1) && dictOutput.TryGetValue(path, out var item2))
//                {
//                    if (item1 is not null && item2 is not null)
//                    {                        
//                        // 두 항목이 같은지 비교
//                        if (item1.Equals(item2))
//                        {
//                            item1.IsComparison = item2.IsComparison = true;
//                        }
//                        else
//                        {
//                            if (item1.Checksum != "" && item2.Checksum != "")
//                            {
//                                item1.IsComparison = item2.IsComparison = false;
//                                var desc1 = _fileDescriptionService.GetDescription(item1.FilePath, item1.FileName);
//                                var desc2 = _fileDescriptionService.GetDescription(item2.FilePath, item2.FileName);
//                                item1.Description = desc1.Description;
//                                item2.Description = desc2.Description;
                                
//                                // 차이가 있는 파일을 결과 목록에 추가
//                                differenceFiles.Add(item1);
//                                differenceFiles.Add(item2);

//                            }
//                        }

//                        if (item1.Children != null && item2.Children != null)
//                        {
//                            // 하위 항목 비교
//                            await CompareModelCollections(item1.Children, item2.Children, differenceFiles);
//                        }
//                    }
//                }
//                else
//                {
//                    // 한쪽에만 파일이나 폴더가 있는 경우 false
//                    if (dictInput.TryGetValue(path, out item1))
//                    {
//                        if (item1.Checksum != "")
//                        {
//                            item1.IsComparison = false;
//                            var desc1 = _fileDescriptionService.GetDescription(item1.FilePath, item1.FileName);
//                            item1.Description = desc1.Description;
                            
//                            // 한쪽에만 있는 파일 추가
//                            differenceFiles.Add(item1);
//                        }
//                        else
//                        {
//                            if (item1.Children is not null)
//                                // 하위 항목 비교
//                                await CompareModelCollections(item1.Children!, null, differenceFiles);
//                        }
//                    }
//                    if (dictOutput.TryGetValue(path, out item2))
//                    {
//                        if (item2.Checksum != "")
//                        {
//                            item2.IsComparison = false;
//                            var desc2 = _fileDescriptionService.GetDescription(item2.FilePath, item2.FileName);
//                            item2.Description = desc2.Description;
                            
//                            // 한쪽에만 있는 파일 추가
//                            differenceFiles.Add(item2);
//                        }
//                        else
//                        {
//                            // 하위 항목 비교
//                            if (item2 is not null && item2.Children is not null)
//                                await CompareModelCollections(null, item2.Children!, differenceFiles);
//                        }

//                    }
//                }
//            }
//        }
//    }
//}
