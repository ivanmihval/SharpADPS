namespace CoreADPS.Helpers
{
    public enum LoggingLevel
    {
        Debug=1,
        Info=2,
        Warning=3,
        Error=4,
    }

    public interface ILogger
    {
        void Write(LoggingLevel level, string message);
    }
}
