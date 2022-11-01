using System;
using System.Windows.Input;

namespace DevelopTools.Infrastructure
{
    internal class DelegateCommand : ICommand
    {
        private event EventHandler CanExecuteChangedInternal;

        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;

        public DelegateCommand(Action executeMethod) : this(executeMethod, () => true) { }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : this((o) => { executeMethod(); }, o => canExecuteMethod())
        {
            if (null == executeMethod || null == canExecuteMethod)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }
        }

        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }

        public DelegateCommand(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (null == _canExecute)
            {
                return true;
            }
            return _canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        public void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChangedInternal;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
