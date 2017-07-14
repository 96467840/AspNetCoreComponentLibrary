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
    public interface IListVM : IAdminVM
    {
        HtmlString GetH1();
        List<IBaseDM> Items { get; }
        //Dictionary<string, List<string>> FilterValues { get; }
        Type Type { get; }
        //IEnumerable<FilterFieldVM> Filters{ get; }
        Form Form { get; }
        string LocalizeFieldKey(string propertyName, string key, bool anyProperty = true);
    }

    public class ListVM<K, T, R> : AdminVM, IListVM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T> /*where K : struct*/
    {
        public ListIM<K, T, R> Input { get; set; }
        public List<IBaseDM> Items { get; set; }
        
        public ListVM(ControllerEditable<K, T, R> controller, ListIM<K, T, R> input) : base(controller)
        {
            Input = input;
            string href;
            var title = controller.Localizer2Garin.Localize("common.breadcrumb.main");
            if (title != "")
            {
                href = controller.Url.RouteUrlWithCulture("Page", new { page = "index.html" });
                Breadcrumb.Items.Add(new MenuItem(href, new HtmlString(title)));
            }

            href = controller.Url.RouteUrlWithCulture("Admin", new { controller = "sites" });
            Breadcrumb.Items.Add(new MenuItem(href, controller.Localizer2Garin.LocalizeHtml("common.breadcrumb.admin")));

            Breadcrumb.Items.Add(new MenuItem(null, GetH1()));

        }

        public HtmlString GetH1()
        {
            return Controller.Localizer2Garin.LocalizeHtml(Controller.LocalizerPrefix + ".name");
        }

        public string LocalizeFieldKey(string propertyName, string key, bool anyProperty = true)
        {
            return Controller.Localizer2Garin.Localize(Utils.GenLocalizeKeysList(Controller.LocalizerPrefix, null, propertyName, key, anyProperty));
        }

        public Type Type => typeof(T);

        private Form _form;
        public Form Form
        {
            get
            {
                if (_form == null)
                {
                    _form = typeof(T).GetForm<FilterAttribute>(Controller);//new Form(Controller, typeof(T), "filter");
                    _form.Load(Controller.SiteLanguages);
                }
                return _form;
            }
        }
    }

    // ---------------- Input Model
    public class ListIM<K, T, R> : BaseIM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T>/* where K : struct*/
    {
        public int? Offset { get; set; }
        //public Dictionary<string, List<string>> Filter { get; set; }

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
                        vm.Items = controller.Repository.GetFiltered(controller.Site.Id, vm.Form).Take(30).Select(i => (IBaseDM)i).ToList();
                }
            }
            catch (Exception e)
            {
                vm.Error = e;
            }
            var tmp = vm.Form;
            return controller.View("Admin/List", vm);
        }
    }

}
