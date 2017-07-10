using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    [EntitySettings(LocalizerPrefix = "menus")]
    public partial class Menus : BaseDM<long>, IBlockable, ISortable, IWithSiteId, IWithPage
    {
        public Menus()
        {
            //SitesE404page = new HashSet<Sites>();
            //SitesOrderPage = new HashSet<Sites>();
        }

        // не редактируемое поле
        public DateTime Created { get; set; }

        [Field(HtmlType = EnumHtmlType.Hidden)]
        public long SiteId { get; set; }

        [Filter(HtmlType = EnumHtmlType.Tree, SelectRepository = typeof(IMenuRepository), SelectValueName = "Id", SelectTitleName = "MenuName", SelectParentName ="ParentId")]
        [Field(HtmlType = EnumHtmlType.Tree, SelectValueName = "Id", SelectTitleName = "Name")]
        public long? ParentId { get; set; }

        [Field(HtmlType = EnumHtmlType.Text)]
        public string Page { get; set; }

        [Field(HtmlType = EnumHtmlType.Text, NeedTranslate = true)]
        [OrderBy(Priority = 20)]
        public string MenuName { get; set; }

        [Field(HtmlType = EnumHtmlType.Text, NeedTranslate = true)]
        public string Name { get; set; }

        [Field(HtmlType = EnumHtmlType.Text)]
        [OrderBy(Priority = 10)]
        public int Priority { get; set; }

        [Filter(HtmlType = EnumHtmlType.Select/*, SelectValuesJson = "[{Value:'',TitleKey:'common.all'},{Value:'True',TitleKey:'common.yes'},{Value:'False',TitleKey:'common.no'}]"*/)]
        public bool IsBlocked { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowSubmenu { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowInTop { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowInBottom { get; set; }

        [Filter(HtmlType = EnumHtmlType.Select)]
        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool? ShowInMenu { get; set; }

        [Field(HtmlType = EnumHtmlType.TextArea, NeedTranslate = true)]
        public string ShortContent { get; set; }

        [Field(HtmlType = EnumHtmlType.TextArea, NeedTranslate = true)]
        public string Content { get; set; }

        [Field(HtmlType = EnumHtmlType.Text, NeedTranslate = true)]
        public string SeoTitle { get; set; }

        [Field(HtmlType = EnumHtmlType.Text, NeedTranslate = true)]
        public string SeoKeywords { get; set; }

        [Field(HtmlType = EnumHtmlType.Text, NeedTranslate = true)]
        public string SeoDescription { get; set; }

        [Field(HtmlType = EnumHtmlType.Text)]
        public string Url { get; set; }

        public string Layout { get; set; }

        //public long? ArticlesId { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool InWww { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool InVk { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool InFb { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowNews { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool ShowLeftMenu { get; set; }

        public string Lang { get; set; }
        public string Css { get; set; }
        public string BodyCss { get; set; }

        [Field(HtmlType = EnumHtmlType.Image)]
        public string ImageLogo { get; set; }

        [Field(HtmlType = EnumHtmlType.Image)]
        public string ImageLogoRight { get; set; }

        public string Search { get; set; }

        [Field(HtmlType = EnumHtmlType.Text, NeedTranslate = true)]
        public string Description { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool IsSeparatedFaqs { get; set; }

        public string ExternalId { get; set; }

        [Field(HtmlType = EnumHtmlType.Image)]
        public string ImageOnMain { get; set; }

        [Field(HtmlType = EnumHtmlType.CheckBox)]
        public bool IsShowOnMain { get; set; }


        //public virtual ICollection<Sites> SitesE404page { get; set; }
        //public virtual ICollection<Sites> SitesOrderPage { get; set; }
        public virtual Menus Parent { get; set; }
        public virtual ICollection<Menus> InverseParent { get; set; }
        //public virtual Sites Site { get; set; }

    }
}
