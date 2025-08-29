namespace wpfCodeCheck.Domain.Models
{
    /// <summary>
    /// 코드 분석 결과를 담는 모델들
    /// </summary>
    public class ClassInfoModel
    {
        public string ClassName { get; set; } = string.Empty;
        public string ClassPath { get; set; } = string.Empty;
        public List<FunctionInfoModel> FunctionInfos { get; set; } = new List<FunctionInfoModel>();
        public List<VariableInfoModel> Variables { get; set; } = new List<VariableInfoModel>();
    }

    public class FunctionInfoModel
    {
        public string FunctionName { get; set; } = string.Empty;
        public string ParentFunctionName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;
        public List<ParameterInfoModel> Parameters { get; set; } = new List<ParameterInfoModel>();
    }

    public class ParameterInfoModel
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class VariableInfoModel
    {
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}