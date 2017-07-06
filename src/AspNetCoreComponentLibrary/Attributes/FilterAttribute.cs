using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreComponentLibrary.Abstractions;

namespace AspNetCoreComponentLibrary
{
    
    /// <summary>
    /// Атрибут для создания фильтра в админке
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : FieldBaseAttribute
    {
        public override string LocalizePrefix => "filter";

        /// <summary>
        /// Ключ локализации для лабела. Если не указано то будет использовано название поля.
        /// </summary>
        //public string Title { get; set; }

        /// <summary>
        /// Ключ локализации для "placeholder".
        /// </summary>
        //public string Placeholder { get; set; }

        /// <summary>
        /// Тип сравнения. В основном для текстовых полей
        /// </summary>
        public EnumFilterCompare Compare { get; set; }

    }
}
