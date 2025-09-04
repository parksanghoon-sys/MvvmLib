namespace wpfCodeCheck.ProjectChangeTracker.Local.Helpers;

public static class RetryHelper
{
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        int maxRetry = 3,
        int delayMs = 500,
        Action<Exception, int>? onRetry = null)
    {
        int attempt = 0;
        while (true)
        {
            attempt++;
            try
            {
                return await action();
            }
            catch (Exception ex) when (attempt < maxRetry)
            {
                onRetry?.Invoke(ex, attempt);
                await Task.Delay(delayMs);
            }
        }
    }

    public static async Task ExecuteAsync(
        Func<Task> action,
        int maxRetry = 3,
        int delayMs = 500,
        Action<Exception, int>? onRetry = null)
    {
        await ExecuteAsync(async () => { await action(); return true; }, maxRetry, delayMs, onRetry);
    }
}
