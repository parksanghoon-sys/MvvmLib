using System.Collections.Concurrent;

namespace wpfCodeCheck.Domain.Services.LogService
{
    public class FileLogger : ILoggerService, IDisposable
    {
        private readonly BlockingCollection<string> _logQueue = new();
        private readonly ConcurrentQueue<string> _recentLogs = new();
        private readonly string _logFilePath;
        private readonly Thread _workerThread;
        private readonly CancellationTokenSource _cts = new();

        public FileLogger()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            _logFilePath = Path.Combine(exeDir, $"{today}.log");

            // 로그 디렉토리 자동 생성
            var dir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // 로그 쓰는 전용 스레드 시작
            _workerThread = new Thread(ProcessLogs)
            {
                IsBackground = true,
                Name = "LogWriterThread"
            };
            _workerThread.Start();
        }
        private void Enqueue(string level, string message, Exception? ex = null)
        {
            string log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            if (ex != null)
                log += Environment.NewLine + ex;

            _logQueue.Add(log);

            // 최근 로그는 최대 100개 유지
            _recentLogs.Enqueue(log);
            while (_recentLogs.Count > 100 && _recentLogs.TryDequeue(out _)) { }
        }

        private void ProcessLogs()
        {
            try
            {
                using var writer = new StreamWriter(_logFilePath, append: true);
                foreach (var log in _logQueue.GetConsumingEnumerable(_cts.Token))
                {
                    writer.WriteLine(log);
                    writer.Flush();
                }
            }
            catch (OperationCanceledException)
            {
                // 정상 종료
            }
        }

        public void Info(string message) => Enqueue("INFO", message);
        public void Warn(string message) => Enqueue("WARN", message);
        public void Error(string message, Exception? ex = null) => Enqueue("ERROR", message, ex);

        public IEnumerable<string> GetRecentLogs(int count) =>
            _recentLogs.Reverse().Take(count);

        public void Dispose()
        {
            _cts.Cancel();
            _logQueue.CompleteAdding();
            _workerThread.Join();
            _cts.Dispose();
        }
    }
}
