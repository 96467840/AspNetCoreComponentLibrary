using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class OptionVM {
        public string Value { get; set; }
        public string Title { get; set; }
        
        /// <summary>
        /// Если данная опция выбрана, то значение будет равно "selected"
        /// </summary>
        public string Selected { get; set; }

        public OptionVM(string value, string title, string selected)
        {
            Value = value; Title = title; Selected = selected;
        }
    }

    public class BaseFieldVM<A> where A : FieldBaseAttribute
    {
        public A Attribute { get; set; }

        public PropertyInfo Property { get; set; }
        public List<string> Values { get; set; }
        //public string LocalizerPrefix { get; set; }
        public Controller2Garin Controller { get; set; }

        public BaseFieldVM(A attribute, PropertyInfo property, List<string> values, Controller2Garin controller)
        {
            Attribute = attribute; Property = property; Values = values; Controller = controller;
        }

        public virtual List<string> NameKeys
        {
            get
            {
                var res = new List<string>();
                res.Add(Controller.LocalizerPrefix + ".field." + Property.Name + ".title");
                res.Add("common.field." + Property.Name + ".title");
                return res;
            }
        }

        public virtual List<string> PlaceholderKeys
        {
            get
            {
                var res = new List<string>();
                res.Add(Controller.LocalizerPrefix + ".field." + Property.Name + ".placeholder");
                res.Add("common.field." + Property.Name + ".placeholder");
                return res;
            }
        }

        public List<OptionVM> SelectOptions
        {
            get
            {
                var res = new List<OptionVM>();
                if (!string.IsNullOrWhiteSpace(Attribute.SelectValuesJson))
                {

                }
                return res;
            }

        }
    }

    // ----------------- Filters
    public class FilterFieldVM: BaseFieldVM<FilterAttribute>
    {
        public FilterFieldVM(FilterAttribute attribute, PropertyInfo property, List<string> values, Controller2Garin controller) : base(attribute, property, values, controller)
        {
        }

        public override List<string> NameKeys
        {
            get
            {
                var res = base.NameKeys;
                if (!string.IsNullOrWhiteSpace(Attribute.Title)) res.Insert(0, Controller.LocalizerPrefix + "." + Attribute.Title);// .Add(LocalizerPrefix + "." + Attribute.Title);
                //res.Add(LocalizerPrefix + ".field." + Property.Name + ".title");
                //res.Add("common.field." + Property.Name + ".title");
                return res;
            }
        }

        public override List<string> PlaceholderKeys
        {
            get
            {
                var res = base.PlaceholderKeys;//new List<string>();
                if (!string.IsNullOrWhiteSpace(Attribute.Placeholder)) res.Insert(0, Controller.LocalizerPrefix + "." + Attribute.Placeholder);//.Add(LocalizerPrefix + "." + Attribute.Placeholder);
                //res.Add(LocalizerPrefix + ".field." + Property.Name + ".placeholder");
                //res.Add("common.field." + Property.Name + ".placeholder");
                return res;
            }
        }
    }

    // ----------------- Fields
    public class FieldVM : BaseFieldVM<FieldAttribute>
    {
        public FieldVM(FieldAttribute attribute, PropertyInfo property, List<string> values, Controller2Garin controller) : base(attribute, property, values, controller)
        {
        }
    }

}
