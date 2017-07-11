using AspNetCoreComponentLibrary.Abstractions;
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
    // ---------------- View Model
    public class SetupVM : BaseVM
    {
        public SetupVM() : base(null)
        {
            SEO = new SEO("Setup.");
        }

        public SetupIM Input { get; set; }
        //public Exception Error { get; set; }
    }

    // ---------------- Input Model
    public class SetupIM
    {
        [Display(Name = "Site name")]
        public string Site { get; set; }

        [Required(ErrorMessage = "The Host field is required.")]
        [Display(Name = "Host")]
        public string Host { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string Back { get; set; }

        public IActionResult ToActionResult(Controller controller, IStorage Storage, ILoggerFactory loggerFactory)
        {
            var Logger = loggerFactory.CreateLogger(this.GetType().FullName);
            //var LoggerMEF = loggerFactory.CreateLogger(Utils.MEFNameSpace);
            var Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);

            if (Sites.StartQuery(0).Any())
            {
                return new RedirectResult(Sites.CheckBackUrl(Back) ? Back : controller.Url.RouteUrl("Page"));
            }

            var vm = new SetupVM() { Input = this };
            if ( "POST".EqualsIC(controller.Request.Method) ) // сохранение данных
            {
                if (controller.ModelState.IsValid)
                {
                    Logger.LogInformation("Input valid");
                    var Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);
                    var UserSites = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);

                    try
                    {
                        // сохраняем данные
                        var site = new Sites() { Name = Site, Hosts = Host.ToLower(), IsVisible = true };
                        var user = new Users() { Email = Email.ToLower(), Password = Utils.CryptPassword(Password) };

                        //var us = new UserSites() { User = user, Site = site, IsAdmin = true };

                        //site.UserSites.Add(us);
                        //user.UserSites.Add(us);

                        site = Sites.Save(site); // БД для тока что созданного сайта будет создана автоматически после сохранения сайта
                        user = Users.Save(user);

                        var us = new UserSites() { UserId = user.Id, SiteId = site.Id, IsAdmin = true };
                        UserSites.Save(us);
                        Users.RemoveFromCache(user.Id);
                        //Storage.Save(EnumDB.UserSites);

                        //Users.AfterSave(user, true);
                        //Sites.AfterSave(site, true);

                        return new RedirectResult(Sites.CheckBackUrl(Back) ? Back : controller.Url.RouteUrl("Page"));
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
}
