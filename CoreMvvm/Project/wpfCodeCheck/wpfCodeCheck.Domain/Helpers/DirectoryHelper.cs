using System;

namespace wpfCodeCheck.Domain.Helpers
{
    public class DirectoryHelper
    {
        static DirectoryHelper()
        {
            if (Directory.Exists(GetLocalSettingDirectory()) is false)
            {
                Directory.CreateDirectory(GetLocalSettingDirectory());
            }
        }
        public static string GetLocalSettingDirectory(string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}SettingData\\{fileName}";
        public static string GetLocalDirectory(string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}{fileName}\\";
        public static string GetLocalSettingDirectory() => $"{AppDomain.CurrentDomain.BaseDirectory}\\SettingData";
        public static string GetLocalExportDirectory() => $"{AppDomain.CurrentDomain.BaseDirectory}\\EXPORT";
        public static string GetLocaDirectory(string subDir, string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}\\{subDir}\\{fileName}";
        public static string GetMyDocumentsDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static bool CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath) is false)
            {
                Directory.CreateDirectory(directoryPath);
                return true ;
            }
            return false ;
        }
        
    }
}
