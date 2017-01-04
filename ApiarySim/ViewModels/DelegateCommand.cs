using System;
using System.Windows.Input;

namespace ApiarySim.ViewModels
{
    internal class DelegateCommand : ICommand

    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(Action<object> execute)

        {
            _execute = execute;

            _canExecute = (x) => true;
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)

        {
            _execute = execute;

            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)

        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)

        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()

        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}