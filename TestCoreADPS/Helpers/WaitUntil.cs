using System;
using System.Threading;

namespace TestCoreADPS.Helpers
{
    public class WaitUntil
    {
        public static void WaitUntilFuncIsTrue(Func<bool> func, int intervalMiliseconds = 1000, int attempts = 10)
        {
            for (var i = 0; i < attempts; i++)
            {
                if (func())
                {
                    return;
                }
                Thread.Sleep(intervalMiliseconds);
            }

            if (!func())
            {
                throw new Exception("Timeout exception");
            }
        }
    }
}
