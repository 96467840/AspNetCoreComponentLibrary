using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    [EntitySettings(LocalizerPrefix = "news")]
    public partial class News : BaseDM<long>, IBlockable, IWithSiteId
    {
        [Field(HtmlType = EnumHtmlType.Hidden)]
        public long SiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public bool IsBlocked { get; set; }
        public bool InWww { get; set; }
        public bool InVk { get; set; }
        public bool InFb { get; set; }
        public string Page { get; set; }
        public string Image { get; set; }
        public string Search { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public string ExternalId { get; set; }
    }
}
