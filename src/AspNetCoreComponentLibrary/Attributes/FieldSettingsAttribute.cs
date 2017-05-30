using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    /// <summary>
    /// Атрибут для настройки отображения поля в форме
    /// </summary>
    public class FieldSettingsAttribute:Attribute
    {
        /// <summary>
        /// Ключ локализации для названия поля.
        /// </summary>
        // используем след схему LocalizerPrefix.ParentId
        //public string Title { get; set; }

        /// <summary>
        /// Тип поля
        /// </summary>
        public EnumHtmlType HtmlType { get; set; }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Порядковый номер в форме.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Имя поля ключа (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public string SelectKeyName { get; set; }

        /// <summary>
        /// Имя поля строкового значения (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public string SelectValueName { get; set; }

    }
}
