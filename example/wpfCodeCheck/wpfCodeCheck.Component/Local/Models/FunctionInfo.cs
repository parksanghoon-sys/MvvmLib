namespace wpfCodeCheck.Component.Local.Models
{
    /// <summary>
    /// 함수 정보를 나타내는 모델 클래스
    /// 함수명, 부모 함수, 요약 정보, 반환 타입, 매개변수 목록을 포함합니다.
    /// </summary>
    public class FunctionInfo
    {
        /// <summary>
        /// 함수의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 부모 함수의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string ParentFunctionName { get; set; }
        /// <summary>
        /// 함수에 대한 요약 설명을 가져오거나 설정합니다.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 함수의 반환 타입을 가져오거나 설정합니다.
        /// </summary>
        public string ReturnType { get; set; }
        /// <summary>
        /// 함수의 매개변수 목록을 가져오거나 설정합니다.
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();        
    }
}
