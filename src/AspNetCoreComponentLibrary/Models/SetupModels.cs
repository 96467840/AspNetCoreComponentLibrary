using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class SetupIM
    {
        public string Site { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public IActionResult ToActionResult(Controller controller)
        {
            var vm = new SetupVM() { Input = this };
            if (controller.Request.Method == "post")
            {

            }
            return controller.View();
        }
    }

    public class SetupVM
    {
        public SetupIM Input { get; set; }
        public Exception Error { get; set; }

    }
}
