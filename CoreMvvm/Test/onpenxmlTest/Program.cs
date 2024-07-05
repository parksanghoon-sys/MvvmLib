using ClosedXML.Excel;
using onpenxmlTest;
using System.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        string CSV_DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Data//");

        List<TestData> dataList = new List<TestData>()
        { 
            new TestData("1","CTest1","1","VTest1"),                        
            new TestData("2","CTest1","",""),                        
            new TestData("","","3","VTest3"),                        
            new TestData("4","CTest1","1","VTest4"),                        
            new TestData("5","CTest1","61","VTes5"),                        
            new TestData("6","CTest1","",""),                        
        };
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

        OpenXmlUsed excelPaser = new OpenXmlUsed(copyExcelFilePath);
        
        excelPaser.SetStartCellName("F454");
        excelPaser.SetExcelDate(dataList);



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
public record TestData
{
    public TestData(string currentIndex = "", string currentValue = "", string previewIndex = "", string previewValue = "")
    {
        CurrentIndex = currentIndex;
        CurrrentValue = currentValue;
        PrevewIndex = previewIndex;
        PreviewValue = previewValue;
    }    
    public string CurrrentValue { get; set; } = string.Empty;
    public string PreviewValue { get; set; } = string.Empty;
    public string CurrentIndex { get; set; }
    public string PrevewIndex {  get; set; }
}

