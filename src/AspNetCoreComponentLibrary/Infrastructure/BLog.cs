using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class BLog : IDisposable
    {
        private readonly ILogger Logger;
        private string Name;

        /*public BLog(ILoggerFactory loggerFactory, string name, string args, string @namepace = "Microsoft.EntityFrameworkCore.Our")
        {
            Logger = loggerFactory.CreateLogger(namepace);
            Name = name;
            Start(args);
        }/**/

        public BLog(ILogger logger, string name, string args)
        {
            Logger = logger;
            Name = name;
            Start(args);
        }

        public void Start(string args)
        {
            Logger.LogInformation("--------------- {0} {1}\n\n\n\n\n", Name, args);
        }

        public void Dispose()
        {
            Logger.LogInformation("- - - - - - - - END OF {0}\n\n\n\n\n", Name);
        }
    }
}
