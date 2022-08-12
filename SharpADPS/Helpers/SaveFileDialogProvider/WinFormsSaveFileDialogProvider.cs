using System.Windows.Forms;

namespace WPFSharpADPS.Helpers.SaveFileDialogProvider
{
    public class WinFormsSaveFileDialogProvider : ISaveFileDialogProvider
    {
        public bool ChooseFile(out string path)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;
                return true;
            }

            path = null;
            return false;
        }
    }
}
