using System.Windows;

namespace WPFSharpADPS.Helpers.MessageBoxProvider
{
    public class WinFormsMessageBoxProvider: IMessageBoxProvider
    {
        public void Show(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        public MessageBoxConfirmResult Confirm(string message, string title)
        {
            var dialogResult = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return dialogResult == MessageBoxResult.Yes ? MessageBoxConfirmResult.Yes : MessageBoxConfirmResult.No;
        }
    }
}
