namespace wpfCodeCheck.Component.Local.Models
{
    /// <summary>
    /// 변수 정보를 나타내는 모델 클래스
    /// 변수명, 요약, 데이터 타입 정보를 포함합니다.
    /// </summary>
    public class VariableInfo
    {
        /// <summary>
        /// 변수의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 변수에 대한 요약 설명을 가져오거나 설정합니다.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 변수의 데이터 타입을 가져오거나 설정합니다.
        /// </summary>
        public string Type { get; set; }
    }
}
