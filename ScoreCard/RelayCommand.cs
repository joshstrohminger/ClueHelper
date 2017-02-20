using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ScoreCard
{
    #region RelayCommand
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        #endregion Fields

        #region Constructors
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion Constructors

        #region Members
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
        #endregion Members
    }

    public class RelayCommand : ICommand
    {
        #region Fields
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        #endregion Fields

        #region Constructors
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion Constructors

        #region Members
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object obj)
        {
            _execute();
        }
        #endregion Members
    }
    #endregion RelayCommand
}