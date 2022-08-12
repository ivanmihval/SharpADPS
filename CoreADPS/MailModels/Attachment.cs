using System;
using System.IO;
using CoreADPS.Storage.Models;
using Newtonsoft.Json;

namespace CoreADPS.MailModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Attachment
    {
        public const string DefaultHashsumAlgorithm = "sha512";

        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        [JsonProperty(PropertyName = "size_bytes")]
        public UInt64 SizeBytes { get; set; }

        [JsonProperty(PropertyName = "hashsum_hex")]
        public string HashsumHex { get; set; }

        [JsonProperty(PropertyName = "hashsum_alg")]
        public string HashsumAlgorithm { get; set; }

        public Attachment(string filename, UInt64 sizeBytes, string hashsumHex, string hashsumAlgorithm = DefaultHashsumAlgorithm)
        {
            Filename = filename;
            SizeBytes = sizeBytes;
            HashsumHex = hashsumHex;
            HashsumAlgorithm = hashsumAlgorithm;
        }

        public static Attachment FromMailAttachmentInfo(MailAttachmentInfo attachmentInfo)
        {
            var filename = Path.GetFileName(attachmentInfo.Path);
            var sizeBytes = (UInt64) new FileInfo(attachmentInfo.Path).Length;

            return new Attachment(filename, sizeBytes, attachmentInfo.HashsumHex);
        }
    }
}
