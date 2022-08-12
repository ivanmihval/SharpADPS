using System;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class DateTimeCreatedRangeFilter : IMailParamFilter
    {
        public readonly DateTime? DateTimeFrom = null;
        public readonly DateTime? DateTimeTo = null;

        public DateTimeCreatedRangeFilter(DateTime? dateTimeFrom = null, DateTime? dateTimeTo = null)
        {
            DateTimeFrom = dateTimeFrom;
            DateTimeTo = dateTimeTo.HasValue ? dateTimeTo : DateTime.Today + TimeSpan.FromDays(2);
        }

        public bool IsFiltered(Mail mail)
        {
            if (DateTimeFrom.HasValue && (DateTimeFrom.Value > mail.DateCreated))
            {
                return false;
            }

            if (DateTimeTo.HasValue && (DateTimeTo.Value < mail.DateCreated))
            {
                return false;
            }

            return true;
        }
    }
}
