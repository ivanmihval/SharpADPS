using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreADPS.Helpers;

namespace CoreADPS.Storage.Models
{
    public struct CopyMailsEstimationResult
    {
        public List<EstimationFileResult> MailFilesEstimationFileResults;
        public List<EstimationFileResult> AttachmentFilesEstimationFileResults;

        public int TotalFilesNumber;
        public UInt64 TotalFilesSizeBytes;

        public CopyMailsEstimationResult(List<EstimationFileResult> mailFilesEstimationFileResults,
                                         List<EstimationFileResult> attachmentFilesEstimationFileResults)
        {
            MailFilesEstimationFileResults = mailFilesEstimationFileResults;
            AttachmentFilesEstimationFileResults = attachmentFilesEstimationFileResults;

            var totalFilesNumber = mailFilesEstimationFileResults.Count + attachmentFilesEstimationFileResults.Count;
            UInt64 totalFilesSizeBytes = 0;
            foreach (var estimationResult in IterTools.Chain(mailFilesEstimationFileResults, attachmentFilesEstimationFileResults))
            {
                totalFilesSizeBytes += estimationResult.SizeBytes;
            }

            TotalFilesNumber = totalFilesNumber;
            TotalFilesSizeBytes = totalFilesSizeBytes;
        }

    }
}
