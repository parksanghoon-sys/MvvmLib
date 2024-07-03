namespace wpfCodeCheck.Shared.Local.Models
{
    public class ClassInfo
    {
        public string ClassName { get; set; }
        public string ClassPath { get; set; }
        public List<FunctionInfo> FunctionInfos { get; set; } = new List<FunctionInfo>();
        public List<VariableInfo> Variables { get; set; } = new List<VariableInfo>();
    }
}
