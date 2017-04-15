using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;

namespace AspNetCoreComponentLibrary
{
    class ControllerEditable : Controller2Garin
    {
        public ControllerEditable(IStorage storage, ILoggerFactory loggerFactory) : base(storage, loggerFactory)
        {
        }
    }
}
