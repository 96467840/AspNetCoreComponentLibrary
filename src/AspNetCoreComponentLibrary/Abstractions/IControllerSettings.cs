using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IControllerSettings
    {
        IStorage Storage { get; set; }
        ILoggerFactory LoggerFactory { get; set; }
        ILocalizer2Garin Localizer2Garin { get; set; }
        //IStringLocalizerFactory LocalizerFactory { get; set; }
        //IStringLocalizer Localizer { get; set; }
        //string DefaultCulture { get; set; }
    }
}
