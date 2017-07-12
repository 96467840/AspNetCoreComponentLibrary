using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface ILocalizer2Garin
    {
        //ILoggerFactory LoggerFactory { get; set; }
        //IStringLocalizerFactory LocalizerFactory { get; set; }
        //IStringLocalizer Localizer { get; set; }

        string DefaultCulture { get; set; }
        Languages Language { get; }

        HtmlString LocalizeHtml(string key, params object[] args);
        HtmlString LocalizeHtml(List<string> keys, params object[] args);

        string Localize(string key, params object[] args);
        string Localize(List<string> keys, params object[] args);

        void ResolveCulture(string CultureFromRouteData, List<Languages> SiteLanguages, IList<string> CulturalPreferences);
    }
}
