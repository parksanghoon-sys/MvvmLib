using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // var summary = BenchmarkRunner.Run<Benchmarks>();

        //string filePath1 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight";
        //string filePath2 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21. PPC 소스파일\Source\FlightSolution";

        //var dierctoryFileInfo1 = new SourceDirectoryService(new Crc32FileChecSum());
        //var dierctoryFileInfo2 = new SourceDirectoryService(new Crc32FileChecSum());

        //var fileList = await dierctoryFileInfo1.GetDirectoryCodeFileInfosAsync(filePath1);
        //var secondsList = await dierctoryFileInfo2.GetDirectoryCodeFileInfosAsync(filePath2);

        //var list1 = fileList.OrderBy(i => i.FileName).ToList();
        //var list2 = secondsList.OrderBy(i => i.FileName).ToList();


        //CompareModelCollections(list1, list2);
        //int diff1 = 0, diff2 = 0;
        //foreach (var file in fileList)
        //{
        //    if (file.ComparisonResult == true)
        //    {
        //        Console.WriteLine(file.ToString());
        //        diff1++;
        //    }
        //}

        //Console.WriteLine("/////////////////////////////////////////////////////////////////////////////////////////////////////");

        //foreach (var file in secondsList)
        //{
        //    if (file.ComparisonResult == true)
        //    {
        //        Console.WriteLine(file.ToString());
        //        diff2++;
        //    }
        //}

        //Console.WriteLine($"dif1 : {diff1} dif2 : {diff2} ");
    }
    //static void CompareModelCollections(IList<CodeInfoModel> collection1, IList<CodeInfoModel> collection2)
    //{
    //    int i = 0, j = 0;
    //    while (i < collection1.Count && j < collection2.Count)
    //    {
    //        CodeInfoModel model1 = collection1[i];
    //        CodeInfoModel model2 = collection2[j];

    //        int comparison = string.Compare(model1.FileName, model2.FileName);
    //        if (comparison == 0)
    //        {
    //            bool comparisonResult = model1.Equals(model2);
    //            if (comparisonResult == true)
    //            {
    //                model1.ComparisonResult = comparisonResult;
    //                model2.ComparisonResult = comparisonResult;
    //            }

    //            i++;
    //            j++;
    //        }
    //        else if (comparison < 0)
    //        {
    //            model1.ComparisonResult = false;
    //            i++;
    //        }
    //        else
    //        {
    //            model2.ComparisonResult = false;
    //            j++;
    //        }
    //    }

    //    // Remaining elements in collection1 are not in collection2
    //    while (i < collection1.Count)
    //    {
    //        collection1[i].ComparisonResult = false;
    //        i++;
    //    }

    //    // Remaining elements in collection2 are not in collection1
    //    while (j < collection2.Count)
    //    {
    //        collection2[j].ComparisonResult = false;
    //        j++;
    //    }
    //}
}