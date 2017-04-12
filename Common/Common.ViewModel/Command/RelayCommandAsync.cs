using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.ViewModel.Command
{
    [DataContract]
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private Task _task;

        public event EventHandler CanExecuteChanged;
        public RelayCommandAsync(Func<Task> execute)
            : this(execute, null)
        {
        }

        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_task == null || _task.IsCompleted)
                return _canExecute == null ? true : _canExecute();
            else
                return false;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            _task = _execute();
            RaiseCanExecuteChanged();
            await _task;
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    [DataContract]
    public class RelayCommandAsync<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Predicate<T> _canExecute;
        private Task _task;

        public event EventHandler CanExecuteChanged;
        public RelayCommandAsync(Func<T, Task> execute) : this(execute, null) { }

        public RelayCommandAsync(Func<T, Task> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_task == null || _task.IsCompleted)
                return _canExecute == null ? true : _canExecute((T)parameter);
            else
                return false;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            _task = _execute((T)parameter);
            RaiseCanExecuteChanged();
            await _task;
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
