using wpfCodeCheck.Domain.Models.Code;

namespace wpfCodeCheck.Component.Local.Services
{
    public interface ICodeParser
    {
        IList<ClassInfoModel> Parse(string code, string codePath);
    }
}
