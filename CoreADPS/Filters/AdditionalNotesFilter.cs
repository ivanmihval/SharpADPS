using System;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class AdditionalNotesFilter: IMailParamFilter
    {
        public readonly string AdditionalNotes;
        private readonly string _additionalNotesLower;

        public AdditionalNotesFilter(string additionalNotes)
        {
            if (additionalNotes == null)
            {
                throw new ArgumentNullException("additionalNotes");
            }
            AdditionalNotes = additionalNotes;
            _additionalNotesLower = additionalNotes.ToLowerInvariant();
        }

        public bool IsFiltered(Mail mail)
        {
            return mail.AdditionalNotes != null && mail.AdditionalNotes.ToLowerInvariant().Contains(_additionalNotesLower);
        }
    }
}
