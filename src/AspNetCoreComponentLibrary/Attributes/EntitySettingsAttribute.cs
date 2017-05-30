using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class EntitySettingsAttribute:Attribute
    {
        /// <summary>
        /// Префикс для локализации
        /// </summary>
        public string LocalizerPrefix { get; set; }

    }
}
