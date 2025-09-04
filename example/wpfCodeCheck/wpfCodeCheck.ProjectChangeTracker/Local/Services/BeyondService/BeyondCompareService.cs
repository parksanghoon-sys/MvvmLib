using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.Services.Interfaces;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Services.BeyondService;

public interface IComparisonService
{
    Task<string> CompareFiles(string inputFilePath, string outputFilePath);
}
public class BeyondCompareService : IComparisonService
{    
    private readonly string _resultFilePath;
    public BeyondCompareService(ISettingService settingService)
    { 
        _resultFilePath = settingService.GeneralSetting!.ExportBeyondCompareFilePath == string.Empty 
            ? @"D:\Temp\result.txt" : settingService.GeneralSetting!.ExportBeyondCompareFilePath;        
    }


    public async Task<string> CompareFiles(string inputFilePath, string outputFilePath)
    {
        string processResult = string.Empty;

        if (Directory.Exists(inputFilePath) || Directory.Exists(outputFilePath))
            return "Error: Directory path provided to file comparison";

        if (!File.Exists(inputFilePath) && !File.Exists(outputFilePath))
            return "Error: Files do not exist";

        if (!File.Exists(_resultFilePath))
            File.Create(_resultFilePath).Close();

        try
        {
            var startInfo = new ProcessStartInfo(@"C:\Program Files\Beyond Compare 4\BComp.com")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                Arguments = $"""
                        "@{DirectoryHelper.GetLocalSettingDirectory()}\beyondCli.txt" "{inputFilePath}" "{outputFilePath}" "{_resultFilePath}"
                        """,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };

            process.Start();

            // 출력 비동기 읽기
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(); 

            string output = await outputTask;
            processResult = await errorTask;

            if (!string.IsNullOrEmpty(processResult))
                Debug.WriteLine($"Error: {processResult}");
        }
        catch (Exception ex)
        {
            processResult = $"Beyond Compare execution error: {ex.Message}";
            Debug.WriteLine(processResult);
            return processResult;
        }

        return _resultFilePath;
    }

    async Task<string>  IComparisonService.CompareFiles(string inputFilePath, string outputFilePath)
    {
        return await CompareFiles(inputFilePath, outputFilePath);
    }
}
