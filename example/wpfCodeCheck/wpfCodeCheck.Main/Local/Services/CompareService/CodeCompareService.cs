using CompareEngine;
using System.Collections;
using System.IO;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Models.Compare;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.CompareServices;


namespace wpfCodeCheck.Main.Local.Servies
{
    public class CodeCompareService : CompareBaseService
    {
        public CodeCompareService(ICsvHelper csvHelper)
            :base(csvHelper) 
        {            
        }

        #region Override Method
        public override async Task<List<FileCompareModel>> CompareModelCollections(IList<FileCompareModel> inputItems, IList<FileCompareModel> outputItems)
        {
            List<FileCompareModel> diffResultModel = new();
            int i = 0, j = 0;
            while (i < inputItems.Count && j < outputItems.Count)
            {
                
                FileCompareModel item1 = inputItems[i];
                FileCompareModel item2 = outputItems[j];

                int comparison = string.Compare(item1.FileName, item2.FileName);
                if (comparison == 0)
                {
                    bool comparisonResult = item1.Equals(item2);

                    item1.IsComparison = comparisonResult;
                    item2.IsComparison = comparisonResult;

                    i++;
                    j++;
                    
                    // 평면화된 리스트이므로 모든 항목이 SOURCECODE 파일임
                    if (comparisonResult == false)
                    {
                        AddCodeCompreResult(GetCompareResult(item1, item2));

                        _code1.Add(item1);
                        _code2.Add(item2);

                        AddSwDetailItem(item1, item2 );
                        diffReulstModel.Add(new CompareEntity
                        {
                            InputFileName = item1.FileName,
                            InputFilePath = item1.FilePath,
                            OutoutFilePath = item2.FilePath,
                            OutoutFileName = item2.FileName,
                        });
                    }
                }
                else if (comparison < 0)
                {
                    // INPUT에만 있는 파일
                    AddCodeCompreResult(GetCompareResult(item1, new FileCompareModel()));
                    item1.IsComparison = false;
                    _code1.Add(item1);
                    AddSwDetailItem(item1, null);
                    diffReulstModel.Add(new CompareEntity
                    {
                        InputFileName = item1.FileName,
                        InputFilePath = item1.FilePath,
                        OutoutFilePath = string.Empty,
                        OutoutFileName = string.Empty,
                    });
                    i++;
                }
                else
                {
                    // OUTPUT에만 있는 파일
                    AddCodeCompreResult(GetCompareResult(new FileCompareModel(), item2));

                    item2.IsComparison = false;
                    _code2.Add(item2);
                    AddSwDetailItem(null, item2);

                    diffReulstModel.Add(new CompareEntity
                    {
                        InputFileName = string.Empty,
                        InputFilePath = string.Empty,
                        OutoutFilePath = item2.FilePath,
                        OutoutFileName = item2.FileName,
                    });
                    j++;
                }

            }

            // 남은 INPUT 파일들 처리
            while (i < inputItems.Count)
            {
                inputItems[i].IsComparison = false;
                _code1.Add(inputItems[i]);
                AddCodeCompreResult(GetCompareResult(inputItems[i], new FileCompareModel()));
                AddSwDetailItem(inputItems[i], null);
                diffReulstModel.Add(new CompareEntity
                {
                    InputFileName = inputItems[i].FileName,
                    InputFilePath = inputItems[i].FilePath,
                    OutoutFilePath = string.Empty,
                    OutoutFileName = string.Empty,
                });
                i++;
            }

            // 남은 OUTPUT 파일들 처리
            while (j < outputItems.Count)
            {
                outputItems[j].IsComparison = false;
                _code2.Add(outputItems[j]);
                AddCodeCompreResult(GetCompareResult(new FileCompareModel(), outputItems[j]));
                AddSwDetailItem(null, outputItems[j]);
                diffReulstModel.Add(new CompareEntity
                {
                    InputFileName = string.Empty,
                    InputFilePath = string.Empty,
                    OutoutFilePath = outputItems[j].FilePath,
                    OutoutFileName = outputItems[j].FileName,
                });
                j++;
            }
            _code2.Distinct();
            _code1.Distinct();

            return diffReulstModel;
        }
    
        #endregion
        public CompareEntity GetCompareResult(FileItem inputModel, FileItem outputModel)
        {
            if (inputModel.FileSize == null && outputModel.FileSize == null)
                return null;

            CompareText sourceDiffList;
            CompareText destinationDiffList;

            if (File.Exists(inputModel.FilePath) && !Directory.Exists(inputModel.FilePath))
                sourceDiffList = new CompareText(inputModel.FilePath);
            else
                sourceDiffList = new CompareText();

            if (File.Exists(outputModel.FilePath) && !Directory.Exists(outputModel.FilePath))
                destinationDiffList = new CompareText(outputModel.FilePath);
            else
                destinationDiffList = new CompareText();

            CompareEngine.CompareEngine compareEngine = new CompareEngine.CompareEngine();
            compareEngine.StartDiff(sourceDiffList, destinationDiffList);

            ArrayList resultLines = compareEngine.DiffResult();
            CustomCodeComparer compareResult = new CustomCodeComparer();
            compareResult.InputCompareText = sourceDiffList;
            compareResult.OutputCompareText = destinationDiffList;
            compareResult.CompareResultSpans = GetArrayListToList<CompareResultSpan>(resultLines);

            compareResult.InputFileName = inputModel.FileName;
            compareResult.OutoutFileName = outputModel.FileName;

            compareResult.InputFilePath = inputModel.FilePath;
            compareResult.OutoutFilePath = outputModel.FilePath;

            return compareResult;
        }
        private IList<T> GetArrayListToList<T>(ArrayList list)
        {
            List<T> result = new List<T>();

            foreach (var item in list)
            {
                if (item is T)
                {
                    result.Add((T)item);
                }
            }

            return result;
        }

        private bool AddCodeCompreResult(CompareEntity customCodeComparer)
        {
            if (customCodeComparer is null)
                return false;
            if (_codeCompareModel.CompareResults.Contains(customCodeComparer) == false)
            {
                _codeCompareModel.CompareResults.Add(customCodeComparer);
            }
            return true;
        }

        
    }
}
