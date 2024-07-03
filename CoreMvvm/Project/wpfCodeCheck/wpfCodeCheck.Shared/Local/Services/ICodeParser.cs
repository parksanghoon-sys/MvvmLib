using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public interface ICodeParser
    {
        IList<ClassInfo> Parse(string code, string codePath);
    }
}
