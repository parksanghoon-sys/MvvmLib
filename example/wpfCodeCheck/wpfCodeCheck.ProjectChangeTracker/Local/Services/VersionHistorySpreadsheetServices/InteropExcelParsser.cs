using DocumentFormat.OpenXml.Drawing;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using TextCopy;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.ProjectChangeTracker.Local.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using Excel = Microsoft.Office.Interop.Excel;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services
{
    public class InteropExcelParsser : IExcelPaser
    {
        private string _filePath;
        private Excel.Application excelApp = null;
        private Excel.Workbook workbook = null;
        private Excel.Worksheet worksheet = null;
        private int _startRowIndex;
        private int _startCellIndex;
        private int _excelIndex = 1;
        private string _resultFilePath = @"D:\Temp\result.txt";
        private static ConcurrentQueue<CompareEntity> _queue = new();
        //private static FileSystemWatcher _watcher;
        private static AutoResetEvent _fileWrittenEvent = new AutoResetEvent(false);
        //private static SemaphoreSlim _fileSemaphore = new SemaphoreSlim(1, 1);
        private readonly ICsvHelper? _csvHelper;
        private readonly IBaseService _baseService;
        private List<FailClassAnalysisModel> _failClassAnalysisModels = new();

        public InteropExcelParsser(ICsvHelper? csvHelper, IBaseService baseService)
        {
            _csvHelper = csvHelper;
            _baseService = baseService;
        }     
        public async Task<bool> WriteExcelAync(FileEntity inputFile, FileEntity outputFile)
        {
            bool isResult = false;
            
            await Task.Run(async () =>
            {
                try
                {
                    if (_baseService.CompareResult is null)
                    {
                        Console.WriteLine("Not Data");
                        return;
                    }
                    excelApp = new Excel.Application();
                    workbook = excelApp.Workbooks.Open(_filePath);
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;

                    // 새 워크북 추가                    
                    worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                    _startCellIndex = FindWriteCellLastRowIndes();

                    var processResult = ProcessBeyondCompareCliExcuteion(inputFile.FilePath, outputFile.FilePath);

                    //_fileSemaphore.Release();
                    CustomCodeComparer project = new CustomCodeComparer()
                    {
                        InputFileName = inputFile.FileName,
                        OutoutFileName = outputFile.FileName,
                        InputFilePath = inputFile.FilePath,
                        OutoutFilePath = outputFile.FilePath
                    };

                    _queue.Enqueue(project);
                    //_fileWrittenEvent.WaitOne();
                    //Thread.Sleep(100);
                    isResult = await WriteExcelAsync();
                }
                catch (Exception)
                {

                }
                finally
                {
                    workbook.Save();
                    workbook.Close();
                    excelApp.Quit();
                    // COM 개체 해제
                    if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                    if (workbook != null) Marshal.ReleaseComObject(workbook);
                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        Marshal.ReleaseComObject(excelApp);
                    }

                    // 가비지 컬렉션 강제 호출
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                }
            });
            return isResult;
        }
        public async Task<bool> WriteExcelAync(string fileFullName)
        {
            _filePath = fileFullName;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await Task.Run(async () =>
            {
                try
                {
                    if (_baseService.CompareResult is null)
                    {
                        Console.WriteLine("Not Data");
                        return;
                    }
                    excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;

                    // 새 워크북 추가
                    workbook = excelApp.Workbooks.Add();
                    worksheet = (Excel.Worksheet)workbook.Worksheets[1];

                    //_watcher = new FileSystemWatcher(Path.GetDirectoryName(_resultFilePath));
                    //_watcher.Filter = Path.GetFileName(_resultFilePath);
                    //_watcher.NotifyFilter = NotifyFilters.LastWrite;
                    //_watcher.Changed += OnFileWatchChanged;
                    //_watcher.EnableRaisingEvents = true;
                    
                    _startCellIndex = 1;
                    foreach (var project in _baseService.CompareResult.CompareResults)
                    {
                        //ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\Beyond Compare 4\BComp.com");
                        //startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        //startInfo.Arguments = $"""
                        //                        "@{DirectoryHelper.GetLocalSettingDirectory()}\beyondCli.txt" "{project.InputFilePath}" "{project.OutoutFilePath}" "{_resultFilePath}"
                        //                       """;

                        //// 출력 캡처를 위한 설정
                        //startInfo.RedirectStandardOutput = true;
                        //startInfo.RedirectStandardError = true;
                        //startInfo.UseShellExecute = false;
                        //startInfo.CreateNoWindow = true;


                        //_fileSemaphore.Wait();
                        // 프로세스 시작
                        var processResult = ProcessBeyondCompareCliExcuteion(project.InputFilePath, project.OutoutFilePath);
                        //_fileSemaphore.Release();
                        _queue.Enqueue(project);

                        await WriteExcelAsync();

                    }
                    // Wait until all files are processed
                    //while (!_queue.IsEmpty)
                    //{
                    //    OnFileWatchChanged(null, null);
                    //    await Task.Delay(100); // Polling interval
                    //}


                }
                catch (Exception)
                {

                }
                finally
                {
                    workbook.SaveAs(_filePath);
                    // COM 개체 해제
                    if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                    if (workbook != null) Marshal.ReleaseComObject(workbook);
                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        Marshal.ReleaseComObject(excelApp);
                    }

                    // 가비지 컬렉션 강제 호출
                    GC.Collect();
                    GC.WaitForPendingFinalizers(); 

                }
            });
            stopwatch.Stop();
            // 경과 시간 출력 (시:분:초:밀리초)
            Debug.WriteLine($"Elapsed time: {stopwatch.Elapsed}");

            if (File.Exists(_filePath + ".csv") is true)
            {
                string json = JsonConvert.SerializeObject(_failClassAnalysisModels);
                string jsonFilePath = _filePath.Replace(".xlsx", ".json");
                File.WriteAllText(jsonFilePath, json);
                _failClassAnalysisModels.Clear();
                return false;
            }
            return true;

        }
        private async Task<bool> WriteExcelAsync()
        {
            if (File.Exists(_resultFilePath))
            {
                CompareEntity customCodeComparer = null;
                try
                {
                    if (_queue.TryDequeue(out customCodeComparer) == true)
                    {
                        if (customCodeComparer != null)
                        {
                            StringBuilder htmlContent = new StringBuilder();
                            string[] excludeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "File:", "Left file:", "Right file:", "Mode:&nbsp; Differences &nbsp;", "Text Compare", "Produced:" };

                            var strs = await SafeReadFromFileAsync(_resultFilePath);
                            foreach (string str in strs)
                            {
                                if (str == "&nbsp; &nbsp;")
                                    continue;
                                if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
                                {
                                    htmlContent.Append(str);
                                }
                                else
                                {
                                    //Debug.Print(str);
                                }
                            }                            
                            int lastcell = _startCellIndex;
                            // Copy HTML data to clipboard
                            ClipboardService.SetText(htmlContent.ToString());
                            Thread.Sleep(10);
                            // Paste content to Excel
                            Excel.Range cell = worksheet.Cells[_startCellIndex, ECELL.COL_INPUT_LINE];
                            cell.Select();
                            worksheet.Paste();
                            Thread.Sleep(10);

                            // Remove processed item from queue
                            _startCellIndex = FindWriteCellLastRowIndes();

                            if (lastcell == _startCellIndex)
                            {
                                FailClassAnalysisModel fail = new FailClassAnalysisModel()
                                {
                                    InputFile = new FileEntity()
                                    {
                                        FileName = customCodeComparer!.InputFileName,
                                        FilePath = customCodeComparer!.InputFilePath,
                                    },
                                    OutputFile = new FileEntity()
                                    {
                                        FileName = customCodeComparer.OutoutFileName,
                                        FilePath = customCodeComparer.OutoutFilePath
                                    }
                                };

                                _failClassAnalysisModels.Add(fail);
                                return true;
                            }
                                
                            Excel.Range rgIndex = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_INDEX], worksheet.Cells[_startCellIndex - 1, ECELL.COL_INDEX]];
                            Excel.Range rgDataName = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_DATA_NAME], worksheet.Cells[_startCellIndex - 1, ECELL.COL_DATA_NAME]];
                            Excel.Range rgDatasheetNumber = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_DATASHEET_NUMBER], worksheet.Cells[_startCellIndex - 1, ECELL.COL_DATASHEET_NUMBER]];

                            Excel.Range rgName = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_CLASSNAME], worksheet.Cells[_startCellIndex - 1, ECELL.COL_CLASSNAME]];
                            Excel.Range rgSummery = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_SUMMARY_CELL + 1], worksheet.Cells[_startCellIndex - 1, ECELL.COL_SUMMARY_CELL + 1]];
                            Excel.Range rgIssue = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_ISSUE + 1], worksheet.Cells[_startCellIndex - 1, ECELL.COL_ISSUE + 1]];
                            Excel.Range rgCode = worksheet.Range[worksheet.Cells[lastcell, ECELL.COL_CODE + 1], worksheet.Cells[_startCellIndex - 1, ECELL.COL_CODE+ 1]];

                            rgIndex.Merge();
                            rgDataName.Merge();
                            rgDatasheetNumber.Merge();
                            rgName.Merge();
                            rgSummery.Merge();
                            rgIssue.Merge();
                            rgCode.Merge();

                            rgIndex.Value = _excelIndex.ToString();
                            rgName.Value = customCodeComparer.InputFileName == string.Empty ? customCodeComparer.OutoutFileName : customCodeComparer.InputFileName;
                            rgDataName.Value = """
                                                                B6체계 MQ-105K
                                주부조종사운용장치
                                소프트웨어산출물명세서
                                (80445501SPS)
                                """;
                            rgDatasheetNumber.Value = "80445501SPS";
                            rgSummery.Value = "o 기능개선\r\n : 항전개조 1단계 적용 \r\n OT보완 개선사항 반영";
                            rgIssue.Value = "영향없음";
                            rgCode.Value = "A6";

                            rgIndex.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rgIndex.Borders.Weight = Excel.XlBorderWeight.xlThin;

                            rgDataName.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rgDataName.Borders.Weight = Excel.XlBorderWeight.xlThin;

                            rgDatasheetNumber.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rgDatasheetNumber.Borders.Weight = Excel.XlBorderWeight.xlThin;

                            rgName.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rgName.Borders.Weight = Excel.XlBorderWeight.xlThin;

                            rgSummery.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rgSummery.Borders.Weight = Excel.XlBorderWeight.xlThin;

                            Marshal.ReleaseComObject(rgIndex);
                            Marshal.ReleaseComObject(rgName);
                            Marshal.ReleaseComObject(rgSummery);
                            Marshal.ReleaseComObject(rgDataName);
                            Marshal.ReleaseComObject(rgDatasheetNumber);

                            _excelIndex++;
                        }

                    }
                    //_fileWrittenEvent.Set();
                   //_fileSemaphore.Release();                    

                }
                catch (Exception ex)
                {
                    var csvExceptionData = new string[] { customCodeComparer!.InputFileName, customCodeComparer.OutoutFileName, customCodeComparer.InputFilePath, customCodeComparer.OutoutFilePath };
                    FailClassAnalysisModel fail = new FailClassAnalysisModel()
                    {
                        InputFile = new FileEntity()
                        {
                            FileName = customCodeComparer!.InputFileName,
                            FilePath = customCodeComparer!.InputFilePath,
                        },
                        OutputFile = new FileEntity()
                        {
                            FileName = customCodeComparer.OutoutFileName,
                            FilePath = customCodeComparer.OutoutFilePath
                        }
                    };
                        
                    _failClassAnalysisModels.Add(fail);

                    _csvHelper!.ExcepCOMExceptionToCsv(_filePath + ".csv", csvExceptionData);
                    Debug.WriteLine($"File read error: {ex.Message} Error Input : {customCodeComparer.InputFileName} , {customCodeComparer.OutoutFileName}");
                    return false;
                }
                finally
                {
                    Thread.Sleep(50);
                }
                return true;
            }
            return false;
        }
        private string ProcessBeyondCompareCliExcuteion(string inputFilePath, string outpuFilePath)
        {
            string processResult; 
            ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\Beyond Compare 4\BComp.com");
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.Arguments = $"""
                                                "@{DirectoryHelper.GetLocalSettingDirectory()}\beyondCli.txt" "{inputFilePath}" "{outpuFilePath}" "{_resultFilePath}"
                                               """;

            // 출력 캡처를 위한 설정
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            //_fileSemaphore.Wait();
            // 프로세스 시작
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                // 표준 출력 및 표준 오류 출력 읽기
                string output = process.StandardOutput.ReadToEnd();
                processResult = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(processResult))
                {
                    Debug.WriteLine("Error:");
                    Debug.WriteLine(processResult);
                }
            }
            return processResult;
        }
        //private async Task OnFileWatchChanged(object sender, FileSystemEventArgs e)
        //{
        //    if (File.Exists(_resultFilePath))
        //    {
        //        try
        //        {
        //            CustomCodeComparer customCodeComparer = null;
        //            if (_queue.TryDequeue(out customCodeComparer) == true)
        //            {
        //                _fileSemaphore.Wait();
        //                StringBuilder htmlContent = new StringBuilder();
        //                string[] excludeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "Left file:", "File:", "Right file:", "Mode:&nbsp; Differences &nbsp;", "Text Compare", "Produced:" };
        //                //string[] strs = File.ReadAllLines(_resultFilePath);

        //                //using (StreamReader reader = new StreamReader(_resultFilePath))
        //                //{
        //                //    string line;
        //                //    while ((line = reader.ReadLine()) != null)
        //                //    {
        //                //        if (!excludeStrs.Any(excludeStr => line.Contains(excludeStr)))
        //                //        {
        //                //            htmlContent.Append(line);
        //                //        }
        //                //        //htmlContent.Append(line);
        //                //    }
        //                //}
        //                var strs = await SafeReadFromFileAsync(_resultFilePath);
        //                foreach (string str in strs)
        //                {

        //                    if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
        //                    {
        //                        htmlContent.Append(str);
        //                    }
        //                }
        //                // Copy HTML data to                        
        //                ClipboardService.SetText(htmlContent.ToString());


        //                // Paste content to Excel
        //                Excel.Range cell = worksheet.Cells[_startCellIndex, 2];
        //                cell.Select();
        //                worksheet.Paste();
        //                Thread.Sleep(50);
        //                Excel.Range cellinputClass = worksheet.Cells[_startCellIndex, 1];
        //                Excel.Range cellOutputClass = worksheet.Cells[_startCellIndex, 7];


        //                cellinputClass.Value = customCodeComparer.InputFileName;
        //                cellOutputClass.Value = customCodeComparer.OutoutFileName;
        //                // Remove processed item from queue

        //                _startCellIndex = FindWriteCellLastRowIndes();
        //                Debug.WriteLine($"input file: {customCodeComparer.InputFileName}");
        //                Debug.WriteLine($"output file: {customCodeComparer.OutoutFileName}");

        //                if (_startCellIndex <= 0)
        //                {
        //                    Debug.WriteLine($"start Index is under zero");
        //                }
        //                else
        //                {
        //                    Debug.WriteLine($"start Index is {_startCellIndex}");
        //                }
        //                //_fileWrittenEvent.Set();
        //                _fileSemaphore.Release();
        //            }


        //        }
        //        catch (IOException ex)
        //        {
        //            Console.WriteLine($"File read error: {ex.Message}");
        //        }
        //        catch (System.Runtime.InteropServices.COMException ex)
        //        {
        //            Console.WriteLine("COM 오류 발생: " + ex.Message);
        //        }
        //        finally
        //        {

        //        }
        //    }
        //}

        private int FindWriteCellLastRowIndes()
        {
            int lastRowA = GetLastUsedRow(worksheet, ECELL.COL_INPUT_LINE);
            int lastRowB = GetLastUsedRow(worksheet, ECELL.COL_INPUT_CODE);
            int lastRowD = GetLastUsedRow(worksheet, ECELL.COL_OUTPUT_LINE + 1);
            int lastRowE = GetLastUsedRow(worksheet, ECELL.COL_OUTPUT_CODE + 1);

            try
            {
                int lastRow = Math.Max(Math.Max(lastRowA, lastRowB), Math.Max(lastRowD, lastRowE)) + 1;
                return lastRow;

            }
            catch (Exception)
            {

                return 1;
            }
        }
        private int GetLastUsedRow(Excel.Worksheet worksheet, ECELL column)
        {
            Excel.Range columnRange = worksheet.Columns[column];

            Excel.Range lastCell = columnRange.Cells.Find("*", System.Reflection.Missing.Value,
                Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,
                Excel.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);

            return lastCell != null ? lastCell.Row : 1; // 셀이 없으면 기본적으로 1행 반환
        }
        private async Task<string[]> SafeReadFromFileAsync(string path)
        {
            int maxRetry = 3;
            int delay = 1000;
            IList<string> files = new List<string>();
            for (int i = 0; i < maxRetry; i++)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            files.Add(line);
                        }
                        break;
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(delay);
                }
            }
            return files.ToArray();
        }


    }
}
