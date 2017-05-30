using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    [EntitySettings(LocalizerPrefix = "users")]
    public partial class Users : BaseDM<long>
    {
        public Users()
        {
            UserSites = new HashSet<UserSites>();
        }

        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string VkId { get; set; }
        public string VkToken { get; set; }
        public string VkExpires { get; set; }
        public string FbId { get; set; }
        public string FbToken { get; set; }
        public string FbExpires { get; set; }

        public virtual ICollection<UserSites> UserSites { get; set; }
    }
}
