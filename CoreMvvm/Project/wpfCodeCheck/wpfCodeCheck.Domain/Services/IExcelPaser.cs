using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public enum ECELL
    {
        INPUT_LINE,
        INPUT_CODE,
        OUTPUT_LINE,
        OUTPUT_CODE,
        CLASS_CELL = 4,
        SUMMARY_CELL = 9,
    }
    public interface IExcelPaser
    {
        void SetFilePath(string FilePath);
        Task  WriteExcelAync();
    }
}
