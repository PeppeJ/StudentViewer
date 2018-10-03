using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentViewer
{
    public static class Extensions
    {
        public static void AddUnique<T, V>(this IDictionary<T, V> dict, T key, V value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
        }
    }
}
