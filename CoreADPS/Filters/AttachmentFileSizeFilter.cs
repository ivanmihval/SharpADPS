using System.Linq;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class AttachmentFileSizeFilter: IMailParamFilter
    {
        public readonly ulong MaxAttachmentSizeBytes;

        public AttachmentFileSizeFilter(ulong maxAttachmentSizeBytes)
        {
            MaxAttachmentSizeBytes = maxAttachmentSizeBytes;
        }

        public bool IsFiltered(Mail mail)
        {
            return mail.Attachments.All(attachment => attachment.SizeBytes <= MaxAttachmentSizeBytes);
        }
    }
}
