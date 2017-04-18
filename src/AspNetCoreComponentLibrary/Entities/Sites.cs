using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Sites : BaseDM<long>
    {
        public Sites()
        {
            UserSites = new HashSet<UserSites>();
        }

        public bool IsDefault { get; set; }
        public bool IsVisible { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Hosts { get; set; }
        public string Description { get; set; }
        public string Layout { get; set; }
        public string Contacts { get; set; }
        public string ContactsShort { get; set; }
        public string YandexMetrika { get; set; }
        public string GoogleanAlytics { get; set; }
        public bool Share42 { get; set; }
        public bool IsDeleted { get; set; }
        public string VkAppId { get; set; }
        public string VkAppSecret { get; set; }
        public string FbAppId { get; set; }
        public string FbAppSecret { get; set; }
        public string FbNamespace { get; set; }
        public string RecaptchaSiteKey { get; set; }
        public string RecaptchaSecretKey { get; set; }
        public string Template { get; set; }
        public string ExternalId { get; set; }
        public string Favicon { get; set; }
        public long? OrderPageId { get; set; }
        public long? E404pageId { get; set; }

        public virtual ICollection<UserSites> UserSites { get; set; }

        #region Fill List of Hosts
        // не очень удачное решение. удачно если репозиторий сайтов будет в кешах. если без кешей, то кабздец
        // но тут в любом случае без кешей будет жопа, так как возможны "*"
        // в целях оптимизации * разрешены тока вначале и тока в виде полного поддомена "*.example.com"
        private List<string> _ListHosts;
        private List<string> _ListHostsWithAsteriks;
        private void _fillHosts()
        {
            var tmp = Hosts.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(i => !string.IsNullOrWhiteSpace(i));

            _ListHosts = tmp.Where(i => i.IndexOf("*") < 0).Select(i => i.Trim()).ToList();
            _ListHostsWithAsteriks = tmp.Where(i => i.IndexOf("*") == 0).Select(i => i.Trim().TrimStart(new[] { '*' })).ToList();
        }
        public List<string> ListHosts
        {
            get
            {
                if (_ListHosts != null) return _ListHosts;

                _fillHosts();

                return _ListHosts;
            }
        }
        public List<string> ListHostsWithAsteriks
        {
            get
            {
                if (_ListHostsWithAsteriks != null) return _ListHostsWithAsteriks;

                _fillHosts();

                return _ListHostsWithAsteriks;
            }
        }
        # endregion

        public bool TestHost(string host) {
            if (ListHosts.Any(i => i == host)) return true;
            if (ListHostsWithAsteriks.Any(i => host.EndsWith(host))) return true;
            return false;
        }

    }

}
