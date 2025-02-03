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
            // �׽�Ʈ �� �ҽ��ڵ�
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

            // �ҽ� �ڵ带 ����̿� �����ϰ� ������ �׽�Ʈ ���
            return TestHelper.Verify(source);
        }
    }
    public static class TestHelper
    {
        public static Task Verify(string source)
        {
            // ������ ���ڿ��� C# ���� Ʈ���� ���� �м�
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // ���� Ʈ���� ���� Roslyn ������ ����
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "GeneratorTest",
                syntaxTrees: new[] { syntaxTree });


            // EnumGenerator ���� �ҽ� �������� �ν��Ͻ� ����
            var generator = new RelayCommandCodeGen();

            // GeneratorDriver�� �����Ͽ� ���� �����⸦ �����ϴµ� ����
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // �ҽ� �����⸦ ����!
            driver = driver.RunGenerators(compilation);

            // �ҽ� ������ ����� ������ �׽�Ʈ�Ϸ��� Verifier�� ���!
            return Verifier.Verify(driver);
        }
    }
}