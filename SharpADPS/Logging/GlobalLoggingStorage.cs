using System;
using System.Collections.Generic;
using System.Linq;
using CoreADPS.Helpers;

namespace WPFSharpADPS.Logging
{
    public enum GlobalLoggingStorageState
    {
        Active,
        Inactive,
    }

    public static class GlobalLoggingStorage
    {
        public static int MinMessagesCount = 500000;
        public static int MaxMessagesCount = 1000000;

        public static List<string> Entries = new List<string>(MaxMessagesCount);

        public static LoggingLevel Level = LoggingLevel.Debug;
        public static GlobalLoggingStorageState State = GlobalLoggingStorageState.Inactive;

        public static void CleanEntries()
        {
            Entries = new List<string>(MaxMessagesCount);
        }

        public static void AddRecord(string message)
        {
            if (State == GlobalLoggingStorageState.Inactive)
            {
                return;
            }

            if (Entries.Count >= MaxMessagesCount)
            {
                var newEntriesTmp = Entries.Skip(Math.Max(0, Entries.Count - MinMessagesCount)).ToArray();
                CleanEntries();
                Entries.AddRange(newEntriesTmp);
            }

            Entries.Add(message);
        }
    }
}
