using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services
{
    public interface IExcelPaserService : IDisposable
    {
        //void SetExportFilePath(string exportFilePath);
        //Task<bool> WriteExcelAync(string compareResultFilePath);
        //Task<bool> WriteExcelAync(FileTreeModel inputFile, FileTreeModel outputFile);

        /// Excel 세션 시작 (Workbook 열기/신규)
        Task StartSessionAsync(string excelFilePath);

        /// 비교 결과 파일을 읽어 Excel에 기록 (템플릿 순서 내에서 처리)
        Task WriteExcelAsync(string compareResultFilePath, CompareEntity compare);

        /// 저장/닫기 (세션 종료)
        Task SaveAndCloseAsync();

        void SetStatmentReportInformation(string summery, string documentNumber, EProjectType type);

    }
}