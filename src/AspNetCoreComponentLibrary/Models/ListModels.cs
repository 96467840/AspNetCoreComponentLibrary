﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{

    // ---------------- View Model
    public interface IListVM: IAdminVM
    {
        HtmlString GetH1();

    }

    public class ListVM<K, T> : AdminVM, IListVM where T : BaseDM<K> /*where K : struct*/
    {
        public ListIM<K, T> Input { get; set; }

        public ListVM(Controller2Garin controller) : base(controller)
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
    public class ListIM<K, T> : BaseIM where T : BaseDM<K>/* where K : struct*/
    {
        public int? Offset { get; set; }

        public virtual IActionResult ToActionResult(Controller2Garin controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);

            var vm = new ListVM<K, T>(controller) { Input = this };

            return controller.View("Admin/List", vm);
        }
    }

}
