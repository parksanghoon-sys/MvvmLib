using System.Windows.Input;

namespace CoreMvvmLib.Core.Commands;

public class RelayCommandAsync : ICommand
{
    private readonly Func<Task> _execute = null;
    private readonly Func<bool> _canExecute;

    private bool _isRunning = false;
    #region Public Property
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            _isRunning = value;
        }
    }
    #endregion
    public RelayCommandAsync(Func<Task> excute)
    {
        _execute = excute;
    }
    public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public async void Execute(object? parameter)
    {
        if (_execute != null)
        {
            var task = _execute();
            this.IsRunning = true;
            await task;
            this.IsRunning = false;
        }
    }
}

public class RelayCommandAsync<T> : ICommand
{
    private readonly Func<T, Task> _execute;
    private readonly Predicate<T> _canExecute;
    private bool _isRunning = false;
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            _isRunning = value;
        }
    }
    public RelayCommandAsync(Func<T,Task> excute)
    {
        _execute ??= _execute = excute;
    }
    public RelayCommandAsync(Func<T, Task> execute, Predicate<T> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException(nameof(execute));
        if (canExecute == null)
            throw new ArgumentNullException(nameof(canExecute));

        _execute = execute;
        _canExecute = canExecute;
    }
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (_canExecute == null)
            return true;

        return _canExecute.Invoke((T)parameter);
    }

    public async void Execute(object? parameter)
    {
        if (_execute != null)
        {
            var task = _execute((T)parameter);
            this.IsRunning = true;
            await task;
            this.IsRunning = false;
        }
    }
}

