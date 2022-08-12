namespace WPFSharpADPS.Helpers.SaveFileDialogProvider
{
    public interface ISaveFileDialogProvider
    {
        bool ChooseFile(out string path);
    }
}
