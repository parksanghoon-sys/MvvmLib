namespace wpfCodeCheck.Component.Local.Models
{
    /// <summary>
    /// 함수 매개변수 정보를 나타내는 모델 클래스
    /// 매개변수의 이름과 데이터 타입 정보를 포함합니다.
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// 매개변수의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 매개변수의 데이터 타입을 가져오거나 설정합니다.
        /// </summary>
        public string Type { get; set; }
    }
}
