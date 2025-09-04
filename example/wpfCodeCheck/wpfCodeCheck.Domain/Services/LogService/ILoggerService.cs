namespace wpfCodeCheck.Domain.Services.LogService
{
    public interface ILoggerService
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception? ex = null);

        IEnumerable<string> GetRecentLogs(int count);
    }
}
