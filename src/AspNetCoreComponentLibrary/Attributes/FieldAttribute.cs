using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreComponentLibrary.Abstractions;

namespace AspNetCoreComponentLibrary
{
    /// <summary>
    /// Атрибут для настройки отображения поля в форме
    /// </summary>
    public class FieldAttribute : FieldBaseAttribute
    {
        /// <summary>
        /// Ключ локализации для названия поля.
        /// </summary>
        // используем след схему LocalizerPrefix.ParentId
        //public string Title { get; set; }

    }
}
