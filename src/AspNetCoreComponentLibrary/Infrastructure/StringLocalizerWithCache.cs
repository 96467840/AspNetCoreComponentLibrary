using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace AspNetCoreComponentLibrary
{
    // примитивный кеширующий способ
    public class StringLocalizerWithCache //: IStringLocalizer
    {
        private static Dictionary<string, StringLocalizerWithCache> cache = new Dictionary<string, StringLocalizerWithCache>();

        private Dictionary<string, string> _cache;
        //public string this[string name] => Localize(name);

        public string this[string name, params object[] arguments] => Localize(name, arguments);

        public static StringLocalizerWithCache Get(IStringLocalizer source, string key)
        {
            if (!cache.ContainsKey(key))
            {
                cache[key] = new StringLocalizerWithCache(source);
            }
            return cache[key];
        }

        private StringLocalizerWithCache(IStringLocalizer source)
        {
            _cache = source.GetAllStrings(/*true*/).ToDictionary(i => i.Name, i => i.Value);
        }

        public string Localize(string key, params object[] args)
        {
            if (_cache.ContainsKey(key)) return (args == null) ? _cache[key] : string.Format(_cache[key], args);

            return key;
        }
    }
}
