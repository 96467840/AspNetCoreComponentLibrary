using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    /// <summary>
    /// Настройка репозитория для админки
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RepositorySettingsAttribute : Attribute
    {
        /// <summary>
        /// Название пункта меню
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// Порядковый номер в меню. Если меньше 0, то в меню не включать
        /// </summary>
        public int Priority { get; set; }
    }
}
