using System;

namespace wpfCodeCheck.Domain.Helpers
{
    /// <summary>
    /// 디렉토리 관리를 위한 유틸리티 클래스
    /// 로컬 설정 디렉토리 생성, 경로 조합, 디렉토리 삭제 기능을 제공합니다.
    /// </summary>
    public class DirectoryHelper
    {
        /// <summary>
        /// 정적 생성자 - 로컬 설정 디렉토리가 존재하지 않으면 생성합니다.
        /// </summary>
        static DirectoryHelper()
        {
            if (Directory.Exists(GetLocalSettingDirectory()) is false)
            {
                Directory.CreateDirectory(GetLocalSettingDirectory());
            }
        }
        /// <summary>
        /// 설정 디렉토리에서 특정 파일의 전체 경로를 반환합니다.
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>설정 디렉토리의 파일 전체 경로</returns>
        public static string GetLocalSettingDirectory(string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}SettingData\\{fileName}";
        /// <summary>
        /// 애플리케이션 기본 디렉토리에서 특정 하위 디렉토리 경로를 반환합니다.
        /// </summary>
        /// <param name="fileName">하위 디렉토리명</param>
        /// <returns>하위 디렉토리 전체 경로</returns>
        public static string GetLocalDirectory(string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}{fileName}\\";
        /// <summary>
        /// 설정 데이터를 저장하는 로컬 디렉토리 경로를 반환합니다.
        /// </summary>
        /// <returns>설정 디렉토리 경로</returns>
        public static string GetLocalSettingDirectory() => @$"{AppDomain.CurrentDomain.BaseDirectory}SettingData";
        /// <summary>
        /// 내보내기 파일을 저장하는 로컬 디렉토리 경로를 반환합니다.
        /// </summary>
        /// <returns>내보내기 디렉토리 경로</returns>
        public static string GetLocalExportDirectory() => @$"{AppDomain.CurrentDomain.BaseDirectory}EXPORT";
        /// <summary>
        /// 애플리케이션 기본 디렉토리에서 하위 디렉토리와 파일명을 조합한 경로를 반환합니다.
        /// </summary>
        /// <param name="subDir">하위 디렉토리명</param>
        /// <param name="fileName">파일명</param>
        /// <returns>조합된 파일 경로</returns>
        public static string GetLocaDirectory(string subDir, string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}\\{subDir}\\{fileName}";
        /// <summary>
        /// 현재 사용자의 내 문서 폴더 경로를 반환합니다.
        /// </summary>
        /// <returns>내 문서 폴더 경로</returns>
        public static string GetMyDocumentsDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// <summary>
        /// 지정된 경로에 디렉토리가 존재하지 않으면 생성합니다.
        /// </summary>
        /// <param name="directoryPath">생성할 디렉토리 경로</param>
        /// <returns>새로운 디렉토리가 생성되면 true, 이미 존재하면 false</returns>
        public static bool CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath) is false)
            {
                Directory.CreateDirectory(directoryPath);
                return true ;
            }
            return false ;
        }
        /// <summary>
        /// 지정된 경로의 모든 폴더 및 하위 파일을 재귀적으로 삭제합니다.
        /// 삭제 중 발생하는 예외는 무시됩니다.
        /// </summary>
        /// <param name="path">삭제할 디렉토리 경로</param>
        public static void DeleteDirectory(string path)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }

                try
                {
                    Directory.Delete(path, true);
                }
                catch (IOException)
                {
                    Directory.Delete(path, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(path, true);
                }
            }
            catch { }
        }
    }
}
