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
            //if ()
        }

        private MenuVM _LeftMenu { get; set; }
        public MenuVM LeftMenu
        {
            get
            {
                if (_LeftMenu != null) return _LeftMenu;
                var items = new List<MenuItem>();

                foreach (Type type in Controller.GetType().GetTypeInfo().Assembly.GetTypes())
                {
                    var attr = (AdminControllerSettingsAttribute)type.GetTypeInfo().GetCustomAttribute(typeof(AdminControllerSettingsAttribute));
                    if (attr != null && type.GetTypeInfo().IsClass)
                    {
                        var tmp = type.FullName.Explode(".");
                        if (!tmp.Any()) continue;
                        var controllerName = Regex.Replace(tmp.Last(), "Controller$", "");
                        var href = Controller.Url.RouteUrlWithCulture("Admin", new { Controller = controllerName, Action = "List" });
                        var title = Controller.SharedLocalizer[attr.MenuName];// tmp.Last();
                        var priority = attr.Priority;
                        items.Add(new MenuItem(href, title));
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
