﻿using CoreMvvmLib.Core.CodeGenerators.GenInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace CoreMvvmLib.Core.CodeGenerators
{
    [Generator]
    internal class PropertyCodeGen : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {

            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //                Debugger.Launch();
            //            }
            //#endif 

            var classDeclarations = context.SyntaxProvider
                                           .CreateSyntaxProvider(predicate: static (s, _) => PropertyCodeGen.IsSyntaxForCodeGen(s),
                                                                 transform: static (ctx, _) => PropertyCodeGen.GetTargetForCodeGen(ctx))
                                           .Where(x => x != null);


            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClass = context.CompilationProvider.Combine(classDeclarations.Collect());
            context.RegisterSourceOutput(compilationAndClass, static (spc, source) => PropertyCodeGen.PropertyCodeExecute(source.Item1, source.Item2, spc));
        }



        #region Filter
        internal static bool IsSyntaxForCodeGen(SyntaxNode node)
        {
            if (node is not ClassDeclarationSyntax) return false;
            var classSyntax = (ClassDeclarationSyntax)node;
            if (classSyntax.Members.Count == 0) return false;
            if (classSyntax.Members.Where((syntax) => syntax.AttributeLists.Count > 0).Count() == 0) return false;
            return true;
        }

        internal static ClassDeclarationSyntax GetTargetForCodeGen(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;


            foreach (var memeberSyntax in classDeclarationSyntax.Members)
            {
                foreach (AttributeListSyntax attributeListSyntax in memeberSyntax.AttributeLists)
                {
                    foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                    {
                        if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                        {
                            continue;
                        }
                        INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        string fullName = attributeContainingTypeSymbol.ToDisplayString();
                        if (fullName == "CoreMvvmLib.Core.Attributes.PropertyAttribute")
                        {
                            return classDeclarationSyntax;
                        }
                    }
                }
            }


            return null;
        }
        #endregion

        #region Generator
        internal static string GetNamespace(Compilation compilation, MemberDeclarationSyntax cls)
        {
            var model = compilation.GetSemanticModel(cls.SyntaxTree);

            foreach (NamespaceDeclarationSyntax ns in cls.Ancestors().OfType<NamespaceDeclarationSyntax>())
            {
                return ns.Name.ToString();
            }

            return "";
        }

        internal static List<AutoFieldInfo> GetFieldList(Compilation compilation, MemberDeclarationSyntax cls)
        {
            List<AutoFieldInfo> fieldList = new List<AutoFieldInfo>();

            var model = compilation.GetSemanticModel(cls.SyntaxTree);

            foreach (FieldDeclarationSyntax field in cls.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                bool found = false;
                foreach (AttributeListSyntax attributeListSyntax in field.AttributeLists)
                {
                    foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                    {
                        if (model.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                        {
                            continue;
                        }
                        INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        string fullName = attributeContainingTypeSymbol.ToDisplayString();
                        if (fullName == "CoreMvvmLib.Core.Attributes.PropertyAttribute")
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found == true) break;
                }

                if (found == false) continue;

                foreach (var item in field.Declaration.Variables)
                {

                    AutoFieldInfo info = new AutoFieldInfo
                    {
                        Identifier = item.Identifier.ValueText,
                        TypeName = field.Declaration.Type.ToString()
                    };

                    fieldList.Add(info);
                }
            }

            return fieldList;
        }

        internal static void PropertyCodeExecute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            if (classes.IsDefaultOrEmpty)
            {
                return;
            }




            IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.Distinct();
            foreach (var cls in distinctClasses)
            {
                // Import all the packages
                var usingDirectives = cls.SyntaxTree.GetCompilationUnitRoot().Usings;
                var usingDeclaration = new StringBuilder($@"{usingDirectives}");

                string clsNamespace = GetNamespace(compilation, cls);
                if (cls.Modifiers.Count == 0) continue;
                if (cls.Modifiers.Where((token) => token.ToString().Contains("partial") == true).Count() == 0)
                {
                    var description = new DiagnosticDescriptor("CoreMvvmLib0001",
                                                               "Wrong class modifier",
                                                               $"{clsNamespace}.{cls.Identifier.ValueText} class modifier must be partial",
                                                               "Problem",
                                                               DiagnosticSeverity.Error,
                                                               true);
                    context.ReportDiagnostic(Diagnostic.Create(description, Location.None));
                    return;
                }

                if (cls.BaseList == null)
                {
                    var description = new DiagnosticDescriptor("CoreMvvmLib0000",
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
                                           $"{clsNamespace}.{cls.Identifier.ValueText} class have to inhert NotifyObject",
                                           "Problem",
                                           DiagnosticSeverity.Error,
                                           true);
                    context.ReportDiagnostic(Diagnostic.Create(description, Location.None));
                    return;
                }
                List<AutoFieldInfo> fieldList = GetFieldList(compilation, cls);
                if (fieldList.Count == 0) continue;


                foreach (var field in fieldList)
                {
                    if (field.Identifier.StartsWith("_") == true) continue;
                    var description = new DiagnosticDescriptor("CoreMvvmLib0003",
                                                               "Property should start with _ character",
                                                               $"{clsNamespace}.{cls.Identifier.ValueText} class have wrong property : {field.Identifier}",
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

                source = source.Replace("{clsNamespace}", clsNamespace);
                source = source.Replace("{clsName}", cls.Identifier.ValueText);


                var propertyCodeGroup = "";
                foreach (var field in fieldList)
                {

                    string propertyCode = """

                                public {typeName} {fieldName}{
                                    get => {_fieldName};
                                    set => SetProperty(ref {_fieldName}, value);
                                }

                    """;

                    propertyCode = propertyCode.Replace("{typeName}", field.TypeName);
                    propertyCode = propertyCode.Replace("{_fieldName}", field.Identifier);

                    var fieldName = field.Identifier;
                    fieldName = fieldName.Remove(0, 1);
                    fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
                    propertyCode = propertyCode.Replace("{fieldName}", fieldName);
                    propertyCodeGroup += propertyCode;
                }

                source = source.Replace("{using}", usingDeclaration.ToString());
                source = source.Replace("{fieldCollection}", propertyCodeGroup);

                context.AddSource($"{clsNamespace}.{cls.Identifier.ValueText}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
        #endregion

    }
}
