namespace onpenxmlTest
{
    internal interface IExcelPaser
    {        
        void SetExcelDate(CompareResult dataList);
        Task WriteExcel();
        void SetStartCellName(string cellName);
    }
}
