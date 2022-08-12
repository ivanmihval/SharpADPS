namespace WPFSharpADPS.Helpers.OpenFileDialogProvider
{
    public interface IOpenFileDialogProvider
    {
        bool ChooseFile(out string path);
    }
}
