using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public interface IField {
        int Priority { get; set; }
        string PropertyName { get; set; }
        string Title { get; }
        string Placeholder { get; }
        //List<string> NameKeys { get; set; }
        //List<string> PlaceholderKeys { get; set; }

        void Load(List<Languages> languages);
        string Check();
        object DefaultObject { get; }
        //List<object> ValueObject { get; }
        //string ValueForForm { get; }
        List<TT> GetValues<TT>();
        Type Type { get; }
        string DebugValues { get; }
    }

    public class Field<T> : IField
    {
        public Controller2Garin Controller { get; set; }
        protected readonly ILogger Logger;
        public EnumHtmlType HtmlType { get; set; }

        public bool NeedTranslate { get; set; }

        /// <summary>
        /// Ключи локализации имени на форме
        /// </summary>
        //public List<string> NameKeys { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Локализация placeholder
        /// </summary>
        //public List<string> PlaceholderKeys { get; set; }
        public string Placeholder { get; set; }

        /// <summary>
        /// Имя в форме и в сущности
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public T Default { get; set; }
        public object DefaultObject => Default;

       
        /// <summary>
        /// Список значений поля
        /// </summary>
        public List<T> Value { get; set; }
        //public List<object> ValueObject => Value.Select(i=>(object)i).ToList();

        /// <summary>
        /// Порядковый номер в форме.
        /// </summary>
        public int Priority { get; set; }

        public Type Type => typeof(T);

        public List<TT> GetValues<TT>()
        {
            if (typeof(T) != typeof(TT)) throw new Exception("Type mismatch");
            if (Value == null) return new List<TT>();
            // здесь можно написать конвертацию через Expression взамен двойного приведения типов (TT)(object)
            return Value.Select(i => (TT)(object)i).ToList();
        }
        public string DebugValues => string.Join(", ", GetValues<T>().Select(i => i.ToString()));

        public virtual void Load(List<Languages> languages)
        {
            Value = null; // признак того что поле не установлено

            var Values = Controller.Request.GetRequestValue(PropertyName);// as List<object>;
            if (Values == null)
            {
                // установка значения по умолчанию
                if (Default != null)
                {
                    // ....
                }
            }
            else
            {
                Value = new List<T>(); // если список будет пустой, то это значит что на форме что-то было, но преобразовать это не смогли

                if (typeof(T).Name == "string")
                {
                    foreach (var v in Values)
                    {
                        if (v == null) continue;
                        if (HtmlType == EnumHtmlType.TextArea)
                            Value.Add((T)(object)v.SanitizeHtml());
                        else
                            Value.Add((T)(object)v.StripHtml());
                    }
                }
                else
                {
                    var parse = typeof(T).GetMethod("Parse");
                    if (parse != null)
                    {
                        foreach (var v in Values)
                        {
                            if (v == null) continue;
                            try
                            {
                                Value.Add((T)parse.Invoke(null, new object[] { v }));
                            }
                            catch (Exception e)
                            {
                                Logger.LogError("Field::Load() exception in field {name} load: {e}", PropertyName, e.ToString());
                            }
                        }
                    }
                }
            }

            // загрузка переводов
            if (typeof(T).Name == "string")// стоит ли ограничивать тока строки преводами?
            {
                if ( languages != null && languages.Count > 1)
                    foreach (var lang in languages)
                    {
                        Values = Controller.Request.GetRequestValue(PropertyName);// as List<object>;

                    }
            }
        }

        public virtual string Check()
        {

            return null;
        }

        public Field(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, bool needTranslate, string propertName, int priority, string Default)
        {
            Controller = controller;
            Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);
            HtmlType = htmlType;
            PropertyName = propertName;
            Priority = priority;
            NeedTranslate = needTranslate;
            Title = title;
            Placeholder = placeholder;
            
            //this.Default = Default;
            if (!string.IsNullOrWhiteSpace(Default))
            {
                if (typeof(T).Name == "string")
                {
                    if (HtmlType == EnumHtmlType.TextArea)
                        this.Default = (T)(object)Default.SanitizeHtml();
                    else
                        this.Default = (T)(object)Default.StripHtml();
                }
                else
                {
                    var parse = typeof(T).GetMethod("Parse");
                    if (parse != null)
                    {
                        this.Default = (T)parse.Invoke(null, new object[] { Default });
                    }
                }
            }
        }
    }

    public class FieldSelect<T> : Field<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="title"></param>
        /// <param name="placeholder">Здесь в этом параметре будет текст "Выберите значение"</param>
        /// <param name="htmlType"></param>
        /// <param name="propertName"></param>
        /// <param name="priority"></param>
        /// <param name="Default"></param>
        public FieldSelect(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, string propertName, int priority, string Default) 
            : base(controller, title, placeholder, htmlType, false, propertName, priority, Default)
        {
        }

        /// <summary>
        /// Репозиторий (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public Type SelectRepository { get; set; }

        /// <summary>
        /// Имя поля ключа (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public string SelectKeyName { get; set; }

        /// <summary>
        /// Имя поля ключа связи (для типа EnumHtmlType.Tree)
        /// </summary>
        public string SelectParentName { get; set; }

        /// <summary>
        /// Имя поля строкового значения (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public string SelectValueName { get; set; }

        /// <summary>
        /// Cписок значений. Может быть нулл, тогда список составляем по SelectRepository. Значение закодировано Json
        /// </summary>
        public string SelectValuesJson { get; set; }
    }

    public class FieldFile : Field<string>
    {
        public FieldFile(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, string propertName, int priority) 
            : base(controller, title, placeholder, htmlType, false, propertName, priority, null)
        {
        }
    }

    public class OptionVM
    {
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

    // ------------------------ удалить!
    /*public class BaseFieldVM<A> where A : FieldBaseAttribute
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
    }*/

}
