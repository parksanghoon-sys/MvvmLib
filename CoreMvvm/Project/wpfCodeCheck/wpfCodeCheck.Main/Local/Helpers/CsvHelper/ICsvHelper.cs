namespace wpfCodeCheck.Main.Local.Helpers.CsvHelper
{
    public interface ICsvHelper
    {
        void CreatePathFolder(string path);
        bool CreateCSVFile<T>(ICollection<T> collection, string path, bool overwrite = true, bool writeHeader = true);
        string[,] OpenCSVFile(string path);
        void DeleteDirectory(string path);
    }
}
