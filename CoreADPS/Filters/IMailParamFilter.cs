using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public interface IMailParamFilter
    {
        bool IsFiltered(Mail mail);
    }
}
