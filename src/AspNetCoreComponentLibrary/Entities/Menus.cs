using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    [EntitySettings(LocalizerPrefix="menus")]
    public partial class Menus : BaseDM<long>, IBlockable, ISortable, IWithSiteId
    {
        public Menus()
        {
            //SitesE404page = new HashSet<Sites>();
            //SitesOrderPage = new HashSet<Sites>();
        }

        // не редактируемое поле
        public DateTime Created { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Hidden)]
        public long SiteId { get; set; }

        [Filter(HtmlType = EnumHtmlType.Tree, SelectRepository = typeof(IMenuRepository), SelectKeyName = "Id", SelectValueName ="Name")]
        [FieldSettings(HtmlType = EnumHtmlType.Tree, SelectKeyName = "Id", SelectValueName = "Name")]
        public long? ParentId { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string Page { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string MenuName { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string Name { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public int Priority { get; set; }

        [Filter(HtmlType = EnumHtmlType.CheckBox)]
        public bool IsBlocked { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowSubmenu { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowInTop { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowInBottom { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowInMenu { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.TextArea)]
        public string Content { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string SeoTitle { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string SeoKeywords { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string SeoDescription { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string Url { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string Layout { get; set; }

        //public long? ArticlesId { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool InWww { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool InVk { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool InFb { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowNews { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowLeftMenu { get; set; }

        public string Lang { get; set; }
        public string Css { get; set; }
        public string BodyCss { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Image)]
        public string ImageLogo { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Image)]
        public string ImageLogoRight { get; set; }

        public string Search { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string Description { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool IsSeparatedFaqs { get; set; }

        public string ExternalId { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Image)]
        public string ImageOnMain { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.CheckBox)]
        public bool IsShowOnMain { get; set; }

        [FieldSettings(HtmlType = EnumHtmlType.Text)]
        public string ShortContent { get; set; }

        //public virtual ICollection<Sites> SitesE404page { get; set; }
        //public virtual ICollection<Sites> SitesOrderPage { get; set; }
        public virtual Menus Parent { get; set; }
        public virtual ICollection<Menus> InverseParent { get; set; }
        //public virtual Sites Site { get; set; }

    }
}
