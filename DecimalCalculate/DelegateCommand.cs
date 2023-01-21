using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DecimalCalculate
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            this._canExecute = canExecute;
            this._command = command;
        }

        public void Execute(object parameter = null)
        {
            this._command(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
                return this._canExecute();
            return true;
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged == null)
                return;
            this.CanExecuteChanged((object)this, EventArgs.Empty);
        }
    }
}
