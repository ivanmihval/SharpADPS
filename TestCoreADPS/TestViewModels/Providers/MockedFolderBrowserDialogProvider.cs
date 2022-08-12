using WPFSharpADPS.Helpers.FolderBrowserDialogProvider;

namespace TestCoreADPS.TestViewModels.Providers
{
    public class MockedFolderBrowserDialogProvider : IFolderBrowserDialogProvider
    {
        public string OutputPath;
        public int CalledTimes = 0;

        public bool ChooseFolder(out string path)
        {
            CalledTimes++;

            if (OutputPath == null)
            {
                path = null;
                return false;
            }

            path = OutputPath;
            return true;
        }

        public MockedFolderBrowserDialogProvider(string outputPath = null)
        {
            OutputPath = outputPath;
        }
    }
}
