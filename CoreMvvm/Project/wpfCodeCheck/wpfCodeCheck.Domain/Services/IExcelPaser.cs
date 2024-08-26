using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public enum ECELL
    {
        COL_INDEX = 1,
        COL_DATA_NAME,
        COL_DATASHEET_NUMBER,
        COL_CLASSNAME,
        COL_INPUT_LINE,
        COL_INPUT_CODE,
        COL_OUTPUT_LINE,
        COL_OUTPUT_CODE,         
        COL_SUMMARY_CELL,
    }
    public interface IExcelPaser
    {
        void SetFilePath(string FilePath);
        Task<bool>  WriteExcelAync();
    }
}
