using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TextCopy;
using wpfCodeCheck.Domain.Datas;
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

                    _watcher = new FileSystemWatcher(Path.GetDirectoryName(_resultFilePath));
                    _watcher.Filter = Path.GetFileName(_resultFilePath);
                    _watcher.NotifyFilter = NotifyFilters.LastWrite;
                    _watcher.Changed += OnFileWatchChanged;
                    _watcher.EnableRaisingEvents = true;

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
                        _fileWrittenEvent.WaitOne();
                        //StringBuilder htmlContent = new();
                        //if (File.Exists(_resultFilePath))
                        //{
                        //    string[] exculudeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "Left file:", "Right file:"
                        //                                ,"Mode:&nbsp; Differences &nbsp;","Text Compare","Produced:"};
                        //    string[] strs = File.ReadAllLines(_resultFilePath);

                        //    foreach (string str in strs)
                        //    {
                        //        if (exculudeStrs.Any(excludeStr => str.Contains(excludeStr)) == false)
                        //        {
                        //            htmlContent.Append(str);
                        //        }
                        //    }
                        //    // 클립보드에 HTML 데이터 복사
                        //    ClipboardService.SetText(htmlContent.ToString());

                        //    // 셀에 붙여넣기
                        //    Excel.Range cell = worksheet.Cells[_startCellIndex, 1];
                        //    cell.Select();
                        //    worksheet.Paste();                            
                        //}             

                    }
                    // Wait until all files are processed
                    while (!_queue.IsEmpty)
                    {
                        OnFileWatchChanged(null, null);
                        await Task.Delay(100); // Polling interval
                    }

                    workbook.SaveAs(_filePath);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
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

        private void OnFileWatchChanged(object sender, FileSystemEventArgs e)
        {
            _fileSemaphore.Wait();

            if (File.Exists(_resultFilePath))
            {
                try
                {
                    CustomCodeComparer customCodeComparer = null;
                    if (_queue.TryDequeue(out customCodeComparer) == true)
                    {
                        StringBuilder htmlContent = new StringBuilder();
                        string[] excludeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "Left file:", "Right file:", "Mode:&nbsp; Differences &nbsp;", "Text Compare", "Produced:" };
                        string[] strs = File.ReadAllLines(_resultFilePath);

                        foreach (string str in strs)
                        {
                            if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
                            {
                                htmlContent.Append(str);
                            }
                            //htmlContent.Append(str);
                        }

                        // Copy HTML data to clipboard
                        ClipboardService.SetText(htmlContent.ToString());

                        // Paste content to Excel
                        Excel.Range cell = worksheet.Cells[_startCellIndex, 2];
                        cell.Select();
                        worksheet.Paste();
                        Excel.Range cellinputClass = worksheet.Cells[_startCellIndex, 1];
                        Excel.Range cellOutputClass = worksheet.Cells[_startCellIndex, 7];


                        cellinputClass.Value = customCodeComparer.FileName;
                        cellOutputClass.Value = customCodeComparer.FileName;
                        // Remove processed item from queue
                        _startCellIndex = FindACellLastRowIndes();                        
                        
                    }
                    _fileWrittenEvent.Set();

                }
                catch (IOException ex)
                {
                    Console.WriteLine($"File read error: {ex.Message}");
                }
                finally
                {
                    _fileSemaphore.Release();
                }
            }
        }

        private int FindACellLastRowIndes()
        {
            Excel.Range columnRange = worksheet.Columns["B"];
            try
            {
                int lastUsedRow = columnRange.Cells.Find("*", System.Reflection.Missing.Value,
                            Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,
                                Excel.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;
                return lastUsedRow;

            }
            catch (Exception)
            {

                return 1;
            }
            

        }
    }
}
