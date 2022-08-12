using System;
using System.Collections.Generic;
using WPFSharpADPS.Helpers.MessageBoxProvider;

namespace TestCoreADPS.TestViewModels.Providers
{
    public class MockedMessageBoxProvider : IMessageBoxProvider
    {
        public readonly List<Tuple<string, string>> Calls = new List<Tuple<string, string>>();
        public readonly MessageBoxConfirmResult ConfirmResult = MessageBoxConfirmResult.Yes;

        public void Show(string message, string title)
        {
            Calls.Add(new Tuple<string, string>(message, title));
        }

        public MessageBoxConfirmResult Confirm(string message, string title)
        {
            return ConfirmResult;
        }
    }
}
