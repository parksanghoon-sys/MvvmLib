using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TextCopy;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Local.Helpers;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.LogService;
using wpfCodeCheck.ProjectChangeTracker.Local.Helpers;
using wpfCodeCheck.ProjectChangeTracker.Local.Models;
using wpfCodeCheck.ProjectChangeTracker.Local.Services.BeyondService;
using Excel = Microsoft.Office.Interop.Excel;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services.ExcelService;

public class InteropExcelParsserService : IExcelPaserService
{
    private List<FailClassAnalysisModel> _failClassAnalysisModels = new();
    private string _exportFilePath;
    private readonly ConcurrentQueue<CompareEntity> _queue = new();
   
    private readonly ICsvHelper _csvHelper;    
    private readonly IComparisonService _comparisonService;
    private readonly ILoggerService _loggerService;
    private readonly StaDispatcher _sta = new("Excel-STA");
    private Excel.Application? _excelApp;
    private Excel.Workbook? _workbook;
    private Excel.Worksheet? _worksheet;

    private int _excelIndex = 1;    
    private string? _excelFilePath;
    // ====== 유지 요청한 형태 (시그니처 동일) ======
    private Task<T> RunStaTaskAsync<T>(Func<T> action) => _sta.InvokeAsync(action);
    private Task RunStaTaskAsync(Action action) => _sta.InvokeAsync(action);
    public InteropExcelParsserService(ICsvHelper csvHelper,        
        IComparisonService comparisonService,
        ILoggerService loggerService)
    {
        _csvHelper = csvHelper;        
        _comparisonService = comparisonService;
        _loggerService = loggerService;
    }
    public async Task StartSessionAsync(string excelFilePath)
    {
        _excelFilePath = excelFilePath;

        await RunStaTaskAsync(() =>
        {
            _excelApp = new Excel.Application { Visible = false, DisplayAlerts = false };
            _workbook = File.Exists(excelFilePath)
                ? _excelApp.Workbooks.Open(excelFilePath)
                : _excelApp.Workbooks.Add();

            _worksheet = (Excel.Worksheet)_workbook!.Worksheets[1];            
        });
    }
    public async Task WriteExcelAsync(string compareResultFilePath, CompareEntity compare)
    {
        await RetryHelper.ExecuteAsync(async () =>
        {
            var html = await BuildHtmlAsync(compareResultFilePath);
            var excelRange = await PasteHtmlToExcelAsync(html);
            await PostProcessCellsAsync(compare, excelRange);
        },
        maxRetry: 3,
        delayMs: 500,
        onRetry: (ex, attempt) =>
        {
            _loggerService.Warn($"Excel Write Retry {attempt}: {ex.Message}, {compare.FileName}, fileIndex : {_excelIndex}");
        });
    }
    public async Task SaveAndCloseAsync()
    {
        await RunStaTaskAsync(() =>
        {
            if (_workbook != null)
            {
                if (!string.IsNullOrEmpty(_excelFilePath))
                    _workbook.SaveAs(_excelFilePath);
                else
                    _workbook.Save();
            }
        });
    }
    private async Task<string> BuildHtmlAsync(string resultFilePath)
    {
        StringBuilder htmlContent = new StringBuilder();
        string[] excludeStrs = 
            { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "File:", 
            "Left file:", "Right file:", "Mode:&nbsp; Differences &nbsp;", 
            "Text Compare", "Produced:" };

        var strs = await SafeReadFromFileAsync(resultFilePath);
        foreach (string str in strs)
        {
            if (str == "&nbsp; &nbsp;")
                continue;
            if (!excludeStrs.Any(excludeStr => str.Contains(excludeStr)))
            {
                htmlContent.Append(str);
            }      
        }
        return htmlContent.ToString();
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
    private async Task<(int StartRow, int LastCellIndex)> PasteHtmlToExcelAsync(string html)
    {        
        return await RunStaTaskAsync(() =>
        {
            int startRowIndex = FindWriteCellLastRowIndex(_worksheet!);
            // CF_HTML 로 넣어야 서식 유지됨

            ClipboardService.SetText(html);

            var cell = _worksheet!.Cells[startRowIndex, ECELL.COL_INPUT_LINE];
            cell.Select();
            
            _worksheet.Paste();
            Thread.Sleep(50);
            // 붙여넣기 완료 대기
            while (_excelApp!.CutCopyMode != 0)
                Thread.Sleep(50);
            // 여기서 끝났다는 것은 붙여넣기가 진짜 완료된 상태
            int lastRowIndex = FindWriteCellLastRowIndex(_worksheet!);

            return (startRowIndex, lastRowIndex);
        });

    }

    private Task PostProcessCellsAsync(CompareEntity compare, (int startIndex, int lastIndex) range)
    {
        return RunStaTaskAsync(() =>
        {            
            // (아래 ECELL.* 는 기존 enum/상수 사용)
            Excel.Range rgIndex = _worksheet!.Range[_worksheet.Cells[range.startIndex, ECELL.COL_INDEX],
                                                    _worksheet.Cells[range.lastIndex -1, ECELL.COL_INDEX]];
            Excel.Range rgDataName = _worksheet.Range[_worksheet.Cells[range.startIndex, ECELL.COL_DATA_NAME], 
                                                    _worksheet.Cells[range.lastIndex -1, ECELL.COL_DATA_NAME]];
            Excel.Range rgDatasheetNumber = _worksheet.Range[_worksheet.Cells[range.startIndex, ECELL.COL_DATASHEET_NUMBER], 
                                                            _worksheet.Cells[range.lastIndex -1, ECELL.COL_DATASHEET_NUMBER]];

            Excel.Range rgName = _worksheet.Range[_worksheet.Cells[range.startIndex, ECELL.COL_CLASSNAME], 
                                                            _worksheet.Cells[range.lastIndex -1, ECELL.COL_CLASSNAME]];
            Excel.Range rgSummery = _worksheet.Range[_worksheet.Cells[range.startIndex, ECELL.COL_SUMMARY_CELL + 1], 
                                                    _worksheet.Cells[range.lastIndex -1, ECELL.COL_SUMMARY_CELL + 1]];
            Excel.Range rgIssue = _worksheet.Range[_worksheet.Cells[range.startIndex, ECELL.COL_ISSUE + 1], 
                                                    _worksheet.Cells[range.lastIndex -1, ECELL.COL_ISSUE + 1]];
            Excel.Range rgCode = _worksheet.Range[_worksheet.Cells[range.startIndex, ECELL.COL_CODE + 1], 
                                                    _worksheet.Cells[range.lastIndex -1, ECELL.COL_CODE + 1]];

            rgIndex.Merge(); rgDataName.Merge(); rgDatasheetNumber.Merge();
            rgName.Merge(); rgSummery.Merge(); rgIssue.Merge(); rgCode.Merge();

            rgIndex.Value = _excelIndex.ToString();
            rgName.Value = string.IsNullOrEmpty(compare.InputFileName) ? compare.OutoutFileName : compare.InputFileName;
            rgDataName.Value =
                $"""
                {_projectType.ToString()}체계 MQ-105K
                주부조종사운용장치
                소프트웨어산출물명세서
                ({_documentNumber})
                """;
            rgDatasheetNumber.Value = $"{_documentNumber}";
            rgSummery.Value = _summery;
            rgIssue.Value = "영향없음";
            rgCode.Value = "A6";
          
            rgCode.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgCode.Borders.Weight = Excel.XlBorderWeight.xlThin;

            rgIndex.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgIndex.Borders.Weight = Excel.XlBorderWeight.xlThin;

            rgDataName.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgDataName.Borders.Weight = Excel.XlBorderWeight.xlThin;

            rgDatasheetNumber.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgDatasheetNumber.Borders.Weight = Excel.XlBorderWeight.xlThin;

            rgSummery.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgSummery.Borders.Weight = Excel.XlBorderWeight.xlThin;

            rgIssue.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgIssue.Borders.Weight = Excel.XlBorderWeight.xlThin;

            rgSummery.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rgSummery.Borders.Weight = Excel.XlBorderWeight.xlThin;
            // 테두리     

            // COM 해제
            Marshal.ReleaseComObject(rgIndex);
            Marshal.ReleaseComObject(rgDataName);
            Marshal.ReleaseComObject(rgDatasheetNumber);
            Marshal.ReleaseComObject(rgName);
            Marshal.ReleaseComObject(rgSummery);
            Marshal.ReleaseComObject(rgIssue);
            Marshal.ReleaseComObject(rgCode);

            _excelIndex++;
        });
    }

    // -------- Helpers --------

    private static int FindWriteCellLastRowIndex(Excel.Worksheet ws)
    {
        int lastRowA = GetLastUsedRow(ws, ECELL.COL_INPUT_LINE);
        int lastRowB = GetLastUsedRow(ws, ECELL.COL_INPUT_CODE);
        int lastRowD = GetLastUsedRow(ws, ECELL.COL_OUTPUT_LINE + 1);
        int lastRowE = GetLastUsedRow(ws, ECELL.COL_OUTPUT_CODE + 1);
        return Math.Max(Math.Max(lastRowA, lastRowB), Math.Max(lastRowD, lastRowE)) + 1;
    }

    private static int GetLastUsedRow(Excel.Worksheet ws, ECELL col)
    {
        Excel.Range colRange = (Excel.Range)ws.Columns[col];
        Excel.Range? last = colRange.Cells.Find("*", Type.Missing,
            Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlPart,
            Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
            false, Type.Missing, Type.Missing);
        return last != null ? last.Row : 1;
    }
    public void Dispose()
    {
        // Excel/COM 자원 정리는 생성 스레드(STA)에서
        RunStaTaskAsync(() =>
        {
            try
            {
                if (_worksheet != null) Marshal.ReleaseComObject(_worksheet);
                if (_workbook != null)
                {
                    _workbook.Close();
                    Marshal.ReleaseComObject(_workbook);
                }
                if (_excelApp != null)
                {
                    _excelApp.Quit();
                    Marshal.ReleaseComObject(_excelApp);
                }
            }
            finally
            {
                _worksheet = null;
                _workbook = null;
                _excelApp = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }).GetAwaiter().GetResult();

        _sta.Dispose();
    }
    private string _summery;
    private string _documentNumber;
    private EProjectType _projectType;

    public void SetStatmentReportInformation(string summery, string documentNumber, EProjectType type)
    {
        _summery = summery;
        _documentNumber = documentNumber;
        _projectType = type;
    }
}
