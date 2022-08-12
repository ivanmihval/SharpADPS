namespace WPFSharpADPS.Helpers.MessageBoxProvider
{
    public enum MessageBoxConfirmResult
    {
        Yes,
        No,
    }

    public interface IMessageBoxProvider
    {
        void Show(string message, string title);
        MessageBoxConfirmResult Confirm(string message, string title);
    }
}
