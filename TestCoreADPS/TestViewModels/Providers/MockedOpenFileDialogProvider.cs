using WPFSharpADPS.Helpers.OpenFileDialogProvider;

namespace TestCoreADPS.TestViewModels.Providers
{
    public class MockedOpenFileDialogProvider : IOpenFileDialogProvider
    {
        private readonly string _outputPath;
        public int CalledTimes = 0;

        public bool ChooseFile(out string path)
        {
            CalledTimes++;

            if (_outputPath == null)
            {
                path = null;
                return false;
            }

            path = _outputPath;
            return true;
        }

        public MockedOpenFileDialogProvider(string outputPath = null)
        {
            _outputPath = outputPath;
        }
    }
}
