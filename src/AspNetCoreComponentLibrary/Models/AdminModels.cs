using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace AspNetCoreComponentLibrary
{
    public interface IAdminVM : IBaseVM
    {
        MenuVM LeftMenu { get; }
    }

    public class AdminVM : BaseVM
    {
        public AdminVM(Controller2Garin controller) : base(controller)
        {
            if (!CheckRights())
            {
                throw new Exception(controller.Localize("common").ToString());
            }

        }

        private MenuVM _LeftMenu { get; set; }
        public MenuVM LeftMenu
        {
            get
            {
                if (_LeftMenu != null) return _LeftMenu;
                var items = new List<MenuItem>();
                var current = Controller.GetType().FullName;

                foreach (Type type in Controller.GetType().GetTypeInfo().Assembly.GetTypes())
                {
                    var attr = (AdminControllerSettingsAttribute)type.GetTypeInfo().GetCustomAttribute(typeof(AdminControllerSettingsAttribute));
                    if (attr != null && type.GetTypeInfo().IsClass)
                    {
                        var tmp = type.FullName.Explode(".");
                        if (!tmp.Any()) continue;
                        var controllerName = Regex.Replace(tmp.Last(), "Controller$", "");
                        var href = Controller.Url.RouteUrlWithCulture("Admin", new { Controller = controllerName, Action = "List" });
                        var title = Controller.Localize(attr.LocalizerPrefix + ".name");
                        var priority = attr.Priority;
                        var liClass = string.Empty;
                        if (current == type.FullName)
                        {
                            liClass = "active";
                        }
                        items.Add(new MenuItem(href, title, liClass) { Priority = priority });
                    }
                }

                _LeftMenu = new MenuVM(items, null);
                _LeftMenu.SortMenu();
                return _LeftMenu;
            }
        }

        protected bool CheckRights()
        {
            return true;
        }
    }
}
