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
        Dictionary<string, List<string>> Filter { get; }
        Type Type { get; }
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

        public Dictionary<string, List<string>> Filter => Input.Filter;

        public Type Type => typeof(T);
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
    }

}
