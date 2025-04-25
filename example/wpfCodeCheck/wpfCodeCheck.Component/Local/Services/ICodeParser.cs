using wpfCodeCheck.Component.Local.Models;

namespace wpfCodeCheck.Component.Local.Services
{
    public interface ICodeParser
    {
        IList<ClassInfo> Parse(string code, string codePath);
    }
}
