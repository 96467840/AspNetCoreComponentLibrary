using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    /// <summary>
    /// Настройка репозитория для админки
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AdminControllerSettingsAttribute : Attribute
    {
        /// <summary>
        /// Префикс для локализации
        /// </summary>
        public string LocalizerPrefix { get; set; }

        /// <summary>
        /// Порядковый номер в меню.
        /// </summary>
        public int Priority { get; set; }

        /*public AdminControllerSettingsAttribute(string menuName, int priority)
        {
            this.MenuName = menuName; this.Priority = priority;
        }/**/
    }
}
