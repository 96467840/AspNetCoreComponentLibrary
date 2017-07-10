using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class FieldBaseAttribute : Attribute
    {
        /// <summary>
        /// Тип фильтра (допустимые значения: EnumHtmlType.Select, EnumHtmlType.Tree, EnumHtmlType.CheckBox, EnumHtmlType.Text)
        /// </summary>
        public EnumHtmlType HtmlType { get; set; }

        /// <summary>
        /// Тип сравнения. В основном для текстовых полей
        /// </summary>
        public EnumFilterCompare Compare { get; set; }

        /// <summary>
        /// Префикс для локализации. Знаечние "field" указывать не надо.
        /// Перевод будет искаться в следующем порядке:
        /// <list type="number">
        /// <item>Controller.LocalizerPrefix + "." + LocalizePrefix + "." + Property.Name + ".title"</item>
        /// <item>"common" + "." + LocalizePrefix + "." + Property.Name + ".title"</item>
        /// <item>Controller.LocalizerPrefix + ".field." + Property.Name + ".title"</item>
        /// <item>"common.field." + Property.Name + ".title"</item>
        /// </list>
        /// See <see cref="Utils.GenLocalizeKeysList(string, string, string, string, bool)" /> 
        /// </summary>
        public virtual string LocalizePrefix => null;

        /// <summary>
        /// Поле с переводом
        /// </summary>
        public bool NeedTranslate { get; set; }

        /// <summary>
        /// Возможене множественный выбор
        /// </summary>
        public bool IsMultiple { get; set; }

        /// <summary>
        /// Поле обязательно для заполнения
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Переопределение стандартного формата
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Значение для чекбоксов и радио. Эта опция лишняя. Для типа bool это всегда true, для числа 1
        /// </summary>
        //public string Set { get; set; }

        /// <summary>
        /// Порядковый номер в форме.
        /// </summary>
        public int Priority { get; set; }

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

        /// <summary>
        /// Заполнять только разблокированными знаечниями
        /// </summary>
        public bool SelectOnlyUnblocked { get; set; }

        /// <summary>
        /// Отступ слева для дочерних узлов (для типа EnumHtmlType.Tree) Если не указано, то будет использоватся "&nbsp;&nbsp;&nbsp;&nbsp;"
        /// </summary>
        public string SelectTreePrefix { get; set; }
    }

    public enum EnumHtmlType
    {
        CheckBox = 1,
        Radio = 2,
        Text = 3,
        Select = 4,
        /// <summary>
        /// Дерево значений
        /// </summary>
        Tree = 5,
        TextArea = 6,
        File = 7,
        Files = 8,
        Hidden = 9,
        Image = 10,
        Images = 11,
        /// <summary>
        /// Что то типа справочника ключ -> значение, хранимое в json формате
        /// </summary>
        Json = 12
    }

    public enum EnumFilterCompare
    {
        Equals, Include, Ends, Begins, LT, GT, LTE, GTE
    }
}
