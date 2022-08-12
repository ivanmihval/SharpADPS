using CoreADPS.MailModels;

namespace CoreADPS.Storage.Models
{
    public struct FilteredMailResult
    {
        public Mail Mail;
        public string MailPath;
        public string MailHashsumHex;

        public FilteredMailResult(Mail mail, string mailPath, string mailHashsumHex)
        {
            Mail = mail;
            MailPath = mailPath;
            MailHashsumHex = mailHashsumHex;
        }
    }
}
