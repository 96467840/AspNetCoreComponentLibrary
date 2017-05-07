﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    // ---------------- View Model
    public interface IEditVM : IAdminVM
    {

    }

    public class EditVM<K, T> : AdminVM where T : BaseDM<K>/* where K : struct*/
    {
        public EditIM<K, T> Input { get; set; }

        public EditVM(Controller2Garin controller) : base(controller)
        {
        }
    }

    // ---------------- Input Model
    public class EditIM<K, T> where T : BaseDM<K>/* where K : struct*/
    {
        public virtual IActionResult ToActionResult(Controller2Garin controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);
            //var LoggerMEF = controller.LoggerFactory.CreateLogger(Utils.MEFNameSpace);
            var vm = new EditVM<K, T>(controller) { Input = this};

            return controller.View("Admin/Edit", vm);
        }
    }

}
