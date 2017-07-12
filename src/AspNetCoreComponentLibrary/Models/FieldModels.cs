using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public interface IField
    {
        EnumHtmlType HtmlType { get; }
        int Priority { get; set; }
        string PropertyName { get; set; }
        string Title { get; }
        string Placeholder { get; }
        bool NeedTranslate { get; }
        bool IsMultiple { get; }
        bool IsRequired { get; }
        EnumFilterCompare Compare { get; }
        string Format { get; }

        /// <summary>
        /// Value для checkbox & radio. Зависит от типа свойства.
        /// </summary>
        string Set { get; }
        void Load(List<Languages> languages);
        string Check();
        List<object> GetDefaultAsObject();
        List<string> GetDefaultAsString();
        //List<object> ValueObject { get; }
        //string ValueForForm { get; }
        //List<TT> GetValues<TT>();
        List<string> GetValueAsString();
        List<object> GetValueAsObject();
        Type Type { get; }
        //string DebugValues { get; }

        List<OptionVM> GetOptions();
    }

    public class Field<T> : IField
    {
        public Controller2Garin Controller { get; set; }
        protected ILogger Logger;
        public EnumHtmlType HtmlType { get; set; }

        public bool NeedTranslate { get; set; }

        public bool IsMultiple { get; set; }

        public bool IsRequired { get; set; }

        public EnumFilterCompare Compare { get; set; }

        public string Format { get; set; }

        protected string AttributeLocalizePrefix = "custom";

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
        public List<T> Default { get; set; }

        public List<object> GetDefaultAsObject() => Default?.Select(i => (object)i).ToList() ?? new List<object>();
        // каждый тип T надо приводить к строке по своему! помни про злоебучие даты
        public List<string> GetDefaultAsString() => Default?.Select(i => Type.ToStringVM(i, Format)).ToList() ?? new List<string>();


        /// <summary>
        /// Список значений поля
        /// </summary>
        public List<T> Value { get; set; }
        //public List<object> ValueObject => Value.Select(i=>(object)i).ToList();

        /// <summary>
        /// Порядковый номер в форме.
        /// </summary>
        public int Priority { get; set; }

        public string Set
        {
            get
            {
                if (Type.Name.EqualsIC("bool") || Type.Name.EqualsIC("boolean")) return true.ToString();
                return "1";
            }
        }

        public Type Type => typeof(T);

        public List<object> GetValueAsObject() => Value?.Select(i => (object)i).ToList() ?? new List<object>();
        // каждый тип T надо приводить к строке по своему! помни про злоебучие даты
        public List<string> GetValueAsString() => Value?.Select(i => Type.ToStringVM(i, Format)).ToList() ?? new List<string>();

        //public string DebugValues => string.Join(", ", GetValues<T>().Select(i => i.ToString()));

        private List<T> ParseValuesFromStrings(List<string> Values)
        {
            if (Values == null) return null;
            var res = new List<T>(); // если список будет пустой, то это значит что на форме что-то было, но преобразовать это не смогли

            if (typeof(T).Name.EqualsIC("string"))
            {
                foreach (var v in Values)
                {
                    if (v == null) continue;
                    if (HtmlType == EnumHtmlType.TextArea)
                        res.Add((T)(object)v.SanitizeHtml());
                    else
                        res.Add((T)(object)v.StripHtml());
                }
            }
            else
            {
                // строка это не Nullable!!!
                //var test = Nullable.GetUnderlyingType(typeof(string));

                var type = typeof(T);
                var typeOfNullable = Nullable.GetUnderlyingType(type);

                var parse = typeOfNullable != null ? typeOfNullable.GetMethod("Parse", new Type[] { typeof(string) }) : type.GetMethod("Parse", new Type[] { typeof(string) });
                if (parse != null)
                {
                    foreach (var v in Values)
                    {
                        if (string.IsNullOrWhiteSpace(v)) continue;

                        // для Nullable нужно предусмотреть задать null значение, пустая строка это, по умолчанию, все записи (при фильтрации) при сохранении тоже иногда есть разница между пустой строкой и нулом
                        if (v == "null" && typeOfNullable != null)
                        {
                            res.Add(default(T));
                            continue;
                        }

                        try
                        {
                            res.Add((T)parse.Invoke(null, new object[] { v }));
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("Field::ParseValuesFromStrings() exception in field {name} load: {e}", PropertyName, e.ToString());
                        }
                    }
                }
            }
            return res;
        }

        public virtual void Load(List<Languages> languages)
        {
            var Values = Controller.Request.GetRequestValue(PropertyName);// as List<object>;
            Value = ParseValuesFromStrings(Values);

            // загрузка переводов
            if (typeof(T).Name.EqualsIC("string"))// стоит ли ограничивать тока строки преводами?
            {
                if (languages != null && languages.Count > 1)
                    foreach (var lang in languages)
                    {
                        //Values = Controller.Request.GetRequestValue(PropertyName);// as List<object>;

                    }
            }
        }

        public virtual string Check()
        {

            return null;
        }

        private void _init(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, bool isRequired, bool needTranslate, bool isMultiple,
            string propertyName, int priority, List<string> Default, EnumFilterCompare compare = EnumFilterCompare.Equals, string format = null)
        {
            Controller = controller;
            Logger = controller.LoggerFactory.CreateLogger(GetType().FullName);
            HtmlType = htmlType;
            PropertyName = propertyName;
            Priority = priority;
            IsRequired = isRequired;
            NeedTranslate = needTranslate;
            IsMultiple = isMultiple;
            Title = title;
            Placeholder = placeholder;
            Compare = compare;
            Format = format;

            //this.Default = Default;

            this.Default = ParseValuesFromStrings(Default);

        }

        public virtual List<OptionVM> GetOptions()
        {
            throw new NotImplementedException();
        }

        public Field(Controller2Garin controller, FieldBaseAttribute Attribute, string propertyName)
        {
            var title = controller.Localizer2Garin.Localize(
                Utils.GenLocalizeKeysList(controller.LocalizerPrefix, Attribute.LocalizePrefix, propertyName, "title")
            ) ?? propertyName;

            var placeholder = controller.Localizer2Garin.Localize(
                Utils.GenLocalizeKeysList(controller.LocalizerPrefix, Attribute.LocalizePrefix, propertyName, "placeholder")
            ) ?? propertyName;
            AttributeLocalizePrefix = Attribute.LocalizePrefix;
            _init(controller, title, placeholder, Attribute.HtmlType, Attribute.IsRequired,
                            Attribute.NeedTranslate, Attribute.IsMultiple, propertyName, Attribute.Priority, Attribute.Default == null ? null : new List<string>() { Attribute.Default },
                            Attribute.Compare, Attribute.Format);
        }

        public Field(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, bool isRequired, bool needTranslate, bool isMultiple,
            string propertyName, int priority, List<string> Default, EnumFilterCompare compare = EnumFilterCompare.Equals, string format = null)
        {
            _init(controller, title, placeholder, htmlType, isRequired, needTranslate, isMultiple,
             propertyName, priority, Default, compare, format);
        }
    }

    public class FieldSelect<T> : Field<T>
    {
        public FieldSelect(Controller2Garin controller, FieldBaseAttribute Attribute, string PropertyName) : base(controller, Attribute, PropertyName)
        {
            SelectRepository = Attribute.SelectRepository;
            SelectValueName = Attribute.SelectValueName;
            SelectParentName = Attribute.SelectParentName;
            SelectTitleName = Attribute.SelectTitleName;
            SelectValuesJson = Attribute.SelectValuesJson;
            SelectOnlyUnblocked = Attribute.SelectOnlyUnblocked;
            SelectTreePrefix = Attribute.SelectTreePrefix ?? "&nbsp;&nbsp;&nbsp;&nbsp;";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="title"></param>
        /// <param name="placeholder">Здесь в этом параметре будет текст "Выберите значение"</param>
        /// <param name="htmlType"></param>
        /// <param name="propertyName"></param>
        /// <param name="priority"></param>
        /// <param name="Default"></param>
        /*public FieldSelect(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, bool isRequired, bool isMultiple, 
            string propertyName, int priority, string Default, EnumFilterCompare compare = EnumFilterCompare.Equals) 
            : base(controller, title, placeholder, htmlType, isRequired, false, isMultiple, propertyName, priority, Default, compare)
        {
            SelectRepository = Attribute.SelectRepository;
            SelectKeyName = Attribute.SelectKeyName;
            SelectParentName = Attribute.SelectParentName;
            SelectValueName = Attribute.SelectValueName;
            SelectValuesJson = Attribute.SelectValuesJson;
        }*/

        /// <summary>
        /// Репозиторий (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public Type SelectRepository { get; set; }

        /// <summary>
        /// Имя поля ключа (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public string SelectValueName { get; set; }

        /// <summary>
        /// Имя поля ключа связи (для типа EnumHtmlType.Tree)
        /// </summary>
        public string SelectParentName { get; set; }

        /// <summary>
        /// Имя поля строкового значения (для типа EnumHtmlType.Select и EnumHtmlType.Tree)
        /// </summary>
        public string SelectTitleName { get; set; }

        /// <summary>
        /// Cписок значений. Может быть нулл, тогда список составляем по SelectRepository. Значение закодировано Json
        /// </summary>
        public string SelectValuesJson { get; set; }

        public bool SelectOnlyUnblocked { get; set; }

        public string SelectTreePrefix { get; set; }

        void _fillSelected()
        {
            if (_options == null) return;
            var values = GetValueAsString();

            foreach (var opt in _options)
            {
                opt.BSelected = values.Contains(opt.Value);
            }
        }

        // todo можно сделать кеширование с учетом языка
        List<OptionVM> _options;
        public override List<OptionVM> GetOptions()
        {
            if (_options != null)
            {
                // возможно в этом случае нужно переустановить опцию selected
                _fillSelected();
                return _options;
            }

            _options = new List<OptionVM>();
            if (string.IsNullOrWhiteSpace(SelectValuesJson) && SelectRepository == null)
            {
                // попробуем автоматически создать списки значений
                var type = typeof(T);
                var typeOfNullable = Nullable.GetUnderlyingType(type);
                string title;
                if (type.Name.EqualsIC("bool") || type.Name.EqualsIC("boolean") ||
                    (typeOfNullable != null && (typeOfNullable.Name.EqualsIC("bool") || typeOfNullable.Name.EqualsIC("boolean"))))
                {
                    _options = new List<OptionVM>();

                    title = /*"bool_all";//*/ Controller.Localizer2Garin.Localize(Utils.GenLocalizeKeysList(Controller.LocalizerPrefix, AttributeLocalizePrefix, PropertyName, "bool_all", true));
                    _options.Add(new OptionVM("", title, null));

                    if (typeOfNullable != null && (typeOfNullable.Name.EqualsIC("bool") || typeOfNullable.Name.EqualsIC("boolean")))
                    {
                        title = /*"bool_undefined";//*/ Controller.Localizer2Garin.Localize(Utils.GenLocalizeKeysList(Controller.LocalizerPrefix, AttributeLocalizePrefix, PropertyName, "bool_undefined", true));
                        _options.Add(new OptionVM("null", title, null));
                    }

                    title = /*"bool_true";//*/ Controller.Localizer2Garin.Localize(Utils.GenLocalizeKeysList(Controller.LocalizerPrefix, AttributeLocalizePrefix, PropertyName, "bool_true", true));
                    _options.Add(new OptionVM("True", title, null));

                    title = /*"bool_false";//*/ Controller.Localizer2Garin.Localize(Utils.GenLocalizeKeysList(Controller.LocalizerPrefix, AttributeLocalizePrefix, PropertyName, "bool_false", true));
                    _options.Add(new OptionVM("False", title, null));
                }
            }
            else if (SelectRepository != null)
            {
                var repository = Controller.Storage.GetOptionsRepository(SelectRepository, EnumDB.Content, true);
                if (repository == null) throw new Exception("Cannot find repository: " + SelectRepository.Name);
                _options = repository.GetOptions(Controller.Site.Id, Controller.Localizer2Garin.Language, Controller.SiteLanguages, SelectValueName, SelectTitleName, SelectParentName, SelectTreePrefix, SelectOnlyUnblocked);
                // надо добавить "выбрать все" и "верхний уровень" а возьмем мы их с SelectValuesJson
                if (!string.IsNullOrWhiteSpace(SelectValuesJson))
                {
                    var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
                    var options = JsonConvert.DeserializeObject<List<OptionVM>>(SelectValuesJson, deserializeSettings);
                    if (options == null) throw new Exception("Cann't deserialize SelectValuesJson property");
                    foreach (var opt in options)
                    {
                        if (!string.IsNullOrWhiteSpace(opt.TitleKey))
                            opt.Title = Controller.Localizer2Garin.Localize(opt.TitleKey);
                    }
                    _options.InsertRange(0, options);
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectValuesJson))
            {
                var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
                _options = JsonConvert.DeserializeObject<List<OptionVM>>(SelectValuesJson, deserializeSettings);
                if (_options == null) throw new Exception("Cann't deserialize SelectValuesJson property");
                foreach (var opt in _options)
                {
                    if (!string.IsNullOrWhiteSpace(opt.TitleKey))
                        opt.Title = Controller.Localizer2Garin.Localize(opt.TitleKey);

                }
            }
            _fillSelected();
            return _options;
        }
    }

    public class FieldFile : Field<string>
    {
        public FieldFile(Controller2Garin controller, FieldBaseAttribute Attribute, string PropertyName) : base(controller, Attribute, PropertyName)
        {

        }
        /*public FieldFile(Controller2Garin controller, string title, string placeholder, EnumHtmlType htmlType, bool isRequired, bool isMultiple, string propertyName, int priority) 
            : base(controller, title, placeholder, htmlType, isRequired, false, isMultiple, propertyName, priority, null)
        {
        }*/
    }

    public class OptionVM
    {
        public string Value { get; set; }

        /// <summary>
        /// нужно тока для типа EnumHtmlType.Tree (нужно для оптимизации)
        /// </summary>
        //public Object ValueObj { get; set; }

        /// <summary>
        /// Ключ для локализации. Так как локализация нужна не всегда, то заполнить Title можно 2 способами
        /// </summary>
        public string TitleKey { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Тут планирую хранить чистое значение Title без префиксов SelectTreePrefix (заполняется поле тока для типа EnumHtmlType.Tree)
        /// </summary>
        public string OriginalTitle { get; set; }

        public string Parent { get; set; }

        public bool BSelected { get; set; }

        /// <summary>
        /// Если данная опция выбрана, то значение будет равно "selected"
        /// </summary>
        public string Selected => BSelected ? "selected" : null;

        // для сериализации
        public OptionVM() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="title"></param>
        /// <param name="parent"></param>
        /// <param name="selected"></param>
        public OptionVM(string value, string title, string parent, bool selected = false)
        {
            Value = value; Title = title; Parent = parent; BSelected = selected;
        }

        /// <summary>
        /// Конструктор для внутреннего использования в Expression (см ExpressionHelper)
        /// </summary>
        /// <param name="valueObj"></param>
        /// <param name="title"></param>
        /// <param name="parent"></param>
        public OptionVM(object valueObj, string title, object parent)
        {
            // здесь при ToStringVM null значения так и останутся null, а не "null", так и задумано!
            Value = valueObj?.GetType().ToStringVM(valueObj); Title = title; Parent = parent?.GetType().ToStringVM(parent);
        }
    }

}
