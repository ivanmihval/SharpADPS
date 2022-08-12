using System.Windows.Forms;

namespace WPFSharpADPS.Helpers.FolderBrowserDialogProvider
{
    public class WinFormsFolderBrowserDialogProvider : IFolderBrowserDialogProvider
    {
        public bool ChooseFolder(out string path)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;
                return true;
            }

            path = null;
            return false;
        }
    }
}
