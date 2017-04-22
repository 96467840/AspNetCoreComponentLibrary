using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class ListIM<K, T> where T : BaseDM<K> where K : struct
    {
        public virtual IActionResult ToActionResult(Controller2Garin controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);
            var vm = new ListVM<K, T>(controller) { Input = this };

            return controller.View(vm);
        }
    }

    public class ListVM<K, T> : BaseVM where T : BaseDM<K> where K : struct
    {
        public ListIM<K, T> Input { get; set; }

        public ListVM(Controller2Garin controller) : base(controller)
        {
        }
    }
}
