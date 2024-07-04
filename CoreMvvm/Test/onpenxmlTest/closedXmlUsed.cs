using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onpenxmlTest
{
    internal interface IExcelPaser
    {
        void Parsing(string filePath);
    }
    internal class ClosedXmlUsed : IExcelPaser
    {
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
