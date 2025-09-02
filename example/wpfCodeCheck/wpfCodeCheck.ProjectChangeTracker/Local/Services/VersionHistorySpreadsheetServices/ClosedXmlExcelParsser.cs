//using ClosedXML.Excel;
//using System.IO;
//using wpfCodeCheck.Domain.Models;
//using wpfCodeCheck.Domain.Helpers;
//using wpfCodeCheck.Domain.Services.Interfaces;
//using wpfCodeCheck.ProjectChangeTracker.Local.Models;
//using System.Diagnostics;

//namespace wpfCodeCheck.ProjectChangeTracker.Local.Services
//{
//    public class ClosedXmlExcelParsser : IExcelPaser
//    {
//        private readonly ICsvHelper? _csvHelper;
//        private readonly IBaseService _baseService;
//        private string _filePath = string.Empty;
//        private List<FailClassAnalysisModel> _failClassAnalysisModels = new();

//        public ClosedXmlExcelParsser(ICsvHelper csvHelper, IBaseService baseService)
//        {
//            _csvHelper = csvHelper;
//            _baseService = baseService;
//        }

//        public async Task<bool> WriteExcelAync(FileTreeModel inputFile, FileTreeModel outputFile)
//        {
//            return await Task.Run(() =>
//            {
//                try
//                {
//                    if (_baseService.CompareResult == null)
//                    {
//                        Debug.WriteLine("Not Data");
//                        return false;
//                    }

//                    using var workbook = new XLWorkbook(_filePath);
//                    var worksheet = workbook.Worksheet(1);

//                    // 여기에 Excel 작업 로직 구현
//                    // 기존 Interop 로직을 ClosedXML로 변환 필요

//                    workbook.Save();
//                    return true;
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine($"ClosedXML Excel 처리 오류: {ex.Message}");
//                    return false;
//                }
//            });
//        }

//        public async Task<bool> WriteExcelAync(string fileFullName)
//        {
//            _filePath = fileFullName;
            
//            return await Task.Run(() =>
//            {
//                try
//                {
//                    if (_baseService.CompareResult == null)
//                    {
//                        Debug.WriteLine("Not Data");
//                        return false;
//                    }

//                    using var workbook = new XLWorkbook();
//                    var worksheet = workbook.Worksheets.Add("Sheet1");

//                    // 헤더 설정
//                    worksheet.Cell("A1").Value = "Index";
//                    worksheet.Cell("B1").Value = "Class Name";
//                    worksheet.Cell("C1").Value = "Input File";
//                    worksheet.Cell("D1").Value = "Output File";

//                    int rowIndex = 2;
//                    foreach (var result in _baseService.CompareResult)
//                    {
//                        worksheet.Cell($"A{rowIndex}").Value = rowIndex - 1;
//                        worksheet.Cell($"B{rowIndex}").Value = !string.IsNullOrEmpty(result.InputFileName) ? result.InputFileName : result.OutoutFileName;
//                        worksheet.Cell($"C{rowIndex}").Value = result.InputFilePath;
//                        worksheet.Cell($"D{rowIndex}").Value = result.OutoutFilePath;
//                        rowIndex++;
//                    }

//                    // 스타일 적용
//                    var headerRange = worksheet.Range("A1:D1");
//                    headerRange.Style.Font.Bold = true;
//                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

//                    workbook.SaveAs(_filePath);
//                    return true;
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine($"ClosedXML Excel 생성 오류: {ex.Message}");
//                    return false;
//                }
//            });
//        }
//    }
//}