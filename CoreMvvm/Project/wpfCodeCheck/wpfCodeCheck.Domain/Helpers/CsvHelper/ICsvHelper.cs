namespace wpfCodeCheck.Domain.Local.Helpers
{
    public interface ICsvHelper
    {
        bool CreateCSVFile<T>(ICollection<T> collection, string path, bool overwrite = true, bool writeHeader = true);
        string[,] OpenCSVFile(string path);        

        void ExcepCOMExceptionToCsv(string path, params string[] colunms);
    }
}
