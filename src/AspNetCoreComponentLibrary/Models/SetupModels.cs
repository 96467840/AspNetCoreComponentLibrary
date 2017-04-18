﻿using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public string Back { get; set; }

        public IActionResult ToActionResult(Controller controller, IStorage Storage, ILoggerFactory loggerFactory)
        {
            var Logger = loggerFactory.CreateLogger(this.GetType().FullName);
            var Sites = Storage.GetRepository<ISiteRepository>(false);

            if (Sites.StartQuery().Any())
            {
                return new RedirectResult(Utils.CheckBackUrl(Sites, Back) ? Back : controller.Url.RouteUrl("Page"));
            }

            var vm = new SetupVM() { Input = this };
            if (controller.Request.Method.ToLower() == "post") // сохранение данных
            {
                if (controller.ModelState.IsValid)
                {
                    Logger.LogInformation("Input valid");
                    var Users = Storage.GetRepository<IUserRepository>(false);

                    try
                    {
                        // сохраняем данные
                        var site = new Sites() { Name = Site, Hosts = Host.ToLower(), IsVisible = true };
                        var user = new Users() { Email = Email.ToLower(), Password = Utils.CryptPassword(Password) };

                        var us = new UserSites() { User = user, Site = site, IsAdmin = true };

                        site.UserSites.Add(us);
                        user.UserSites.Add(us);

                        Sites.Save(site);
                        Users.Save(user);

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
                    controller.ModelState.AddModelError("", "Input wrong");
                    Logger.LogInformation("Input wrong");

                }
            }
            else // чтение, инициализация данных
            {
                Host = controller.HttpContext.Request.Host.Host.ToLower();
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
