using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using cliCSharpParsing;

public class Program
{
    static void Main(string[] args)
    {
        //string projectPath = @"D:\WPF_Test_UI\src\CLI\cliWeakEvents";
        string projectPath = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution\B\Source\pspc-flight\UvhfControls";
        string[] excludeFiles = { "App.xaml.cs", "App.xaml", "AssemblyInfo.cs", "Resources.Designer.cs", "Settings.Designer.cs" };

        DirectoryInfo dirInfo = new DirectoryInfo(projectPath);
        DirectoryInfo[] infos = dirInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

        var parser = new CSharpParser();
        var allFunctions = new List<ClassInfo>();

        foreach (DirectoryInfo info in infos)
        {
            string projectName = info.Name;
            if (projectName == ".svn") continue;
            //FileInfo[] fileInfos = new string[] { "*.dll", "*.exe", "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.png", "*.config", "*.resx", "*.settings" }
            FileInfo[] fileInfos = new string[] { "*.cs" }
                    .SelectMany(i => info.GetFiles(i, SearchOption.AllDirectories))
                    .Where(file => !excludeFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

            foreach (var fi in fileInfos)
            {
                string content = File.ReadAllText(fi.FullName);
                var functions = parser.Parse(content, fi.FullName);
                allFunctions.AddRange(functions);
            }
        }


        //foreach (var file in files)
        //{
        //    string content = File.ReadAllText(file);
        //    var functions = parser.Parse(content);
        //    allFunctions.AddRange(functions);
        //}

        foreach (var function in allFunctions)
        {
            string func = string.Join(", ", function.FunctionInfos.Select(p => $"{p.FunctionName} {p.ParentFunctionName} {p.Summary} {p.ReturnType}"));
            Console.WriteLine($"Class: {function.ClassName}");
            Console.WriteLine($"Function: {func}");
            Console.WriteLine("Variables:");
            foreach (var variable in function.Variables)
            {
                Console.WriteLine($"  - Name: {variable.Name} Summary : {variable.Summary},  Type: {variable.Type}");
            }
            Console.WriteLine("Parameters:");
            //foreach (var parameter in function.Parameters)
            //{
            //    Console.WriteLine($"  - Name: {parameter.Name}, Type: {parameter.Type}");
            //}


        }
        string lastDirectory = projectPath
            .Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
            .Last();
        TestWord word = new TestWord();
        string filePath = Path.GetFullPath(@$"D:\Temp\{lastDirectory}.doc");
        word.CreateWordFile(filePath, allFunctions);
        Console.WriteLine();
    }
}

public class FunctionInfo
{
    public string FunctionName { get; set; }
    public string ParentFunctionName { get; set; }
    public string Summary { get; set; }
    public string ReturnType { get; set; }
    public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
}
public class ClassInfo
{
    public string ClassName { get; set; }
    public string ClassPath { get; set; }
    public List<FunctionInfo> FunctionInfos { get; set; } = new List<FunctionInfo>();
    public List<VariableInfo> Variables { get; set; } = new List<VariableInfo>();
}
public class VariableInfo
{
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Type { get; set; }
}
public class ParameterInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
}
public class CSharpParser
{
    public IList<ClassInfo> Parse(string code, string classPath)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot() as CompilationUnitSyntax;
        var functions = new List<FunctionInfo>();
        var classInfos = new List<ClassInfo>();

        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        foreach (var classNode in classes)
        {
            var className = classNode.Identifier.Text;
            var methods = classNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var variables = GetVariables(classNode);

            var classinfo = new ClassInfo()
            {
                ClassName = classNode.Identifier.Text,
                ClassPath = classPath,
                Variables = variables
            };


            foreach (var method in methods)
            {
                var functionInfo = new FunctionInfo
                {
                    FunctionName = method.Identifier.Text,
                    ParentFunctionName = null,
                    Summary = GetSummary(method),
                    ReturnType = method.ReturnType.ToString(),
                };
                foreach (var parameter in method.ParameterList.Parameters)
                {
                    functionInfo.Parameters.Add(new ParameterInfo
                    {
                        Name = parameter.Identifier.Text,
                        Type = parameter.Type!.ToString()
                    });
                }
                var parentFuncName = FindParentFunction(root, functionInfo.FunctionName);
                functionInfo.ParentFunctionName = parentFuncName == null ? className : parentFuncName;
                functions.Add(functionInfo);
            }
            classinfo.FunctionInfos = functions;
            classInfos.Add(classinfo);
        }



        return classInfos;
    }

    private string GetSummary(SyntaxNode node)
    {
        var trivia = node.GetLeadingTrivia();
        var docComment = trivia.Select(trivium => trivium.GetStructure())
                               .OfType<DocumentationCommentTriviaSyntax>()
                               .FirstOrDefault();

        if (docComment != null)
        {
            var summaryElement = docComment.ChildNodes()
                                           .OfType<XmlElementSyntax>()
                                           .FirstOrDefault(e => e.StartTag.Name.ToString() == "summary");

            if (summaryElement != null)
            {
                var summertStr = summaryElement.Content.ToString().Trim().Replace("///", "");
                return summertStr.Replace("\n", "").Replace("\r", "").Trim();
            }
        }

        return null;
    }

    private string FindParentFunction(SyntaxNode root, string functionName)
    {
        var methodCalls = root.DescendantNodes().OfType<InvocationExpressionSyntax>()
                             .Where(invocation => invocation.Expression.ToString().EndsWith(functionName));

        foreach (var call in methodCalls)
        {
            var parentMethod = call.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (parentMethod != null)
            {
                return parentMethod.Identifier.Text;
            }
        }

        return null;
    }

    private List<VariableInfo> GetVariables(ClassDeclarationSyntax classNode)
    {
        var variables = new List<VariableInfo>();

        var fields = classNode.DescendantNodes().OfType<FieldDeclarationSyntax>();
        foreach (var field in fields)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                variables.Add(new VariableInfo
                {
                    Name = variable.Identifier.Text,
                    Summary = GetSummary(field),
                    Type = field.Declaration.Type.ToString()
                });
            }
        }

        var properties = classNode.DescendantNodes().OfType<PropertyDeclarationSyntax>();
        foreach (var property in properties)
        {
            variables.Add(new VariableInfo
            {
                Name = property.Identifier.Text,
                Summary = GetSummary(property),
                Type = property.Type.ToString()
            });
        }

        return variables;
    }
}
