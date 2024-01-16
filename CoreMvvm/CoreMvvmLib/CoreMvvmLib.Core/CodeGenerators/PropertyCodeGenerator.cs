using CoreMvvmLib.Core.CodeGenerators.GenInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace CoreMvvmLib.Core.CodeGenerators
{
    public class PropertyCodeGenerator : IIncrementalGenerator
    {
        private const string AtttributeNamespace = "CoreMvvmLib.Core.Attribute";
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclaration = context.SyntaxProvider
                                    .CreateSyntaxProvider(predicate: static(s, _) => PropertyCodeGenerator.IsSyntasForCodeGenerator(s),
                                                          transform: static(ctx, _) => PropertyCodeGenerator.GetTargetForCodeGenerator(ctx))
                                    .Where(x => x != null);
            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClass =
                context.CompilationProvider.Combine(classDeclaration.Collect());
            context.RegisterSourceOutput(compilationAndClass, static (spc, souce) =>
            {
                PropertyCodeGenerator.PropertyCodeExcute(souce.Item1,souce.Item2, spc);
            });
        }



        private static bool IsSyntasForCodeGenerator(SyntaxNode node)
        {
            if (node is not ClassDeclarationSyntax) return false;
            var classSyntax = node as ClassDeclarationSyntax;
            if(classSyntax.Members.Count == 0) return false;
            if (classSyntax.Members.Where((syntax) => syntax.AttributeLists.Count > 0).Count() == 0) return false;
            return true;
            
        }
        private static ClassDeclarationSyntax GetTargetForCodeGenerator(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = context.Node as ClassDeclarationSyntax;
            foreach(var memberSyntax in classDeclarationSyntax.Members)
            {
                foreach(var attributeListSyntax in memberSyntax.AttributeLists)
                {
                    foreach(var attributeSyntax in attributeListSyntax.Attributes)
                    {
                        if(context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                        {
                            continue;
                        }
                        INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        var fullName = attributeContainingTypeSymbol.ToDisplayString();
                        if (fullName == AtttributeNamespace)
                        {
                            return classDeclarationSyntax;
                        }
                    }
                }
            }
            return null;
        }
        private static void PropertyCodeExcute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            if (classes.IsDefaultOrEmpty) return;

            IEnumerable<ClassDeclarationSyntax> distictClasses = classes.Distinct();
            foreach (var cls in distictClasses)
            {
                var usingDirectives = cls.SyntaxTree.GetCompilationUnitRoot().Usings;
                var usingDeclaration = new StringBuilder($@"{usingDirectives}");
                string classNamespace = GetNamespace(compilation, cls);

                if (cls.Modifiers.Count == 0) continue;

                if (cls.Modifiers.Where((token) => token.ToString().Contains("partial") == true).Any())
                {
                    var description = new DiagnosticDescriptor("CoreMvvmLib",
                                                             "Wrong class modifier",
                                                             $"{classNamespace}.{cls.Identifier.ValueText} class modifier must be partial",
                                                             "Problem",
                                                             DiagnosticSeverity.Error,
                                                             true);
                    context.ReportDiagnostic(Diagnostic.Create(description, Location.None));
                    return;
                }

                if (cls.BaseList == null)
                {
                    var description = new DiagnosticDescriptor("CoreMvvmLib",
                                           "Wrong class modifier",
                                           $"Invalid base class information",
                                           "Problem",
                                           DiagnosticSeverity.Error,
                                           true);
                    context.ReportDiagnostic(Diagnostic.Create(description, Location.None));
                    return;
                }
                if (cls.BaseList.Types.Count == 0)
                {
                    var description = new DiagnosticDescriptor("CoreMvvmLib0002",
                                           "None inheritance",
                                           $"{classNamespace}.{cls.Identifier.ValueText} class have to inhert NotifyObject",
                                           "Problem",
                                           DiagnosticSeverity.Error,
                                           true);
                    context.ReportDiagnostic(Diagnostic.Create(description, Location.None));
                    return;
                }
                List<AutoFieldInfo> fields = GetFieldList(compilation, cls);
                if (fields.Count == 0) { continue; }

                foreach (var field in fields)
                {
                    if (field.Identifier.StartsWith("_") == true) continue;
                    var description = new DiagnosticDescriptor("CoreMvvmLib0003",
                                              "Property should start with _ character",
                                              $"{classNamespace}.{cls.Identifier.ValueText} class have wrong property : {field.Identifier}",
                                              "Problem",
                                              DiagnosticSeverity.Error,
                                              true);
                    context.ReportDiagnostic(Diagnostic.Create(description, Location.None));
                    return;
                }
                var source = """
                    {using}

                    namespace {clsNamespace}{
                        partial class {clsName}{

                    {fieldCollection}
                            
                        }
                    }
                    """;
                source = source.Replace("{clsNamespace}", classNamespace);
                source = source.Replace("{clsName}", cls.Identifier.ValueText);
                var propertyCodeGroup = "";

                foreach (var field in fields)
                {

                    string propertyCode = """

                                public {typeName} {fieldName}{
                                    get => {_fieldName};
                                    set => Property(ref {_fieldName}, value);
                                }

                    """;
                    propertyCode = propertyCode.Replace("{typeName}", field.TypeName);
                    propertyCode = propertyCode.Replace("{_fieldName}", field.Identifier);

                    var fieldName = field.Identifier;
                    fieldName = fieldName.Remove(0, 1);
                    propertyCode = propertyCode.Replace("{fieldName}", fieldName);
                    propertyCodeGroup += propertyCode;

                }
                source = source.Replace("{using}", usingDeclaration.ToString());
                source = source.Replace("{fieldCollection}", propertyCodeGroup);

                context.AddSource($"{classNamespace}.{cls.Identifier.ValueText}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private static List<AutoFieldInfo> GetFieldList(Compilation compilation, ClassDeclarationSyntax cls)
        {
            List<AutoFieldInfo> fieldList = new List<AutoFieldInfo>();
            var model = compilation.GetSemanticModel(cls.SyntaxTree);

            foreach(FieldDeclarationSyntax field in cls.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                bool found = false;
                foreach(AttributeListSyntax attributeListSyntax in field.AttributeLists)
                {
                    foreach(AttributeSyntax attribute in attributeListSyntax.Attributes)
                    {
                        if(model.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol )
                        {
                            continue;
                        }
                        INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        var fullName = attributeContainingTypeSymbol.ToDisplayString();
                        if (fullName == AtttributeNamespace)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false) continue;
                    foreach(var item in field.Declaration.Variables)
                    {
                        AutoFieldInfo info = new AutoFieldInfo()
                        {
                            Identifier = item.Identifier.ValueText,
                            TypeName = field.Declaration.Type.ToString()
                        };
                        fieldList.Add(info);
                    }
                }                
            }
            return fieldList;
        }

        private static string GetNamespace(Compilation compilation, ClassDeclarationSyntax cls)
        {
            var model = compilation.GetSemanticModel(cls.SyntaxTree);

            foreach (NamespaceDeclarationSyntax ns in cls.Ancestors().OfType<NamespaceDeclarationSyntax>())
            {
                return ns.Name.ToString();
            }

            return "";
        }
    }
}
