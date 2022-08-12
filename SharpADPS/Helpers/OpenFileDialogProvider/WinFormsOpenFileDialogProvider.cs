using System.Windows.Forms;

namespace WPFSharpADPS.Helpers.OpenFileDialogProvider
{
    public class WinFormsOpenFileDialogProvider : IOpenFileDialogProvider
    {
        public bool ChooseFile(out string path)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                return true;
            }

            path = null;
            return false;
        }
    }
}
