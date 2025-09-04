namespace wpfCodeCheck.Domain.Local.Helpers
{
    /// <summary>
    /// CSV 파일 생성, 읽기, 예외 처리 기능을 제공하는 인터페이스
    /// </summary>
    public interface ICsvHelper
    {
        /// <summary>
        /// 컨렉션 데이터를 CSV 파일로 생성합니다.
        /// </summary>
        /// <typeparam name="T">컨렉션 요소의 타입</typeparam>
        /// <param name="collection">밴어낼 데이터 컨렉션</param>
        /// <param name="path">CSV 파일 경로</param>
        /// <param name="overwrite">기존 파일 덮어쓰기 여부 (기본값: true)</param>
        /// <param name="writeHeader">헤더 작성 여부 (기본값: true)</param>
        /// <returns>성공 여부</returns>
        bool CreateCSVFile<T>(ICollection<T> collection, string path, bool overwrite = true, bool writeHeader = true);
        /// <summary>
        /// CSV 파일을 읽어서 2차원 문자열 배열로 반환합니다.
        /// </summary>
        /// <param name="path">CSV 파일 경로</param>
        /// <returns>CSV 데이터를 담은 2차원 배열</returns>
        string[,] OpenCSVFile(string path);        

        /// <summary>
        /// COM 예외를 CSV 파일로 기록합니다.
        /// </summary>
        /// <param name="path">CSV 파일 경로</param>
        /// <param name="colunms">기록할 컴럼 데이터</param>
        void ExcepCOMExceptionToCsv(string path, params string[] colunms);
    }
}
