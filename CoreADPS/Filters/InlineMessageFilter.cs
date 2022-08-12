using System;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class InlineMessageFilter: IMailParamFilter
    {
        public readonly string InlineMessage;
        private readonly string _inlineMessageLower;

        public InlineMessageFilter(string inlineMessage)
        {
            if (inlineMessage == null)
            {
                throw new ArgumentNullException("inlineMessage");
            }
            InlineMessage = inlineMessage;
            _inlineMessageLower = inlineMessage.ToLowerInvariant();
        }

        public bool IsFiltered(Mail mail)
        {
            return mail.InlineMessage != null && mail.InlineMessage.ToLowerInvariant().Contains(_inlineMessageLower);
        }
    }
}
