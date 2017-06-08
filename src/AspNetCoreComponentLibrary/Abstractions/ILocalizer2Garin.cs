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

        HtmlString Localize(string key);

        void ResolveCulture(ActionExecutingContext context, List<Languages> SiteLanguages, IUrlHelper Url);
    }
}
