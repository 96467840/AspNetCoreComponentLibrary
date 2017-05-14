using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class ControllerSettings : IControllerSettings
    {
        public IStorage Storage { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public IStringLocalizerFactory LocalizerFactory { get; set; }
        public IStringLocalizer Localizer { get; set; }

        public ControllerSettings(IStorage storage, ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory, IStringLocalizer localizer)
        {
            Storage = storage;
            LoggerFactory = loggerFactory;
            LocalizerFactory = localizerFactory;
            Localizer = localizer;
        }
    }
}
