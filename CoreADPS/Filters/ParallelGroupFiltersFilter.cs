using System.Collections.Generic;
using System.Linq;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class ParallelGroupFiltersFilter: IMailParamFilter
    {
        public readonly IEnumerable<IMailParamFilter> Filters;

        public ParallelGroupFiltersFilter(IEnumerable<IMailParamFilter> filters)
        {
            Filters = filters;
        }

        public bool IsFiltered(Mail mail)
        {
            return Filters.Any(f => f.IsFiltered(mail));
        }
    }
}
