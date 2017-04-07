using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class UserSites
    {
        public long UserId { get; set; }
        public long SiteId { get; set; }
        public string Rights { get; set; }

        public virtual Sites Site { get; set; }
        public virtual Users User { get; set; }

    }
}
