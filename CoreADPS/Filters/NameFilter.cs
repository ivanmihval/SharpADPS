using System;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class NameFilter: IMailParamFilter
    {
        public readonly string Name;

        public NameFilter(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
        }


        public bool IsFiltered(Mail mail)
        {
            return mail.Name == Name;
        }
    }
}
