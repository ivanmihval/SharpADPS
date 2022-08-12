namespace CoreADPS.Storage.Models.ProgressModels
{
    public enum DeleteMailsEstimationStage
    {
        ScanningTargetFiles,
        ScanningAllFiles
    }

    public struct DeleteMailsEstimationProgressData
    {
        public DeleteMailsEstimationStage Stage;
        public int CurrentIndex;
        public int TotalMailsNumber;

        public DeleteMailsEstimationProgressData(DeleteMailsEstimationStage stage, int currentIndex, int totalMailsNumber)
        {
            Stage = stage;
            CurrentIndex = currentIndex;
            TotalMailsNumber = totalMailsNumber;
        }
    }
}
