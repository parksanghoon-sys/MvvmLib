using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.LogService;
using wpfCodeCheck.ProjectChangeTracker.Local.Models;
using wpfCodeCheck.ProjectChangeTracker.Local.Services.BeyondService;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services;

public interface IExcelProcessManager
{
    Task<bool> RunAsync(DetailedStatementDto compareEntities, string excelFilePath);
    event Action<int, string>? ProgressChanged;
}
public class ExcelProcessManager : IExcelProcessManager
{
    private readonly IExcelPaserService _excelParserService;
    private readonly IComparisonService _comparisonService;
    private readonly IDialogService _dialogService;
    private readonly ILoggerService _loggerService;

    /// <summary>
    /// 디렉토리 스캔 진행률 이벤트
    /// </summary>
    public event Action<int, string>? ProgressChanged;
    public ExcelProcessManager(IExcelPaserService excelPaserService,
        IComparisonService comparisonService,
        IDialogService dialogService,
        ILoggerService loggerService)
    {
        _excelParserService = excelPaserService;
        _comparisonService = comparisonService;
        _dialogService = dialogService;
        _loggerService = loggerService;
    }
    public async Task<bool> RunAsync(DetailedStatementDto compareEntities, string excelFilePath)
    {
        var list = compareEntities.CompareResult.ToList();
        int total = list.Count;
        int current = 0;

        await _excelParserService.StartSessionAsync(excelFilePath);

        _excelParserService.SetStatmentReportInformation(compareEntities.Summery, compareEntities.DocumentNumber, compareEntities.ProjectType);
        _loggerService.Info("Excel 작업 시작");
        try
        {
            foreach (var compare in list)
            {
                string resultPath = await _comparisonService.CompareFiles(compare.InputFilePath, compare.OutoutFilePath);
                if (File.Exists(resultPath))
                {                    
                    await _excelParserService.WriteExcelAsync(resultPath, compare);
                }

                current++;
                ReportProgress(current * 100 / Math.Max(1, total), $"Processed {current}/{total}");
            }

            await _excelParserService.SaveAndCloseAsync();
            _loggerService.Info("Excel 작업 완료");
            return true;
        }
        catch (Exception ex)
        {
            ReportProgress(0, $"Error: {ex.Message}");
            _loggerService.Error($"{ex.Message}");
            return false;
        }
        finally
        {
            _excelParserService.Dispose();
        }
    }

    protected virtual void ReportProgress(int percentage, string message)
    {
        int scanProgress = Math.Max(0, Math.Min(100, percentage));
        if (_dialogService.Activate(typeof(LoadingDialogView)))
            WeakReferenceMessenger.Default.Send<(int, string), LoadingDialogView>(new(scanProgress, message));
    }
}
