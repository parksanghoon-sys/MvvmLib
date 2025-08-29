namespace wpfCodeCheck.Domain.Models.Code
{
    public class ClassInfoModel
    {
        public string ClassName { get; set; } = string.Empty;
        public string ClassPath { get; set; } = string.Empty;
        public List<FunctionInfoModel> FunctionInfos { get; set; } = new List<FunctionInfoModel>();
        public List<VariableInfoModel> Variables { get; set; } = new List<VariableInfoModel>();
    }
}