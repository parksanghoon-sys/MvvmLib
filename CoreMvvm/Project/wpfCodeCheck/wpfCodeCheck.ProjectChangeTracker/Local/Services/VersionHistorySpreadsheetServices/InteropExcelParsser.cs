using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Markup.Localizer;
using TextCopy;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Services;
using Excel = Microsoft.Office.Interop.Excel;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services
{
    public class InteropExcelParsser : IExcelPaser
    {
        private readonly string _filePath;
        private CodeDiffReulstModel<CustomCodeComparer>? _dataList;
        private Excel.Application excelApp = null;
        private Excel.Workbook workbook = null;
        private Excel.Worksheet worksheet = null;
        private int _startRowIndex;
        private int _startCellIndex;
        private string _resultFilePath = @"D:\Temp\result.txt";
        private static ConcurrentQueue<CustomCodeComparer> _queue = new();
        private static FileSystemWatcher _watcher;
        private static AutoResetEvent _fileWrittenEvent = new AutoResetEvent(false);
        private static SemaphoreSlim _fileSemaphore = new SemaphoreSlim(1, 1);

        public InteropExcelParsser(string filePath)
        {
            _filePath = filePath;
        }
        public void SetExcelData(CodeDiffReulstModel<CustomCodeComparer> dataList)
        {
            _dataList = dataList;
        }

        public async Task WriteExcelAync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (_dataList is null)
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

                    // 지정된 열에서 마지막 사용된 행 찾기
                    _startCellIndex = 1;
                    foreach (var project in _dataList.CompareResults)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\Beyond Compare 4\BComp.com");
                        startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        startInfo.Arguments = $"""
                                                "@D:\temp\a.txt" "{project.InputFilePath}" "{project.OutoutFilePath}" "{_resultFilePath}"
                                               """;


                        _fileSemaphore.Wait();
                        Process.Start(startInfo);
                        _fileSemaphore.Release();

                        _queue.Enqueue(project);
                        //_fileWrittenEvent.WaitOne();
                        await Task.Delay(300);
                        WriteExcelAsync();
                        
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
        }
        private void WriteExcelAsync()
        {
            if (File.Exists(_resultFilePath))
            {
                CustomCodeComparer customCodeComparer = null;
                try
                {                    
                    if (_queue.TryDequeue(out customCodeComparer) == true)
                    {
                        if (customCodeComparer != null)
                        {
                            //_fileSemaphore.Wait();
                            StringBuilder htmlContent = new StringBuilder();
                            string[] excludeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "Left file:", "Right file:", "Mode:&nbsp; Differences &nbsp;", "Text Compare", "Produced:" };
                            //string[] strs = File.ReadAllLines(_resultFilePath);

                            var strs = SafeReadFromFile(_resultFilePath);
                            foreach (string str in strs)
                            {
                                if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
                                {
                                    htmlContent.Append(str);
                                }
                            }

                            //foreach (string str in strs)
                            //{
                            //    if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
                            //    {
                            //        htmlContent.Append(str + Environment.NewLine);
                            //    }
                            //    //htmlContent.Append(str);
                            //}

                            // Copy HTML data to clipboard
                            ClipboardService.SetText(htmlContent.ToString());
                            Debug.WriteLine($"input file: {customCodeComparer.InputFileName}");
                            Debug.WriteLine($"output file: {customCodeComparer.OutoutFileName}");
                            // Paste content to Excel
                            Excel.Range cell = worksheet.Cells[_startCellIndex, 2];
                            cell.Select();
                            worksheet.Paste();
                            Thread.Sleep(10);

                            Excel.Range cellinputClass = worksheet.Cells[_startCellIndex, 1];
                            Excel.Range cellOutputClass = worksheet.Cells[_startCellIndex, 7];


                            cellinputClass.Value = customCodeComparer.InputFileName;
                            cellOutputClass.Value = customCodeComparer.OutoutFileName;
                            // Remove processed item from queue
                            _startCellIndex = FindACellLastRowIndes();
                            if (_startCellIndex <= 0)
                            {
                                Debug.WriteLine($"start Index is under zero");
                            }
                            else
                            {
                                Debug.WriteLine($"start Index is {_startCellIndex}");
                            }


                        }

                    }
                    //_fileWrittenEvent.Set();
                    //_fileSemaphore.Release();

                }
                catch (Exception ex)
                {
                    ICsvHelper csvHelper = new CsvHelper();
                    csvHelper.ExcepCOMExceptionToCsv(_filePath+".csv", new string[] { customCodeComparer.InputFileName, customCodeComparer.OutoutFileName });
                    Debug.WriteLine($"File read error: {ex.Message} Error Input : {customCodeComparer.InputFileName} , {customCodeComparer.OutoutFileName}");
                }
                finally
                {

                }
            }


        }
        private void OnFileWatchChanged(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(_resultFilePath))
            {
                try
                {
                    CustomCodeComparer customCodeComparer = null;
                    if (_queue.TryDequeue(out customCodeComparer) == true)
                    {
                        _fileSemaphore.Wait();
                        StringBuilder htmlContent = new StringBuilder();
                        string[] excludeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "Left file:", "Right file:", "Mode:&nbsp; Differences &nbsp;", "Text Compare", "Produced:" };
                        //string[] strs = File.ReadAllLines(_resultFilePath);

                        //using (StreamReader reader = new StreamReader(_resultFilePath))
                        //{
                        //    string line;
                        //    while ((line = reader.ReadLine()) != null)
                        //    {
                        //        if (!excludeStrs.Any(excludeStr => line.Contains(excludeStr)))
                        //        {
                        //            htmlContent.Append(line);
                        //        }
                        //        //htmlContent.Append(line);
                        //    }
                        //}
                        var strs = SafeReadFromFile(_resultFilePath);
                        foreach (string str in strs)
                        {
                            if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
                            {
                                htmlContent.Append(str);
                            }
                        }
                        // Copy HTML data to                        
                        ClipboardService.SetText(htmlContent.ToString());
                       

                        // Paste content to Excel
                        Excel.Range cell = worksheet.Cells[_startCellIndex, 2];
                        cell.Select();                        
                        worksheet.Paste();
                        Thread.Sleep(50);
                        Excel.Range cellinputClass = worksheet.Cells[_startCellIndex, 1];
                        Excel.Range cellOutputClass = worksheet.Cells[_startCellIndex, 7];


                        cellinputClass.Value = customCodeComparer.InputFileName;
                        cellOutputClass.Value = customCodeComparer.OutoutFileName;
                        // Remove processed item from queue
                        
                        _startCellIndex = FindACellLastRowIndes();
                        Debug.WriteLine($"input file: {customCodeComparer.InputFileName}");
                        Debug.WriteLine($"output file: {customCodeComparer.OutoutFileName}");

                        if (_startCellIndex <= 0)
                        {
                            Debug.WriteLine($"start Index is under zero");
                        }
                        else
                        {
                            Debug.WriteLine($"start Index is {_startCellIndex}");
                        }
                        //_fileWrittenEvent.Set();
                        _fileSemaphore.Release();
                    }
                    

                }
                catch (IOException ex)
                {
                    Console.WriteLine($"File read error: {ex.Message}");
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    Console.WriteLine("COM 오류 발생: " + ex.Message);
                }
                finally
                {

                }
            }
        }

        private int FindACellLastRowIndes()
        {
            int lastRowA = GetLastUsedRow(worksheet, "B");
            int lastRowB = GetLastUsedRow(worksheet, "C");
            int lastRowD = GetLastUsedRow(worksheet, "E");
            int lastRowE = GetLastUsedRow(worksheet, "F");

            try
            {
                int lastRow = Math.Max(Math.Max(lastRowA, lastRowB), Math.Max(lastRowD, lastRowE));
                return lastRow;

            }
            catch (Exception)
            {

                return 1;
            }
        }
        private int GetLastUsedRow(Excel.Worksheet worksheet, string column)
        {
            Excel.Range columnRange = worksheet.Columns[column];

            Excel.Range lastCell = columnRange.Cells.Find("*", System.Reflection.Missing.Value,
                Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,
                Excel.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);

            return lastCell != null ? lastCell.Row : 1; // 셀이 없으면 기본적으로 1행 반환
        }
        private string[] SafeReadFromFile(string path)
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
                        while ((line = reader.ReadLine()) != null)
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
