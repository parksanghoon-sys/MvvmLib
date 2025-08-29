using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Component.Local.Services
{
    public interface ICodeParser
    {
        IList<ClassInfoModel> Parse(string code, string codePath);
    }
}
