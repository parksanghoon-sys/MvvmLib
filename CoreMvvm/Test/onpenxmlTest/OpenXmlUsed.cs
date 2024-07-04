using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using ClosedXML.Excel;

namespace onpenxmlTest
{
    internal class OpenXmlUsed : IExcelPaser
    {
        public void Parsing(string filePath)
        {
            WriteCellValue(filePath, "4.소스코드", "F454", "TEST");
        }
        public void WriteCellValue(string filePath, string sheetName, string cellReference, string value)
        {
            int startIndex = 1;
            int endIndex = 3;
            string hexColor = "FFFF1111";
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, true))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = GetWorksheetPartByName(workbookPart, sheetName);

                string originalText = string.Empty;
                if (worksheetPart != null)
                {
                    Cell cell = GetCell(worksheetPart.Worksheet, cellReference);

                    cell.CellValue = new CellValue(value);
                    cell.DataType = new EnumValue<CellValues>(CellValues.String);

                    originalText = cell.CellValue.Text;
                    
                    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                    {
                        SharedStringTablePart sstPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        SharedStringItem ssi = sstPart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(cell.CellValue.Text));
                        originalText = ssi.InnerText;

                        // Clear the existing runs in the shared string item
                        ssi.RemoveAllChildren<Run>();

                        // Add new runs with color formatting for the specified range
                        AddRun(ssi, originalText.Substring(0, startIndex), null); // Before colored text
                        AddRun(ssi, originalText.Substring(startIndex, endIndex - startIndex + 1), hexColor); // Colored text
                        AddRun(ssi, originalText.Substring(endIndex + 1), null); // After colored text

                        sstPart.SharedStringTable.Save();
                    }
                    else
                    {
                        originalText = cell.CellValue.Text;

                        // Create an inline string item to replace the existing cell value
                        cell.DataType = new EnumValue<CellValues>(CellValues.InlineString);
                        cell.CellValue = null;
                        InlineString inlineString = new InlineString();
                        cell.Append(inlineString);

                        // Clear the existing runs in the inline string item
                        inlineString.RemoveAllChildren<Run>();

                        // Add new runs with color formatting for the specified range
                        AddRun(inlineString, originalText.Substring(0, startIndex), null); // Before colored text
                        AddRun(inlineString, originalText.Substring(startIndex, endIndex - startIndex + 1), hexColor); // Colored text
                        AddRun(inlineString, originalText.Substring(endIndex + 1), null); // After colored text
                    }

                    ApplyCellFill(workbookPart, cell, "FFFFE3E3");
                    //ApplyVerticalAlignment(cell, workbookPart, VerticalAlignmentValues.Top)
                    worksheetPart.Worksheet.Save();
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
                    new ForegroundColor { Rgb = new HexBinaryValue { Value = hexColor }}
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
            CellFormat cellFormat = new CellFormat { FillId = fillId, ApplyFill = true};
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
