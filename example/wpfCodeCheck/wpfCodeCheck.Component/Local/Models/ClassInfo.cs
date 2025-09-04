namespace wpfCodeCheck.Component.Local.Models
{
    /// <summary>
    /// 클래스 정보를 나타내는 모델 클래스
    /// 클래스명, 경로, 함수 목록, 변수 목록을 포함합니다.
    /// </summary>
    public class ClassInfo
    {
        /// <summary>
        /// 클래스의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 클래스 파일의 경로를 가져오거나 설정합니다.
        /// </summary>
        public string ClassPath { get; set; }
        /// <summary>
        /// 클래스에 포함된 함수 정보의 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<FunctionInfo> FunctionInfos { get; set; } = new List<FunctionInfo>();
        /// <summary>
        /// 클래스에 포함된 변수 정보의 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<VariableInfo> Variables { get; set; } = new List<VariableInfo>();
    }
}
