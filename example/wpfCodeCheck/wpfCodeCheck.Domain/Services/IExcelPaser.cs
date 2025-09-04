using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services
{
    public interface IExcelPaserService : IDisposable
    {
        //void SetExportFilePath(string exportFilePath);
        //Task<bool> WriteExcelAync(string compareResultFilePath);
        //Task<bool> WriteExcelAync(FileTreeModel inputFile, FileTreeModel outputFile);

        /// Excel ���� ���� (Workbook ����/�ű�)
        Task StartSessionAsync(string excelFilePath);

        /// �� ��� ������ �о� Excel�� ��� (���ø� ���� ������ ó��)
        Task WriteExcelAsync(string compareResultFilePath, CompareEntity compare);

        /// ����/�ݱ� (���� ����)
        Task SaveAndCloseAsync();

        void SetStatmentReportInformation(string summery, string documentNumber, EProjectType type);

    }
}