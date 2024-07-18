using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.ConfigurationChange.Local.Services
{
    /// <summary>
    /// 
    /// </summary>
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
        void SetExcelDate(CodeCompareResultModel dataList);
        Task WriteExcel();
    }

}
