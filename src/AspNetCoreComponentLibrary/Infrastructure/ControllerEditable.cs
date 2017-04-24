using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreComponentLibrary
{
    public class ControllerEditable<K, T, R> : Controller2Garin where T : BaseDM<K> where R:IRepositorySetStorageContext
    {
        //public T Item { get; set; }
        public R Repository { get; set; }

        // по умолчанию все сущности храним в БД контента сайта (кроме самого сайта и юзеров)
        protected virtual EnumDB DB { get { return EnumDB.Content; } }

        public ControllerEditable(IStorage storage, ILoggerFactory loggerFactory) : base(storage, loggerFactory)
        {
            Logger.LogTrace("Сonstructor");
            // здесь еще нет конекта к БД
            //Repository = Storage.GetRepository<R>(DB);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Logger.LogTrace("OnActionExecuting");
            base.OnActionExecuting(context);
            Repository = Storage.GetRepository<R>(DB);
        }

        public virtual IActionResult Edit(EditIM<K, T> input)
        {
            Logger.LogTrace("Edit");
            return input.ToActionResult(this);
        }

        public virtual IActionResult List(ListIM<K, T> input)
        {
            Logger.LogTrace("List");
            return input.ToActionResult(this);
        }

    }
}
