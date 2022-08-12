using System;
using System.Windows.Input;
using CoreADPS.Helpers;
using WPFSharpADPS.Helpers.MessageBoxProvider;
using WPFSharpADPS.Logging;

namespace WPFSharpADPS.Helpers
{
    // https://stackoverflow.com/a/12423962
    public class CommandHandler : ICommand
    {
        public IMessageBoxProvider MessageBoxProvider = new WinFormsMessageBoxProvider();

        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;
        private readonly Logger _logger;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
            _logger = new Logger("CommandHandler");
        }

        /// <summary>
        /// Wires CanExecuteChanged event
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                _action(parameter);
            }
            catch (Exception e)
            {
                _logger.Write(LoggingLevel.Error,
                              String.Format("Error on execute {0} with param {1}: {2}", _action, parameter, e));
                MessageBoxProvider.Show(e.ToString(), "Error");
            }
        }
    }
}
