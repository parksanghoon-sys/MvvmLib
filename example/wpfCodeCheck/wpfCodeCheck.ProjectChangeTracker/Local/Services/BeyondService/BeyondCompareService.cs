using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services.BeyondService;

internal interface IComparisonService
{
    string CompareFiles(string inputFilePath, string outputFilePath);
}
internal class BeyondCompareService : IComparisonService
{
    private string _resultFilePath = @"D:\Temp\result.txt";
    internal string CompareFiles(string inputFilePath, string outpuFilePath)
    {
        string processResult = string.Empty;
        // 폴더인지 파일인지 검증
        if (Directory.Exists(inputFilePath) || Directory.Exists(outpuFilePath))
        {
            Debug.WriteLine($"Error: Directory path provided to file comparison. Input: {inputFilePath}, Output: {outpuFilePath}");
            return "Error: Directory path provided to file comparison";
        }

        // 파일 존재 여부 확인
        if (!File.Exists(inputFilePath) && !File.Exists(outpuFilePath))
        {
            Debug.WriteLine($"Error: Neither input nor output file exists. Input: {inputFilePath}, Output: {outpuFilePath}");
            return "Error: Files do not exist";
        }
        try
        {

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Beyond Compare execution error: {ex.Message}");
            processResult = $"Beyond Compare execution error: {ex.Message}";
        }
        return true;
    }
}
