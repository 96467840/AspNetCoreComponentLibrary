using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCoreComponentLibrary
{

    // ---------------- View Model
    public interface IListVM: IAdminVM
    {
        HtmlString GetH1();
        List<IBaseDM> Items { get; }
        Dictionary<string, List<string>> FilterValues { get; }
        Type Type { get; }
        IEnumerable<FilterFieldVM> Filters{ get; }
    }

    public class ListVM<K, T, R> : AdminVM, IListVM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T> /*where K : struct*/
    {
        public ListIM<K, T, R> Input { get; set; }
        public List<IBaseDM> Items { get; set; }

        public ListVM(ControllerEditable<K, T, R> controller, ListIM<K, T, R> input) : base(controller)
        {
            Input = input;
            var href = controller.Url.RouteUrlWithCulture("Page", new { page = "index.html" });
            Breadcrumb.Items.Add(new MenuItem(href, controller.Localize("common.main")));

            href = controller.Url.RouteUrlWithCulture("Admin", new { controller = "sites" });
            Breadcrumb.Items.Add(new MenuItem(href, controller.Localize("common.admin")));

            Breadcrumb.Items.Add(new MenuItem(null, GetH1()));
        }

        public HtmlString GetH1()
        {
            return Controller.Localize(Controller.LocalizerPrefix + ".name");
        }

        public Dictionary<string, List<string>> FilterValues => Input.Filter;

        public Type Type => typeof(T);

        public IEnumerable<FilterFieldVM> Filters => Type.GetProperties().Select(i => new FilterFieldVM(
            (FilterAttribute)i.GetCustomAttribute(typeof(FilterAttribute)),
            i,
            FilterValues?.ContainsKey(i.Name) == true ? FilterValues[i.Name] : null,
            Controller.LocalizerPrefix
        )).Where(i => i.Attribute != null);
    }

    // ---------------- Input Model
    public class ListIM<K, T, R> : BaseIM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T>/* where K : struct*/
    {
        public int? Offset { get; set; }
        public Dictionary<string, List<string>> Filter { get; set; }

        public virtual IActionResult ToActionResult(ControllerEditable<K, T, R> controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);

            var vm = new ListVM<K, T, R>(controller, this);

            try
            {
                if (!typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
                {
                    // а юзеры тут могут быть?
                    vm.Items = controller.Sites.GetForUser(controller.SessionUser.Id).Select(i => (IBaseDM)i).ToList();
                }
                else
                {
                    using (new BLog(controller.LoggerMEF, "ListIM::ToActionResult", GetType().FullName))
                        vm.Items = controller.Repository.GetFiltered(controller.Site.Id, Filter).Select(i => (IBaseDM)i).ToList();
                }
            }
            catch (Exception e)
            {
                vm.Error = e;
            }
            return controller.View("Admin/List", vm);
        }
    }

    // ----------------- Filters
    public class FilterFieldVM
    {
        public FilterAttribute Attribute { get; set; }
        public PropertyInfo Property { get; set; }
        public List<string> Values { get; set; }
        public string LocalizerPrefix { get; set; }
        public ILocalizer2Garin Localizer { get; set; }

        public List<string> NameKeys
        {
            get
            {
                var res = new List<string>();
                if (!string.IsNullOrWhiteSpace(Attribute.Title)) res.Add(LocalizerPrefix + "." + Attribute.Title);
                res.Add(LocalizerPrefix + ".field." + Property.Name + ".title");
                res.Add("common.field." + Property.Name + ".title");
                return res;
            }
        }

        public List<string> PlaceholderKeys
        {
            get
            {
                var res = new List<string>();
                if (!string.IsNullOrWhiteSpace(Attribute.Placeholder)) res.Add(LocalizerPrefix + "." + Attribute.Placeholder);
                res.Add(LocalizerPrefix + ".field." + Property.Name + ".placeholder");
                res.Add("common.field." + Property.Name + ".placeholder");
                return res;
            }
        }

        public FilterFieldVM(FilterAttribute attribute, PropertyInfo property, List<string> values, string localizerPrefix)
        {
            Attribute = attribute;Property = property;Values = values;LocalizerPrefix = localizerPrefix;
        }
    }

}
