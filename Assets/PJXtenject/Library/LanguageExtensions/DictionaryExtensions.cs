using System.Collections.Generic;

namespace PJXtenject.Library.LanguageExtensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TValue, List<TKey>> Invert<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            var inverted = new Dictionary<TValue, List<TKey>>();

            foreach (var (key, value) in original)
            {
                if (!inverted.ContainsKey(value))
                    inverted[value] = new ();

                inverted[value].Add(key);
            }

            return inverted;
        }
    }
}