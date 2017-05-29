using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    // ---------------- View Model
    public interface IEditVM : IAdminVM
    {

    }

    public class EditVM<K, T, R> : AdminVM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T>/* where K : struct*/
    {
        public EditIM<K, T, R> Input { get; set; }

        public EditVM(ControllerEditable<K, T, R> controller) : base(controller)
        {
        }
    }

    // ---------------- Input Model
    public class EditIM<K, T, R> : BaseIM where T : BaseDM<K>, IBaseDM where R : IRepositorySetStorageContext, IRepository<K, T>/* where K : struct*/
    {
        public virtual IActionResult ToActionResult(ControllerEditable<K, T, R> controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);
            //var LoggerMEF = controller.LoggerFactory.CreateLogger(Utils.MEFNameSpace);

            var vm = new EditVM<K, T, R>(controller) { Input = this };

            return controller.View("Admin/Edit", vm);
        }
    }

}
