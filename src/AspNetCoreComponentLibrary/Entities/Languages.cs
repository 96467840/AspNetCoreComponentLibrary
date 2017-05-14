using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public partial class Languages : BaseDM<long>, IBlockable
    {
        public long SiteId { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public bool IsDefault { get; set; }
        public bool IsBlocked { get; set; }
        public string Json { get; set; }
        public string ExternalId { get; set; }
    }
}
