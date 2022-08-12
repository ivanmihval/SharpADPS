using CoreADPS.Helpers;

namespace CoreADPS.Storage.Models
{
    public struct MailAttachmentInfo
    {
        public string Path;
        public string HashsumHex;

        public MailAttachmentInfo(string path, string hashsumHex)
        {
            Path = path;
            HashsumHex = hashsumHex;
        }

        public static MailAttachmentInfo FromFilePath(string path)
        {
            var hashsumHex = HashsumCalculator.Sha512Checksum(path);
            return new MailAttachmentInfo(path, hashsumHex);
        }
    }
}
