using onpenxmlTest;
using System.Diagnostics;

internal class Program
{
    [STAThread] 
    private static async Task Main(string[] args)
    {
        //string CSV_DATA_PATH = Path.Combine(Environment.CurrentDirectory, @"ExportExcel\");

        ////List<TestData> dataList = new List<TestData>()
        ////{
        ////    new TestData("1","CTest1","1","VTest1"),
        ////    new TestData("2","CTest1","",""),
        ////    new TestData("","","3","VTest3"),
        ////    new TestData("4","CTest1","1","VTest4"),
        ////    new TestData("5","CTest1","61","VTes5"),
        ////    new TestData("6","CTest1","",""),
        ////};
        //CompareResult dataList = new CompareResult();
        //string outputFilePath = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight\UvhfControls\UvhfMainStatus\UvhfStatus.xaml";
        //string inputFIlePath = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21_PPC_소스파일\Source\FlightSolution\UvhfControls\UvhfMainStatus\UvhfStatus.xaml";

        //string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
        //string copyExcelFilePath = CSV_DATA_PATH + Path.GetFileName("SW_Chage_Test.xlsx");

        //CreatePathFolder(CSV_DATA_PATH);

        //if (File.Exists(copyExcelFilePath) == true)
        //{
        //    File.Delete(copyExcelFilePath);
        //}

        //File.Copy(baseExcelFilepath, copyExcelFilePath);

        //CLIComparer.GetFiles(new string[] { inputFIlePath, outputFilePath });
        //dataList = CLIComparer.GetCompareFiles();

        //IExcelPaser excelPaser = new ClosedXmlUsed(copyExcelFilePath);

        //excelPaser.SetStartCellName("E454");
        //excelPaser.SetExcelDate(dataList);
        //await excelPaser.WriteExcel();


        ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\Beyond Compare 4\BComp.com");
        startInfo.WindowStyle = ProcessWindowStyle.Minimized;
        startInfo.Arguments = """
            "@D:\temp\a.txt" "D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21_PPC\Source\FlightSolution\CduBackupControl\ConfigTab\ConfigTabViewModel.cs" "D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight\CduBackupControl\ConfigTab\ConfigTabViewModel.cs" "D:\Temp\result.txt"
            """;
        Process.Start(startInfo);
   

        var interop = new Interop();
        Console.WriteLine("Hello, World!");
        // 가비지 컬렉션 강제 호출
        GC.Collect();
        GC.WaitForPendingFinalizers();
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
    public TestData(string inputIndex = "", string inputCode = "", string outputIndex= "", string outputValue = "")
    {
        InputIndex = inputIndex;
        InputCode = inputCode;
        OutputIndex = outputIndex;
        OutputCode = outputValue;
    }    
    public string InputIndex { get; set; } = string.Empty;
    public string InputCode { get; set; } = string.Empty;
    public string OutputIndex { get; set; }
    public string OutputCode {  get; set; }
}

