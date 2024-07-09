using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.ConfigurationChange.Local.Services
{
    public enum ECELL
    {
        INPUT_LINE,
        INPUT_CODE,
        OUTPUT_LINE,
        OUTPUT_CODE,
    }
    public interface IExcelPaser
    {
        void SetExcelDate(CodeCompareModel dataList);
        Task WriteExcel();
    }

}
