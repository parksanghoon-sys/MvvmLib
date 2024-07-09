using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using CompareEngine;

namespace onpenxmlTest
{
    public enum ECELL
    {
        INPUT_LINE,
        INPUT_CODE,
        OUTPUT_LINE,
        OUTPUT_CODE,
    }
    public class OpenXmlUsed : IExcelPaser
    {
        private readonly string _filePath = string.Empty;
        private readonly string _hexColor = "FFFF1111";
        private readonly string _sheetName = "4.소스코드";
        private CompareResult _dataList;
        private string _startcell;
        private uint _startRowIndex;
        private string _startCellIndex;
        public OpenXmlUsed(string filePath)
        {
            _filePath = filePath;
        }
        public void SetExcelDate(CompareResult dataList)
        {
            this._dataList = dataList;
        }
        public void SetStartCellName(string cellName)
        {
            this._startcell = cellName;
            _startRowIndex = GetRowIndex(cellName);
            _startCellIndex = GetColumnName(cellName);
            var test = (char)(_startCellIndex[0] + 1);
            var test2 = _startCellIndex + 2;
        }
        public void Parsing(string filePath)
        {
            //WriteCellValue(filePath, "4.소스코드", "F454", "TEST");
        }
        public OpenXmlUsed SetValue(string value, string cell)
        {
            //WriteCellValue(_filePath, "4.소스코드", cell, value);
            return this;
        }

        public void WriteExcel()
        {
            if (_dataList is null)
            {
                Console.WriteLine("Not Data");
                return;
            }

            int startIndex = 1;
            int endIndex = 3;
            string hexColor = "FFFF1111";
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(_filePath, true))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = GetWorksheetPartByName(workbookPart, _sheetName);

                string originalText = string.Empty;
                if (worksheetPart != null)
                {
                    int addRow = 0;

                    foreach (var data in _dataList.CompareResultSpans)
                    {
                        switch (data.Status)
                        {
                            case CompareResultSpanStatus.DeleteSource:
                                for (int i = 0; i < data.Length; i++)
                                {
                                    Cell cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.INPUT_LINE) + (_startRowIndex + addRow).ToString()));
                                    var inputDelteCodeIndex = (data.SourceIndex + i + 1).ToString();
                                    cell.CellValue = new CellValue(inputDelteCodeIndex);

                                    cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.INPUT_CODE) + (_startRowIndex + addRow).ToString()));
                                    var inputDeleteCodeLine = _dataList.InputCompareText.GetByIndex(data.SourceIndex + i).Line;
                                    cell.CellValue = new CellValue(inputDeleteCodeLine);
                                    ApplyCellFill(workbookPart, cell, "FFFFE3E3");

                                    cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.OUTPUT_LINE) + (_startRowIndex + addRow).ToString()));
                                    cell.CellValue = new CellValue("");

                                    cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.OUTPUT_CODE) + (_startRowIndex + addRow).ToString()));
                                    cell.CellValue = new CellValue("");
                                    addRow++;

                                }
                                break;
                            case CompareResultSpanStatus.AddDestination:
                                for (int i = 0; i < data.Length; i++)
                                {
                                    Cell cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.INPUT_LINE) + (_startRowIndex + addRow).ToString()));
                                    cell.CellValue = new CellValue("");

                                    cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.INPUT_CODE) + (_startRowIndex + addRow).ToString()));
                                    cell.CellValue = new CellValue("");

                                    cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.OUTPUT_LINE) + (_startRowIndex + addRow).ToString()));
                                    var outputAddCodeIndex = (data.DestinationIndex + i + 1).ToString();
                                    cell.CellValue = new CellValue(outputAddCodeIndex);

                                    cell = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.OUTPUT_CODE) + (_startRowIndex + addRow).ToString()));
                                    var outputAddCodeLine = _dataList.OutputCompareText.GetByIndex(data.DestinationIndex + i).Line;
                                    cell.CellValue = new CellValue(outputAddCodeLine);
                                    ApplyCellFill(workbookPart, cell, "FFFFE3E3");
                                    addRow++;

                                }
                                break;
                            case CompareResultSpanStatus.Replace:
                                for (int i = 0; i < data.Length; i++)
                                {
                                    Cell cellInputLine = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.INPUT_LINE) + (_startRowIndex + addRow).ToString()));
                                    var inputDelteCodeIndex = (data.SourceIndex + i + 1).ToString();
                                    cellInputLine.CellValue = new CellValue(inputDelteCodeIndex);

                                    Cell cellInputCode = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.INPUT_CODE) + (_startRowIndex + addRow).ToString()));
                                    var inputDeleteCodeLine = _dataList.InputCompareText.GetByIndex(data.SourceIndex + i).Line;
                                    cellInputCode.CellValue = new CellValue(inputDeleteCodeLine);
                                    ApplyCellFill(workbookPart, cellInputCode, "FFFFE3E3");

                                    Cell cellOutputLine = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.OUTPUT_LINE) + (_startRowIndex + addRow).ToString()));
                                    var outputAddCodeIndex = (data.DestinationIndex + i + 1).ToString();
                                    cellOutputLine.CellValue = new CellValue(outputAddCodeIndex);

                                    Cell cellOutputCode = GetCell(worksheetPart.Worksheet, new string((char)(_startCellIndex[0] + ECELL.OUTPUT_CODE) + (_startRowIndex + addRow).ToString()));
                                    var outputAddCodeLine = _dataList.OutputCompareText.GetByIndex(data.DestinationIndex + i).Line;
                                    cellOutputCode.CellValue = new CellValue(outputAddCodeLine);
                                    ApplyCellFill(workbookPart, cellOutputCode, "FFFFE3E3");
                                    addRow++;
                                    InputOutputCodeCompaer(workbookPart, cellInputCode, cellOutputCode);
                                }
                                break;
                        }



                    }
                    //Cell cell1 = GetCell(worksheetPart.Worksheet, _startcell);


                    //cell1.CellValue = new CellValue("TEST");
                    //cell1.DataType = new EnumValue<CellValues>(CellValues.String);

                    //originalText = cell1.CellValue.Text;

                    //if (cell1.DataType != null && cell1.DataType.Value == CellValues.SharedString)
                    //{
                    //    SharedStringTablePart sstPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                    //    SharedStringItem ssi = sstPart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(cell1.CellValue.Text));
                    //    originalText = ssi.InnerText;

                    //    // Clear the existing runs in the shared string item
                    //    ssi.RemoveAllChildren<Run>();

                    //    // Add new runs with color formatting for the specified range
                    //    AddRun(ssi, originalText.Substring(0, startIndex), null); // Before colored text
                    //    AddRun(ssi, originalText.Substring(startIndex, endIndex - startIndex + 1), hexColor); // Colored text
                    //    AddRun(ssi, originalText.Substring(endIndex + 1), null); // After colored text

                    //    sstPart.SharedStringTable.Save();
                    //}
                    //else
                    //{
                    //    originalText = cell1.CellValue.Text;

                    //    // Create an inline string item to replace the existing cell value
                    //    cell1.DataType = new EnumValue<CellValues>(CellValues.InlineString);
                    //    cell1.CellValue = null;
                    //    InlineString inlineString = new InlineString();
                    //    cell1.Append(inlineString);

                    //    // Clear the existing runs in the inline string item
                    //    inlineString.RemoveAllChildren<Run>();

                    //    // Add new runs with color formatting for the specified range
                    //    AddRun(inlineString, originalText.Substring(0, startIndex), null); // Before colored text
                    //    AddRun(inlineString, originalText.Substring(startIndex, endIndex - startIndex + 1), hexColor); // Colored text
                    //    AddRun(inlineString, originalText.Substring(endIndex + 1), null); // After colored text
                    //}

                    //ApplyCellFill(workbookPart, cell1, "FFFFE3E3");
                    worksheetPart.Worksheet.Save();
                }
            }
        }
        private void InputOutputCodeCompaer(WorkbookPart ws, Cell inputvalue, Cell outputvalue)
        {
            string hexColor = "FFFF1111";

            int CountTotal;
            var inputText = inputvalue.CellValue.Text;
            var ouputText = outputvalue.CellValue.Text;

            inputvalue.DataType = new EnumValue<CellValues>(CellValues.InlineString);
            inputvalue.CellValue = null;
            InlineString inlineString = new InlineString();
            inputvalue.Append(inlineString);

            outputvalue.DataType = new EnumValue<CellValues>(CellValues.InlineString);
            outputvalue.CellValue = null;
            InlineString outlineString = new InlineString();
            outputvalue.Append(outlineString);

            inlineString.RemoveAllChildren<Run>();
            outputvalue.RemoveAllChildren<Run>();

            

            if (inputText.Length >= ouputText.Length)
            {
                CountTotal = inputText.Length;
            }
            else
            {
                CountTotal = ouputText.Length;
            }
            for (int i = 0; i < CountTotal; i++)
            {
                {
                    if (i < inputText.Length  && i < ouputText.Length)
                    {
                        if (inputText[i] != ouputText[i])
                        {                            
                            AddRun(inlineString, inputText.Substring(i,1), hexColor); // Colored text                            
                            AddRun(inlineString, ouputText.Substring(i, 1), hexColor); // Colored text
                        }
                    }
                    else if (i < inputText.Length)
                    {
                        AddRun(inlineString, inputText.Substring(i, 1), hexColor); // Colored text     
                    }
                    else if (i < ouputText.Length)
                    {
                        AddRun(inlineString, ouputText.Substring(i, 1), hexColor); // Colored text
                    }
                }
            }
        }
        private static void AddRun(OpenXmlElement ssi, string text, string hexColor)
        {
            Run run = new Run();
            Text t = new Text(text) { Space = SpaceProcessingModeValues.Preserve };
            run.Append(t);

            if (hexColor != null)
            {
                RunProperties rp = new RunProperties();
                Color color = new Color() { Rgb = new HexBinaryValue() { Value = hexColor } };
                rp.Append(color);
                run.PrependChild(rp);
            }

            ssi.Append(run);
        }
        private WorksheetPart GetWorksheetPartByName(WorkbookPart workbookPart, string sheetName)
        {
            Sheet sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().FirstOrDefault(s => s.Name == sheetName);
            if (sheet == null)
                return null;

            return (WorksheetPart)workbookPart.GetPartById(sheet.Id);
        }

        private Cell GetCell(Worksheet worksheet, string cellReference)
        {
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string columnName = GetColumnName(cellReference);
            uint rowIndex = GetRowIndex(cellReference);

            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            Cell cell = row.Elements<Cell>().FirstOrDefault(c => string.Compare(c.CellReference.Value, cellReference, true) == 0);
            if (cell == null)
            {
                cell = new Cell() { CellReference = cellReference };
                row.Append(cell);
            }

            return cell;
        }
        private Cell GetCell(Worksheet worksheet, uint rowIndex, uint cellIndex)
        {
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            Cell cell = row.Elements<Cell>().FirstOrDefault(c => string.Compare(c.CellReference.Value, cellIndex.ToString(), true) == 0);
            if (cell == null)
            {
                cell = new Cell() { CellReference = cellIndex.ToString() };
                row.Append(cell);
            }

            return cell;
        }
        private string GetColumnName(string cellReference)
        {
            return new string(cellReference.TakeWhile(c => !char.IsDigit(c)).ToArray());
        }

        private uint GetRowIndex(string cellReference)
        {
            return uint.Parse(new string(cellReference.SkipWhile(c => !char.IsDigit(c)).ToArray()));
        }
        private void ApplyCellFill(WorkbookPart workbookPart, Cell cell, string hexColor)
        {
            Stylesheet stylesheet = workbookPart.WorkbookStylesPart.Stylesheet;
            uint fillId = CreateCellFill(stylesheet, hexColor);

            if (cell.StyleIndex == null)
            {
                cell.StyleIndex = CreateCellFormat(stylesheet, fillId);
            }
            else
            {
                uint styleIndex = cell.StyleIndex.Value;
                CellFormat cellFormat = stylesheet.CellFormats.Elements<CellFormat>().ElementAt((int)styleIndex);
                cellFormat.FillId = fillId;
                cellFormat.Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Top };
            }

            stylesheet.Save();
        }

        private uint CreateCellFill(Stylesheet stylesheet, string hexColor)
        {
            Fill fill = new Fill(
                new PatternFill(
                    new ForegroundColor { Rgb = new HexBinaryValue { Value = hexColor } }
                )
                { PatternType = PatternValues.Solid }
            );
            var algnment = new Alignment
            {
                Vertical = VerticalAlignmentValues.Top
            };
            stylesheet.Fills.Append(fill);
            return ((uint)stylesheet.Fills.Count() - 1);
        }

        private uint CreateCellFormat(Stylesheet stylesheet, uint fillId)
        {
            CellFormat cellFormat = new CellFormat { FillId = fillId, ApplyFill = true };
            stylesheet.CellFormats.Append(cellFormat);
            return (uint)(stylesheet.CellFormats.Count() - 1);
        }

        private void ApplyVerticalAlignment(Cell cell, WorkbookPart worksheetPart, VerticalAlignmentValues alignment)
        {
            CellFormat cellFormat = new CellFormat
            {
                Alignment = new Alignment
                {
                    Vertical = alignment
                },
                ApplyAlignment = true
            };

            Stylesheet stylesheet = worksheetPart.WorkbookStylesPart.Stylesheet;

            stylesheet.CellFormats.Append(cellFormat);
            stylesheet.CellFormats.Count = (uint)stylesheet.CellFormats.ChildElements.Count;
            stylesheet.Save();

            cell.StyleIndex = (uint)stylesheet.CellFormats.ChildElements.Count - 1;
        }

    }
}
