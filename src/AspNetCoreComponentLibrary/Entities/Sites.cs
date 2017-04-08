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
            //Menus = new HashSet<Menus>();
            //UserSites = new HashSet<UserSites>();
        }

        public bool IsDefault { get; set; }
        public bool IsVisible { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Url { get; set; }
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

        //public virtual ICollection<Menus> Menus { get; set; }
        public virtual ICollection<UserSites> UserSites { get; set; }
        //public virtual Menus E404page { get; set; }
        //public virtual Menus OrderPage { get; set; }
    }

}
