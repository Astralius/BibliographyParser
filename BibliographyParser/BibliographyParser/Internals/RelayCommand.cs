using System;
using System.Windows.Input;

namespace BibliographyParser.Internals
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Func<bool> canExecute;
        private readonly Action executeAction;
        private readonly Action<object> execute;

        public RelayCommand(Action executeAction, Func<bool> canExecuteFunc)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecuteFunc;
        }

        public RelayCommand(Action executeAction) : this(executeAction, () => true) { }

        public bool CanExecute(object parameter) => this.canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => executeAction();

        public void UpdateCanExecute() => this.canExecute.Invoke();
    }
}
