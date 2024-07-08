using CodeCompareTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class CodeComparer
{
    public List<string> CompareFiles(string file1Path, string file2Path)
    {
        var differences = new List<string>();
        var lines1 = File.ReadAllLines(file1Path);
        var lines2 = File.ReadAllLines(file2Path);

        var methods1 = ExtractMethods(lines1);
        var methods2 = ExtractMethods(lines2);

        foreach (var method in methods1.Keys.Union(methods2.Keys))
        {
            if (!methods2.ContainsKey(method))
            {
                differences.Add($"Method only in file1: {method}");
                differences.AddRange(methods1[method]);
            }
            else if (!methods1.ContainsKey(method))
            {
                differences.Add($"Method only in file2: {method}");
                differences.AddRange(methods2[method]);
            }
            else
            {
                differences.AddRange(CompareMethodContents(method, methods1[method], methods2[method]));
            }
        }

        return differences;
    }

    private Dictionary<string, List<string>> ExtractMethods(string[] lines)
    {
        var methods = new Dictionary<string, List<string>>();
        var currentMethod = new List<string>();
        string currentMethodName = null;
        int braceCount = 0;
        bool inComment = false;
        bool inMethod = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // 주석 처리
            if (line.StartsWith("/*"))
            {
                inComment = true;
            }
            if (line.EndsWith("*/"))
            {
                inComment = false;
                continue;
            }
            if (inComment || line.StartsWith("//"))
            {
                continue;
            }

            // 메서드 시작 확인
            if (!inMethod && IsMethodStart(line))
            {
                currentMethodName = GetMethodSignature(lines, i);
                currentMethod = new List<string> { currentMethodName };
                inMethod = true;
                braceCount = 0;
            }
            else if (inMethod)
            {
                currentMethod.Add(line);
                braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');

                if (braceCount == 0)
                {
                    methods[currentMethodName] = currentMethod;
                    currentMethodName = null;
                    currentMethod = new List<string>();
                    inMethod = false;
                }
            }
        }

        if (currentMethodName != null)
        {
            methods[currentMethodName] = currentMethod;
        }

        return methods;
    }

    private bool IsMethodStart(string line)
    {
        var methodPattern = @"^\s*(public|private|protected|internal|static|\w+\s+\w+\s*=>\s*)\s*\w+\s+\w+\s*\(.*\).*$";
        return Regex.IsMatch(line, methodPattern);
    }

    private string GetMethodSignature(string[] lines, int startIndex)
    {
        var signature = new List<string>();
        int braceCount = 0;
        bool foundOpenBrace = false;

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            signature.Add(line);

            braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');

            if (line.Contains("{"))
            {
                foundOpenBrace = true;
            }

            if (foundOpenBrace && braceCount == 0)
            {
                break;
            }
        }

        return string.Join(" ", signature);
    }

    private List<string> CompareMethodContents(string methodName, List<string> method1, List<string> method2)
    {
        var differences = new List<string>();
        var ifStack = new Stack<string>();

        for (int i = 0; i < Math.Max(method1.Count, method2.Count); i++)
        {
            if (i >= method1.Count)
            {
                differences.Add($"In method {methodName}: Extra line in file2: {method2[i]}");
            }
            else if (i >= method2.Count)
            {
                differences.Add($"In method {methodName}: Extra line in file1: {method1[i]}");
            }
            else if (method1[i] != method2[i])
            {
                var line1 = method1[i].Trim();
                var line2 = method2[i].Trim();

                if (line1.StartsWith("if") || line2.StartsWith("if"))
                {
                    ifStack.Push(line1.StartsWith("if") ? line1 : line2);
                    differences.Add($"In method {methodName}: Different if condition:");
                    differences.Add($"  File1: {line1}");
                    differences.Add($"  File2: {line2}");
                }
                else
                {
                    if (ifStack.Count > 0)
                    {
                        differences.Add($"Inside {ifStack.Peek()}:");
                    }
                    differences.Add($"In method {methodName}: Line differs:");
                    differences.Add($"  File1: {line1}");
                    differences.Add($"  File2: {line2}");
                }
            }

            if (method1[i].Trim() == "}" && ifStack.Count > 0)
            {
                ifStack.Pop();
            }
        }

        return differences;
    }
}

// 사용 예시
class Program
{
    static void Main(string[] args)
    {
        var comparer = new CodeComparer();
        string outputFilePath = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight\UvhfControls\UvhfMainStatus\UvhfStatus.xaml";
        string inputFIlePath = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\PreviewVersion\20221107_F07Q\21_PPC_소스파일\Source\FlightSolution\UvhfControls\UvhfMainStatus\UvhfStatus.xaml";
                
        CLIComparer.GetFiles(new string[] { inputFIlePath, outputFilePath });

        CLIComparer.CompareFiles();
      
    }
}