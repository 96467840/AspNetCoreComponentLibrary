using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
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

    /// <summary>
    /// Атрибут для создания фильтра в админке
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
        /// <summary>
        /// Ключ локализации для лабела. Если не указано то будет использовано название поля.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Тип фильтра (допустимые значения: EnumHtmlType.Select, EnumHtmlType.Tree, EnumHtmlType.CheckBox, EnumHtmlType.Text)
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
    }
}
