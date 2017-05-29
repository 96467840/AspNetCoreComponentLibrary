using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{

    // ---------------- View Model
    public interface IListVM: IAdminVM
    {
        HtmlString GetH1();
        List<IBaseDM> Items { get; }
    }

    public class ListVM<K, T, R> : AdminVM, IListVM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T> /*where K : struct*/
    {
        public ListIM<K, T, R> Input { get; set; }
        public List<IBaseDM> Items { get; set; }

        public ListVM(ControllerEditable<K, T, R> controller) : base(controller)
        {
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
    }

    // ---------------- Input Model
    public class ListIM<K, T, R> : BaseIM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T>/* where K : struct*/
    {
        public int? Offset { get; set; }

        public virtual IActionResult ToActionResult(ControllerEditable<K, T, R> controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);

            var vm = new ListVM<K, T, R>(controller) { Input = this };

            if (!typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
            {
                vm.Items = controller.Sites.GetForUser(controller.SessionUser.Id).Select(i => (IBaseDM)i).ToList();
            }
            else
            {
                vm.Items = controller.Repository.GetForSite(controller.Site.Id).Select(i => (IBaseDM)i).ToList();
            }
            return controller.View("Admin/List", vm);
        }
    }

}
