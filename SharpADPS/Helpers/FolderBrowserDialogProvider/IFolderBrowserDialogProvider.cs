namespace WPFSharpADPS.Helpers.FolderBrowserDialogProvider
{
    public interface IFolderBrowserDialogProvider
    {
        bool ChooseFolder(out string path);
    }
}
