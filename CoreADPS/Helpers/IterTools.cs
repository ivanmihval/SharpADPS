using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreADPS.Helpers
{
    public class IterTools
    {
        public static IEnumerable<T> Chain<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            foreach (var item in enumerable1)
            {
                yield return item;
            }

            foreach (var item in enumerable2)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> Chain<T>(IEnumerable<T> enumerable1, Func<IEnumerable<T>> funcEnumerable2)
        {
            foreach (var item in enumerable1)
            {
                yield return item;
            }

            foreach (var item in funcEnumerable2())
            {
                yield return item;
            }
        }
    }
}
