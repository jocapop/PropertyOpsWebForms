using System;

namespace PropertyOps.App
{
    public static class Compat
    {
        public static string SafeToString(object o)
        {
            return (o == null) ? null : o.ToString();
        }

        public static string SafeReplace(string s, string oldValue, string newValue)
        {
            return (s == null) ? null : s.Replace(oldValue, newValue);
        }
    }
}