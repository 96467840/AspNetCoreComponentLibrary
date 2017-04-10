using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class SetupIM
    {
        public string Site { get; set; }
        [Required]
        public string Host { get; set; }
        [Required(ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public IActionResult ToActionResult(Controller controller, IStorage Storage, ILoggerFactory loggerFactory)
        {
            var Logger = loggerFactory.CreateLogger(this.GetType().FullName);
            var vm = new SetupVM() { Input = this };
            if (controller.Request.Method.ToLower() == "post")
            {
                if (controller.ModelState.IsValid)
                {
                    Logger.LogInformation("Input valid");
                    var Sites = Storage.GetRepository<ISiteRepository>(false);
                    var Users = Storage.GetRepository<IUserRepository>(false);

                    try
                    {
                        // сохраняем данные
                        var site = new Sites() { Name = Site, Hosts = Host };
                        Sites.Save(site);
                        var user = new Users() { Email = Email, Password = Utils.CryptPassword(Password) };
                        Users.Save(user);

                        var us = new UserSites() { User = user, Site = site, IsAdmin = true };

                    }
                    catch (DbUpdateException ex)
                    {
                        vm.Error = ex;
                        //Log the error (uncomment ex variable name and write a log.
                        controller.ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists " +
                            "see your system administrator.");
                    }
                }
                else
                {
                    Logger.LogInformation("Input wrong");

                }
            }
            return controller.View(vm);
        }
    }

    public class SetupVM
    {
        public SetupIM Input { get; set; }
        public Exception Error { get; set; }

    }
}
