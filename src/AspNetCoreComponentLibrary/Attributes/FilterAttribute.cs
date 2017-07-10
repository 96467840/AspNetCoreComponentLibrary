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

    }
}
