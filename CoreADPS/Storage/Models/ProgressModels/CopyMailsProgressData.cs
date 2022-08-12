using System;

namespace CoreADPS.Storage.Models.ProgressModels
{
    public enum CopyMailsStage
    {
        Estimation,
        Copying
    }

    public struct CopyMailProgressData
    {
        public int CurrentFileIndex;
        public UInt64 CurrentFileSizeBytes;
        public string CurrentFilePath;

        public int TotalFilesNumber;
        public UInt64 TotalFilesSizeBytes;

        public UInt64 CopiedBytes;

        public CopyMailProgressData(int currentFileIndex, UInt64 currentFileSizeBytes, string currentFilePath,
                                    int totalFilesNumber, UInt64 totalFilesSizeBytes, UInt64 copiedBytes)
        {
            CurrentFileIndex = currentFileIndex;
            CurrentFileSizeBytes = currentFileSizeBytes;
            CurrentFilePath = currentFilePath;
            TotalFilesNumber = totalFilesNumber;
            TotalFilesSizeBytes = totalFilesSizeBytes;
            CopiedBytes = copiedBytes;
        }
    }

    public struct CopyMailsProgressData
    {
        public CopyMailsStage Stage;

        public FilterMailsProgressData? EstimationProgressData;
        public CopyMailProgressData? CopyingProgressData;

        public CopyMailsProgressData(CopyMailsStage stage, 
                                     FilterMailsProgressData? estimationProgressData = null,
                                     CopyMailProgressData? copyingProgressData = null)
        {
            Stage = stage;
            EstimationProgressData = estimationProgressData;
            CopyingProgressData = copyingProgressData;
        }
    }
}
