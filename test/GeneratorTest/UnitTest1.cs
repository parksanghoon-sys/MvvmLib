using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using CoreMvvmLib.Core.CodeGenerators;
using CoreMvvmLib.Core.Attributes;
namespace GeneratorTest
{
    public class PropertyGenerator
    {
        [Property]
        private string _name;
        private void Test ()
        {

        }
        [Fact]
        public void Test1()
        {
                      
        }
        [Fact]
        public Task GeneratesEnumExtensionsCorrectly()
        {
            // 테스트 할 소스코드
            var source = @"
namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        private readonly IFileCheckSum _fileCheckSum;
        [Property]
        private string _export = string.Empty;

        public TestViewModel(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
            Export = ""Test"";
        }        
        [RelayCommand]
        public void FileDialogOpen()
        {
            BrowseForFolderDialog dlg = new BrowseForFolderDialog();
            dlg.Title = ""Select a folder and click OK!"";
            dlg.InitialExpandedFolder = @""c:\"";
            dlg.OKButtonText = ""OK!"";
            if (true == dlg.ShowDialog())
            {
                MessageBox.Show(dlg.SelectedFolder, ""Selected Folder"");
            }
        }
    }
}";

            // 소스 코드를 도우미에 전달하고 스냅샷 테스트 출력
            return TestHelper.Verify(source);
        }
    }
    public static class TestHelper
    {
        public static Task Verify(string source)
        {
            // 제공된 문자열을 C# 구문 트리로 구문 분석
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // 구문 트리에 대한 Roslyn 컴파일 생성
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "GeneratorTest",
                syntaxTrees: new[] { syntaxTree });


            // EnumGenerator 증분 소스 생성기의 인스턴스 생성
            var generator = new RelayCommandCodeGen();

            // GeneratorDriver는 컴파일에 대해 생성기를 실행하는데 사용됨
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // 소스 생성기를 실행!
            driver = driver.RunGenerators(compilation);

            // 소스 생성기 출력을 스냅샷 테스트하려면 Verifier를 사용!
            return Verifier.Verify(driver);
        }
    }
}