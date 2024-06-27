using wpfCodeCheck.Sub.Local.Models;

namespace wpfCodeCheck.Sub.Local.Services
{
    public interface ICodeParser
    {
        IList<ClassInfo> Parse(string code, string codePath);
    }
}
