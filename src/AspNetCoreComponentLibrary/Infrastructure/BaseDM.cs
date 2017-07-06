using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public interface IBaseDM
    {
        bool IsBlockable { get; }

        //Form ToForm<FA>(Controller2Garin controller) where FA:FieldBaseAttribute;
    }

    public abstract class BaseDM<K> : IBaseDM
    {
        public K Id { get; set; }

        public bool IsBlockable
        {
            get
            {
                return GetType().IsImplementsInterface(typeof(IBlockable));
            }
        }

        /*public Form ToForm<FA>(Controller2Garin controller) where FA : FieldBaseAttribute
        {
            var fields = new List<IField>();

            foreach (var f in this.GetType().GetProperties()
                .Select(i => new { Attribute = (FA)i.GetCustomAttribute(typeof(FA)), Property = i, }).Where(i => i.Attribute != null))
            {
                IField field = null;
                switch (f.Attribute.HtmlType)
                {
                    case EnumHtmlType.CheckBox:
                        field = new Field<bool>(controller);
                        break;
                }
                if (field != null) {
                    field.Priority = f.Attribute.Priority;

                    fields.Add(field);
                }
            }

            return new Form(controller, fields);
        }*/
    }
}
