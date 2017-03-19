using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class TestComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string text)
        {
            var vm = new TestVM() { Text = text };
            return View(vm);
        }
    }
}
