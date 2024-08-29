using CompareEngine;
using System.Collections;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public class CodeCompareService : ICompare
    {
        public CompareEntity GetCompareResult(FileItem inputModel, FileItem outputModel)
        {
            if (inputModel.FileSize == "0" && outputModel.FileSize == "0")
                return null;

            CompareText sourceDiffList;
            CompareText destinationDiffList;

            if (File.Exists(inputModel.FilePath))
                sourceDiffList = new CompareText(inputModel.FilePath);
            else
                sourceDiffList = new CompareText();

            if (File.Exists(outputModel.FilePath))
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
    }
}
