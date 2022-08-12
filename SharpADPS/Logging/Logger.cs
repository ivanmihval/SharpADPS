using System;
using CoreADPS.Helpers;

namespace WPFSharpADPS.Logging
{
    public class Logger: ILogger
    {
        private readonly string _name;
        private readonly LoggingLevel _level;

        public void Write(LoggingLevel level, string message)
        {
            if (level >= _level)
            {
                GlobalLoggingStorage.AddRecord(String.Format("{0};{1};{2}", _name, level,
                                                             message.Replace(Environment.NewLine, "<NEWLINE>")));
            }
        }

        public Logger(string name, LoggingLevel? level = null)
        {
            _level = level.HasValue ? level.Value : GlobalLoggingStorage.Level;
            _name = name;
        }

        public static Logger GetDefault(string name)
        {
            return new Logger(name);
        }
    }
}
