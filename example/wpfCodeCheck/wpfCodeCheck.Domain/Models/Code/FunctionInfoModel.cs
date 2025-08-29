namespace wpfCodeCheck.Domain.Models.Code
{
    public class FunctionInfoModel
    {
        public string FunctionName { get; set; } = string.Empty;
        public string ParentFunctionName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;
        public List<ParameterInfoModel> Parameters { get; set; } = new List<ParameterInfoModel>();
    }
}