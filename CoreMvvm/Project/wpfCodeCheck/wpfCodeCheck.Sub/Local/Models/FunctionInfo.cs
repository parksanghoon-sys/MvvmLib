﻿namespace wpfCodeCheck.Sub.Local.Models
{
    public class FunctionInfo
    {
        public string ClassName { get; set; }
        public string FunctionName { get; set; }
        public string ParentFunctionName { get; set; }
        public string Summary { get; set; }
        public string ReturnType { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        public List<VariableInfo> Variables { get; set; } = new List<VariableInfo>();
    }
}