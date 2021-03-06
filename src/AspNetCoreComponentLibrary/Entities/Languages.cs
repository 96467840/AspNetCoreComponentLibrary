﻿using AspNetCoreComponentLibrary.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
//using System.Linq.Expressions;

namespace AspNetCoreComponentLibrary
{
    [EntitySettings(LocalizerPrefix = "languages")]
    public partial class Languages : BaseDM<long>, IBlockable, IWithSiteId
    {
        [Field(HtmlType = EnumHtmlType.Hidden)]
        public long SiteId { get; set; }

        [Field(HtmlType = EnumHtmlType.Text)]
        public string Name { get; set; }

        [Filter(HtmlType = EnumHtmlType.Text, Compare = EnumFilterCompare.Equals)]
        [Field(HtmlType = EnumHtmlType.Text)]
        public string Lang { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool IsDefault { get; set; }

        [Filter(HtmlType = EnumHtmlType.Select, SelectValuesJson = "[{Value:'',TitleKey:'common.all'},{Value:'True',TitleKey:'common.yes'},{Value:'False',TitleKey:'common.no'}]")]
        [Field(HtmlType = EnumHtmlType.CheckBox)]
        [OrderBy(Desc=true, Priority = 20)]
        public bool IsBlocked { get; set; }

        [Field(HtmlType = EnumHtmlType.Json)]
        public string Json { get; set; }

        public string ExternalId { get; set; }

        public string Localize(string key, params object[] args)
        {
            if (Strings.ContainsKey(key)) return (args == null) ? Strings[key] : string.Format(Strings[key], args);

            return key;
        }

        public string this[string key, params object[] args]
        {
            get
            {
                return Localize(key, args);
            }
        }

        private Dictionary<string, string> _Strings;
        private Dictionary<string, string> Strings {
            get
            {
                if (_Strings != null) return _Strings;
                _Strings = new Dictionary<string, string>();
                if (string.IsNullOrWhiteSpace(Json))
                {
                    return _Strings;
                }
                try
                {
                    var tmp = JsonConvert.DeserializeObject<List<List<string>>>(Json);
                    if (tmp != null)
                        foreach (var row in tmp)
                        {
                            if (row.Count >= 2)
                            {
                                var key = row[0];
                                if (string.IsNullOrWhiteSpace(key)) continue;
                                key = key.Trim();
                                if (string.IsNullOrWhiteSpace(key)) continue;

                                if (Regex.IsMatch(key, @"^[a-z\\.0-9_\\-]+$", RegexOptions.IgnoreCase))
                                {
                                    var val = row[1];
                                    if (val == null) continue;
                                    _Strings[key] = val.SanitizeHtml(); // разрешим html теги
                                }
                            }
                        }

                    return _Strings;
                }
                catch (Exception) {
                    return _Strings;
                }
                //return null;
            }
        }
    }
}
