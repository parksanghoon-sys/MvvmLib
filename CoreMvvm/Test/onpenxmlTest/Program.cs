using ClosedXML.Excel;
using onpenxmlTest;
using System.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        string CSV_DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Data//");


        string filePath1 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight\ViewModelLib\StObservableCollection.cs";
        string filePath2 = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21_PPC_소스파일\Source\FlightSolution\ViewModelLib\StObservableCollection.cs";
        
        string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
        string copyExcelFilePath = CSV_DATA_PATH + Path.GetFileName("SW_Chage_Test.xlsx");
        
        CreatePathFolder(CSV_DATA_PATH);

        if(File.Exists(copyExcelFilePath) == true)
        {
            File.Delete(copyExcelFilePath);
        }

        File.Copy(baseExcelFilepath, copyExcelFilePath);        

        IExcelPaser excelPaser = new OpenXmlUsed();

        excelPaser.Parsing(copyExcelFilePath);


        Console.WriteLine("Hello, World!");
    }

    public static void CreatePathFolder(string path)
    {
        string[] folderNames = path.Split('\\');
        string fullPath = string.Empty;
        for (int i = 0; i < folderNames.Length - 1; i++)
        {
            fullPath += folderNames[i] + '\\';
            DirectoryInfo di = new DirectoryInfo(fullPath);
            if (!di.Exists) di.Create();
        }
    }
}