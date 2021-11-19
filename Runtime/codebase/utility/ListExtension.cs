
﻿namespace AllArt.Solana.Utility
{
    using System.Collections.Generic;

    [System.Serializable]

    public static class ListExtension
    {
        public static List<T> Splice<T>(this List<T> source, int index, int count)
        {
            var items = source.GetRange(index, count);
            source.RemoveRange(index, count);
            return source;
        }

        public static (string, bool) FindStringInList(this List<string> list, string key)
        {
            if (list.Contains(key))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Equals(key))
                    {
                        return (list[i + 1], true);
                    }
                }
            };

            return ("", false);
        }
    }
}