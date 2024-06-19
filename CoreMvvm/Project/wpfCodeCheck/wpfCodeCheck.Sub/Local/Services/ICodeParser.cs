using wpfCodeCheck.Sub.Local.Models;

namespace wpfCodeCheck.Sub.Local.Services
{
    public interface ICodeParser
    {
        List<FunctionInfo> Parse(string code);
    }
}
