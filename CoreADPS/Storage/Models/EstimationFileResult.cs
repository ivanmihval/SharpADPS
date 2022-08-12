using System;

namespace CoreADPS.Storage.Models
{
    public struct EstimationFileResult
    {
        public string Path;
        public string HashsumHex;
        public string HashsumAlgorithm;
        public UInt64 SizeBytes;

        public EstimationFileResult(string path, string hashsumHex, UInt64 sizeBytes, string hashsumAlgorithm = "sha512")
        {
            Path = path;
            HashsumHex = hashsumHex;
            SizeBytes = sizeBytes;
            HashsumAlgorithm = hashsumAlgorithm;
        }
    }
}
