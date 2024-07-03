using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Office.Interop.Word;
using wpfCodeCheck.Shared.Local.Models;
using ParameterInfo = wpfCodeCheck.Shared.Local.Models.ParameterInfo;

namespace wpfCodeCheck.Shared.Local.Services
{

    public class CSharpParser : ICodeParser
    {
        public IList<ClassInfo> Parse(string code, string codePath)
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
                    ClassPath = codePath,
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
                    return summaryElement.Content.ToString().Trim().Replace("///", "");
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
}
