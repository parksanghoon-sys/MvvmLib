using CoreMvvmLib.Core.IOC;
using System.Windows.Threading;
using wpfCodeCheck.Domain.Services.LogService;

namespace wpfCodeCheck.ProjectChangeTracker.Local.Helpers;

internal sealed class StaDispatcher : IDisposable
{
    private readonly Thread _thread;
    private readonly TaskCompletionSource<Dispatcher> _dispatcherTcs = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly ILoggerService _loggerService;
    private bool _disposed = false;
    public StaDispatcher(string name = "Excel-STA")
    {
        _thread = new Thread(ThreadStart) { IsBackground = true, Name = name };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
        _loggerService = ContainerProvider.Resolve(typeof(ILoggerService)) as ILoggerService;
    }
    public async Task<T> InvokeAsync<T>(Func<T> func)
    {
        var dispatcher = await _dispatcherTcs.Task.ConfigureAwait(false);
        var tcs = new TaskCompletionSource<T>();
        dispatcher.BeginInvoke(new Action(() =>
        {
            try { tcs.SetResult(func()); }
            catch (Exception ex) 
            { 
                tcs.SetException(ex);
            }
        }));
        

        return await tcs.Task.ConfigureAwait(false);
    }

    public async Task InvokeAsync(Action action)
    {
        var dispatcher = await _dispatcherTcs.Task.ConfigureAwait(false);
        var tcs = new TaskCompletionSource();
        dispatcher.BeginInvoke(new Action(() =>
        {
            try { action();  tcs.SetResult(); }
            catch (Exception ex) 
            {                 
                tcs.SetException(ex);
            }
        }));
        

        await tcs.Task.ConfigureAwait(false);
    }
    private void ThreadStart()
    {
        var dispatcher = Dispatcher.CurrentDispatcher;
        _dispatcherTcs.SetResult(dispatcher);
        Dispatcher.Run(); // 메시지 루프
    }
    // IDisposable 인터페이스의 Dispose() 메서드 구현
    public void Dispose()
    {
        // 관리되는 리소스와 관리되지 않는 리소스를 모두 정리합니다.
        Dispose(true);
        // GC.SuppressFinalize()를 호출하여 소멸자가 다시 호출되는 것을 막습니다.
        GC.SuppressFinalize(this);
    }
    // 소멸자 (Finalizer)
    ~StaDispatcher()
    {
        // 가비지 컬렉터에 의해 호출됩니다. 관리되지 않는 리소스만 정리합니다.
        Dispose(false);
    }
    protected void Dispose(bool disposing)
    {

        // 이미 Dispose 되었으면 아무것도 하지 않습니다.
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            if (_dispatcherTcs.Task.IsCompleted)
            {
                _dispatcherTcs.Task.Result.BeginInvokeShutdown(DispatcherPriority.Normal);
            }
            _cts.Cancel();
        }
        // 객체가 정리되었음을 표시
        _disposed = true;
    }   
}
