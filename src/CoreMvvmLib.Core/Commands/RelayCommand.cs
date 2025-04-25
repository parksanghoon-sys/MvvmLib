using System.Windows.Input;

namespace CoreMvvmLib.Core.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _excute;
        private readonly Predicate<object> _canExecute;
        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            _excute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }
        public RelayCommand(Action execute) : this(execute, null) { }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (_excute != null)
                _excute();
        }
    }
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        public RelayCommand(Action<T> excute) : this(excute, null)
        {
            
        }
        public RelayCommand(Action<T> excute, Predicate<T> canExcute)
        {
            _execute = excute ?? throw new ArgumentNullException(nameof(excute));
            _canExecute= canExcute;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            _execute((T)parameter);
        }
    }
}
