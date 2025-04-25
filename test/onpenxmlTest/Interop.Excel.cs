
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TextCopy;
using Excel = Microsoft.Office.Interop.Excel;

namespace onpenxmlTest
{
    internal class Interop
    {
        Excel.Application excelApp = null;
        Excel.Workbook workbook = null;
        Excel.Worksheet worksheet = null;

        public Interop()
        {
            try
            {
                // Excel 애플리케이션 생성
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                // 새 워크북 추가
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Worksheets[1];

                string resultFilePath = @"D:\Temp\result.txt";
                StringBuilder htmlContent2 = new();
                if (File.Exists(resultFilePath))
                {
                    string[] exculudeStrs = { "<tr class=\"SectionGap\"><td colspan=\"5\">&nbsp;</td></tr>", "<br>", "Left file:", "Right file:" 
                    ,"Mode:&nbsp; Differences &nbsp;","Text Compare","Produced:"};
                    string[] strs = File.ReadAllLines(resultFilePath);
                    
                    foreach (string str in strs)
                    {
                        if (exculudeStrs.Any(excludeStr => str.Contains(excludeStr)) == false)
                        {
                            htmlContent2.Append(str);                            
                        }
                    }
                    
                }
                // HTML 형식의 데이터
                

                // 클립보드에 HTML 데이터 복사
                ClipboardService.SetText(htmlContent2.ToString());

                // 셀에 붙여넣기
                Excel.Range cell = worksheet.Cells[1, 1];
                cell.Select();
                worksheet.Paste();
                
                Excel.Range columnRange = worksheet.Columns["A"];

                // 지정된 열에서 마지막 사용된 행 찾기
                int lastUsedRow = columnRange.Cells.Find("*", System.Reflection.Missing.Value,
                    Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,
                    Excel.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

                Console.WriteLine("A열의 마지막 사용된 행: " + lastUsedRow);

                // 엑셀 파일 저장 (경로는 원하는 경로로 수정)
                workbook.SaveAs(@"D:\Temp\output.xlsx");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                // COM 개체 해제
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }

                // 가비지 컬렉션 강제 호출
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
          

         
        }
    }
}
