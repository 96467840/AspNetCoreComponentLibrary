using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class Menus : BaseDM<long>, IBlockable, ISortable
    {
        public Menus()
        {
            //SitesE404page = new HashSet<Sites>();
            //SitesOrderPage = new HashSet<Sites>();
        }

        public DateTime Created { get; set; }
        public long SiteId { get; set; }
        public long? ParentId { get; set; }
        public string Page { get; set; }
        public string MenuName { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsBlocked { get; set; }
        public bool ShowSubmenu { get; set; }
        public bool ShowInTop { get; set; }
        public bool ShowInBottom { get; set; }
        public bool ShowInMenu { get; set; }
        public string Content { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public string Url { get; set; }
        public string Layout { get; set; }
        //public long? ArticlesId { get; set; }
        public bool InWww { get; set; }
        public bool InVk { get; set; }
        public bool InFb { get; set; }
        public bool ShowNews { get; set; }
        public bool ShowLeftMenu { get; set; }
        public string Lang { get; set; }
        public string Css { get; set; }
        public string BodyCss { get; set; }
        public string ImageLogo { get; set; }
        public string ImageLogoRight { get; set; }
        public string Search { get; set; }
        public string Description { get; set; }
        public bool IsSeparatedFaqs { get; set; }
        public string ExternalId { get; set; }
        public string ImageOnMain { get; set; }
        public bool IsShowOnMain { get; set; }
        public string ShortContent { get; set; }

        //public virtual ICollection<Sites> SitesE404page { get; set; }
        //public virtual ICollection<Sites> SitesOrderPage { get; set; }
        public virtual Menus Parent { get; set; }
        public virtual ICollection<Menus> InverseParent { get; set; }
        //public virtual Sites Site { get; set; }

    }
}
