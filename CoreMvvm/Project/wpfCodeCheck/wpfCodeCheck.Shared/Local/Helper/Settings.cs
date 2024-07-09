    using System.IO;
using System.Xml.Serialization;

namespace wpfCodeCheck.Shared.Local.Helper
{
    public class SettingsXML
    {                
        public string InputPath;
        public string OutputPath;        
    }
    public static class Settings
    {
        private static string path1 = string.Empty;
        private static string path2 = string.Empty;
        private static string userAppDataPath = string.Empty;
        private static string executablePath = string.Empty;
        public static string UserAppDataPath
        {
            get { return userAppDataPath; }
            set { userAppDataPath = value; }
        }
        public static string ExecutablePath
        {
            get { return executablePath; }
            set { executablePath = value; }
        }
        public static string InputPath
        {
            get { return path1; }
            set { path1 = value; }
        }

        public static string OutputPath
        {
            get { return path2; }
            set { path2 = value; }
        }
        public static void SaveSettings()
        {
            try
            {
                SettingsXML settingsXML = new SettingsXML();

                settingsXML.InputPath = InputPath;
                settingsXML.OutputPath = OutputPath;                

                XmlSerializer serializer = new XmlSerializer(typeof(SettingsXML));
                TextWriter writer = new StreamWriter(userAppDataPath + @"\Settings.xml");

                serializer.Serialize(writer, settingsXML);
                writer.Close();
            }
            catch (Exception)
            {
            }
        }
        public static void LoadSettings()
        {
            try
            {
                SettingsXML settingsXML;

                XmlSerializer serializer = new XmlSerializer(typeof(SettingsXML));
                TextReader reader = new StreamReader(userAppDataPath + @"\Settings.xml");

                settingsXML = (SettingsXML)serializer.Deserialize(reader);
                reader.Close();

                InputPath = settingsXML.InputPath;
                OutputPath = settingsXML.OutputPath;                
            }
            catch (Exception)
            {
            }
        }
    }
}
