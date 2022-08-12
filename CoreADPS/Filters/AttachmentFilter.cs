using System;
using System.Linq;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class AttachmentFilter: IMailParamFilter
    {
        public readonly string HashsumHex;

        public AttachmentFilter(string hashsumHex)
        {
            if (hashsumHex == null)
            {
                throw new ArgumentNullException("hashsumHex");
            }

            HashsumHex = hashsumHex;
        }

        public bool IsFiltered(Mail mail)
        {
            return mail.Attachments.Any(attachment => attachment.HashsumHex.StartsWith(HashsumHex));
        }
    }
}
