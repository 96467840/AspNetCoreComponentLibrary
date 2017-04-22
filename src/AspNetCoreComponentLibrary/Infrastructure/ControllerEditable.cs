using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;

namespace AspNetCoreComponentLibrary
{
    class ControllerEditable<K, T> : Controller2Garin where T : BaseDM<K> where K : struct
    {
        public ControllerEditable(IStorage storage, ILoggerFactory loggerFactory) : base(storage, loggerFactory)
        {
        }
    }
}
