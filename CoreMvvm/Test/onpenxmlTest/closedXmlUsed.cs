using ClosedXML.Excel;
using CompareEngine;

namespace onpenxmlTest
{
    internal class ClosedXmlUsed : IExcelPaser
    {
        private readonly string _filePath = string.Empty;
        private readonly string _sheetName = "4.소스코드";
        private CompareResult _dataList;
        private int _startRowIndex;
        private int _startCellIndex;

        public ClosedXmlUsed(string filePath)
        {
            _filePath  = filePath;
        }
        public void SetExcelDate(CompareResult dataList)
        {
            this._dataList = dataList;
        }
        public void SetStartCell(int rowIndex, int columnIndex)
        {
            _startRowIndex = rowIndex;
            _startCellIndex = columnIndex;
        }
        public void SetStartCellName(string cellName)
        {
            var collumn = cellName.TakeWhile(c => !char.IsDigit(c)).First();
            var row = int.Parse(new string(cellName.SkipWhile(c => !char.IsDigit(c)).ToArray()));
            _startCellIndex = collumn - 'A' + 1;            
            _startRowIndex = row;
        }
        public Task WriteExcel()
        {
            return Task.Run(() =>
            {
                if (_dataList is null)
                {
                    Console.WriteLine("Not Data");
                    return;
                }
                using var wb = new XLWorkbook(_filePath);
                var ws = wb.Worksheet(_sheetName);

                if (ws != null)
                {
                    var lastCell = ws.Column(4).LastCellUsed();
                    _startCellIndex = lastCell.Address.ColumnNumber + 1;
                    _startRowIndex = lastCell.Address.RowNumber + 1;
                    int mergeStartRow = _startRowIndex;
                    foreach (var data in _dataList.CompareResultSpans)
                    {
                        
                        switch (data.Status)
                        {
                            case CompareResultSpanStatus.DeleteSource:
                                for (int i = 0; i < data.Length; i++)
                                {
                                    
                                    var diffColor = XLColor.FromArgb(255, 227, 227);
                                    var inputDelteCodeIndex = (data.SourceIndex + i + 1).ToString();
                                    var inputDeleteCodeLine = _dataList.InputCompareText.GetByIndex(data.SourceIndex + i).Line;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_LINE)).Value = inputDelteCodeIndex;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).Value = inputDeleteCodeLine;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).Style.Fill.BackgroundColor = diffColor;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).GetRichText().SetFontColor(XLColor.Red);

                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_LINE)).Value = "";
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).Value = "";
                                    _startRowIndex++;
                                }
                                break;
                            case CompareResultSpanStatus.AddDestination:
                                for (int i = 0; i < data.Length; i++)
                                {                                    
                                    var diffColor = XLColor.FromArgb(255, 227, 227);
                                    var outputAddCodeIndex = (data.DestinationIndex + i + 1).ToString();
                                    var outputAddCodeLine = _dataList.OutputCompareText.GetByIndex(data.DestinationIndex + i).Line;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_LINE)).Value = "";
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).Value = "";

                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_LINE)).Value = outputAddCodeIndex;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).Value = outputAddCodeLine;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).Style.Fill.BackgroundColor = diffColor;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).GetRichText().SetFontColor(XLColor.Red);
                                    _startRowIndex++;
                                }
                                break;
                            case CompareResultSpanStatus.Replace:
                                for (int i = 0; i < data.Length; i++)
                                {                                    
                                    var diffColor = XLColor.FromArgb(255, 227, 227);
                                    var inputDelteCodeIndex = (data.SourceIndex + i + 1).ToString();
                                    var inputDeleteCodeLine = _dataList.InputCompareText.GetByIndex(data.SourceIndex + i).Line;
                                    var outputAddCodeIndex = (data.DestinationIndex + i + 1).ToString();
                                    var outputAddCodeLine = _dataList.OutputCompareText.GetByIndex(data.DestinationIndex + i).Line;

                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_LINE)).Value = inputDelteCodeIndex;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).Value = inputDeleteCodeLine;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).Style.Fill.BackgroundColor = diffColor;

                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_LINE)).Value = outputAddCodeIndex;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).Value = outputAddCodeLine;
                                    ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).Style.Fill.BackgroundColor = diffColor;
                                    InputOutputCodeCompaer(ws, inputDeleteCodeLine, outputAddCodeLine);
                                    _startRowIndex++;
                                }
                                break;
                        }
                     
                    }
                    int mergeEndRow = _startRowIndex;
                    
                    IXLCell xLCell1 = ws.Cell(mergeStartRow , 4);
                    IXLCell xLCell2 = ws.Cell(mergeEndRow -1 , 4);
                    ws.Range(xLCell1, xLCell2).Merge();
                    
                    wb.SaveAs(_filePath);
                }
            });
            
        }
        private void InputOutputCodeCompaer(IXLWorksheet ws ,string inputvalue , string outputvalue)
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
                            ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);                         
                            ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                        }
                    }
                    else if (i < inputvalue.Length)
                    {
                        ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.INPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                    }
                    else if (i < outputvalue.Length)
                    {
                        ws.Cell(_startRowIndex, (int)(_startCellIndex + ECELL.OUTPUT_CODE)).GetRichText().Substring(i, 1).SetFontColor(XLColor.Red);
                    }
                }
            }
        }
        public void Parsing(string filePath)
        {
            using var wb = new XLWorkbook(filePath);
            var ws = wb.Worksheet("4.소스코드");
            ws.Cell(454, 6).Value = "Test";
            ws.Cell(454, 8).Value = "Test2";
            var diffColor = XLColor.FromArgb(255, 227, 227);
            var sameColor = XLColor.FromArgb(238, 238, 255);
            ws.Cell(454, 6).Style.Fill.BackgroundColor = diffColor;
            ws.Cell(454, 8).Style.Fill.BackgroundColor = sameColor;

            ws.Cell(454, 6).GetRichText().Substring(0, 2).SetFontColor(XLColor.White);
            wb.SaveAs(filePath);
        }

      
    }
}
