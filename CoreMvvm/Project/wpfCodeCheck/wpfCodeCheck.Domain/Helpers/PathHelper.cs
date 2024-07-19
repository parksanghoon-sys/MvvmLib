namespace wpfCodeCheck.Domain.Helpers
{
    public class PathHelper
    {
        static PathHelper()
        {
            if (Directory.Exists(GetLocalDirectory()) is false)
            {
                Directory.CreateDirectory(GetLocalDirectory());
            }
        }
        public static string GetLocalDirectory(string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}SettingData\\{fileName}";
        public static string GetLocaDirectory(string subDir, string fileName) => $"{AppDomain.CurrentDomain.BaseDirectory}\\{subDir}\\{fileName}";
        public static string GetLocalDirectory() => $"{AppDomain.CurrentDomain.BaseDirectory}\\SettingData";
    }
}
