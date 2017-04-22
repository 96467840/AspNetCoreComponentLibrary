using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreComponentLibrary
{
    class ControllerEditable<K, T, R> : Controller2Garin where T : BaseDM<K> where K : struct where R:IRepositorySetStorageContext
    {
        //public T Item { get; set; }
        protected R Repository { get; set; }

        // по умолчанию все сущности храним в БД контента сайта (кроме самого сайта и юзеров)
        protected virtual EnumDB DB { get { return EnumDB.Content; } }

        public ControllerEditable(IStorage storage, ILoggerFactory loggerFactory) : base(storage, loggerFactory)
        {
            Repository = Storage.GetRepository<R>(DB);
        }

        public virtual IActionResult Edit(EditIM<K, T> input)
        {
            return input.ToActionResult(this);
        }

        public virtual IActionResult List(ListIM<K, T> input)
        {
            return input.ToActionResult(this);
        }

    }
}
