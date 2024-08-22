using ClosedXML.Excel;
using CompareEngine;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Services;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services
{
    public class ClosedXmlUsedExcelParser : IExcelPaser
    {
        private string _filePath = string.Empty;
        private readonly string _sheetName = "4.소스코드";
        private readonly IBaseService<CustomCodeComparer> _baseService;
        private int _startRowIndex;
        //private int _startCellIndex;
        public ClosedXmlUsedExcelParser(IBaseService<CustomCodeComparer> baseService)
        {
            _baseService = baseService;
        }   
        public void SetFilePath(string FilePath)
        {
            _filePath = FilePath; 
        }

        public async Task WriteExcelAync()
        {
            await Task.Run(() =>
            {
                if (_baseService.CompareResult is null)
                {
                    Console.WriteLine("Not Data");
                    return;
                }
                using var wb = new XLWorkbook(_filePath);
                var ws = wb.Worksheet(_sheetName);

                if (ws != null)
                {
                    var lastCell = ws.Column(4).LastCellUsed();
                    //_startCellIndex = lastCell.Address.ColumnNumber + 1;
                    _startRowIndex = lastCell.Address.RowNumber + 1;

                    foreach (var project in _baseService.CompareResult.CompareResults)
                    {

                        int mergeStartRow = _startRowIndex;
                        foreach (var data in project.CompareResultSpans)
                        {
                            switch (data.Status)
                            {
                                case CompareResultSpanStatus.DeleteSource:
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        var diffColor = XLColor.FromArgb(255, 227, 227);
                                        var inputDelteCodeIndex = (data.SourceIndex + i + 1).ToString();
                                        var inputDeleteCodeLine = project.InputCompareText.GetByIndex(data.SourceIndex + i).Line;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_LINE)).Value = inputDelteCodeIndex;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).Value = inputDeleteCodeLine;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).Style.Fill.BackgroundColor = diffColor;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).GetRichText().SetFontColor(XLColor.Red);

                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_LINE)).Value = "";
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).Value = "";
                                        _startRowIndex++;
                                    }
                                    break;
                                case CompareResultSpanStatus.AddDestination:
                                    for (int i = 0; i < data.Length; i++)
                                    {

                                        var diffColor = XLColor.FromArgb(255, 227, 227);
                                        var outputAddCodeIndex = (data.DestinationIndex + i + 1).ToString();
                                        var outputAddCodeLine = project.OutputCompareText.GetByIndex(data.DestinationIndex + i).Line;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_LINE)).Value = "";
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).Value = "";

                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_LINE)).Value = outputAddCodeIndex;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).Value = outputAddCodeLine;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).Style.Fill.BackgroundColor = diffColor;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).GetRichText().SetFontColor(XLColor.Red);
                                        _startRowIndex++;
                                    }
                                    break;
                                case CompareResultSpanStatus.Replace:
                                    for (int i = 0; i < data.Length; i++)
                                    {

                                        var diffColor = XLColor.FromArgb(255, 227, 227);
                                        var inputDelteCodeIndex = (data.SourceIndex + i + 1).ToString();
                                        var inputDeleteCodeLine = project.InputCompareText.GetByIndex(data.SourceIndex + i).Line;
                                        var outputAddCodeIndex = (data.DestinationIndex + i + 1).ToString();
                                        var outputAddCodeLine = project.OutputCompareText.GetByIndex(data.DestinationIndex + i).Line;

                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_LINE)).Value = inputDelteCodeIndex;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).Value = inputDeleteCodeLine;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).Style.Fill.BackgroundColor = diffColor;

                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_LINE)).Value = outputAddCodeIndex;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).Value = outputAddCodeLine;
                                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).Style.Fill.BackgroundColor = diffColor;
                                        InputOutputCodeCompaer(ws, inputDeleteCodeLine, outputAddCodeLine);
                                        _startRowIndex++;
                                    }
                                    break;
                            }
                        }
                        int mergeEndRow = _startRowIndex;
                        IXLCell mergeCellStart = ws.Cell(mergeStartRow, (int)ECELL.COL_CLASSNAME);
                        IXLCell mergeCellEnd = ws.Cell(mergeEndRow - 1, (int)ECELL.COL_CLASSNAME);
                        IXLCell mergeSummeryCellStart = ws.Cell(mergeStartRow, (int)ECELL.COL_SUMMARY_CELL);
                        IXLCell mergeSummeryCellEnd = ws.Cell(mergeEndRow - 1, (int)ECELL.COL_SUMMARY_CELL);
                        ws.Range(mergeCellStart, mergeCellEnd).Merge();
                        ws.Range(mergeSummeryCellStart, mergeSummeryCellEnd).Merge();
                        ws.Cell(mergeStartRow, (int)ECELL.COL_CLASSNAME).Value = project.InputFileName == string.Empty ? project.OutoutFileName : project.InputFileName;
                        ws.Cell(mergeStartRow, (int)ECELL.COL_SUMMARY_CELL).Value = "o 기능개선\r\n : ICD v5.3a 적용";

                    }
                    wb.SaveAs(_filePath);
                }
            });

        }
        private void InputOutputCodeCompaer(IXLWorksheet ws, string inputvalue, string outputvalue)
        {
            int CountTotal;

            if (inputvalue.Length >= outputvalue.Length)
            {
                CountTotal = inputvalue.Length;
            }
            else
            {
                CountTotal = outputvalue.Length;
            }
            for (int i = 0; i < CountTotal; i++)
            {
                {
                    if (i < inputvalue.Length && i < outputvalue.Length)
                    {
                        if (inputvalue[i] != outputvalue[i])
                        {
                            ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                            ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                        }
                    }
                    else if (i < inputvalue.Length)
                    {
                        ws.Cell(_startRowIndex, (int)(ECELL.COL_INPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                    }
                    else if (i < outputvalue.Length)
                    {
                        ws.Cell(_startRowIndex, (int)(ECELL.COL_OUTPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                    }
                }
            }
        }
    }

}
