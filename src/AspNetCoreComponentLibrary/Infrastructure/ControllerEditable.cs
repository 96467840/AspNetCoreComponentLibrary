using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace AspNetCoreComponentLibrary
{
    public class ControllerEditable<K, T, R> : Controller2Garin where T : BaseDM<K>,IBaseDM where R:IRepositorySetStorageContext, IRepository<K, T>
    {
        //public T Item { get; set; }
        public R Repository { get; set; }

        // по умолчанию все сущности храним в БД контента сайта (кроме самого сайта и юзеров)
        // теперь получаем ссылку на репозиторий с контроллера через рефлексию, так что вызов конструктора не нужен
        //protected virtual EnumDB DB { get { return EnumDB.Content; } }

        public ControllerEditable(IControllerSettings settings) : base(settings)
        {
            Logger.LogTrace("Сonstructor ControllerEditable {0}", this.GetType().FullName);
            // здесь еще нет конекта к БД
            //Repository = Storage.GetRepository<R>(DB);

            // получить атрибуты контролера
            var type = GetType();
            var attr = (AdminControllerSettingsAttribute)type.GetTypeInfo().GetCustomAttribute(typeof(AdminControllerSettingsAttribute));
            if (attr != null && type.GetTypeInfo().IsClass)
            {
                LocalizerPrefix = attr.LocalizerPrefix;
            }
        }

        [NonAction]
        private void SetRepository()
        {
            // а вот тут оптимизировать низя! не у всех свойств есть атрибуты
            foreach (var t in GetType().GetProperties())
            {
                var attr = t.GetCustomAttribute(typeof(RepositorySettingsAttribute));
                if (attr != null)
                {
                    if (t.PropertyType.FullName == typeof(R).FullName)
                    {
                        Repository = (R)t.GetValue(this);
                        if (Repository == null)
                            throw new Exception("Требуемый для контроллера репозиторий не загружен в родительском контроллере.");
                        return;
                    }
                }
            }
            throw new Exception("Требуемый для контроллера репозиторий не прописан в родительском контроллере.");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Logger.LogTrace("ControllerEditable OnActionExecuting");
            base.OnActionExecuting(context);

            // не будем дублировать! все репозитории определены в Controller2Garin и получим нужный нам через рефлексию
            //Repository = Storage.GetRepository<R>(DB);
            SetRepository();
        }

        public virtual IActionResult Edit(EditIM<K, T, R> input)
        {
            Logger.LogTrace("Edit");
            return input.ToActionResult(this);
        }

        public virtual IActionResult List(ListIM<K, T, R> input)
        {
            Logger.LogTrace("List");
            return input.ToActionResult(this);
        }

        /*public virtual IActionResult Block(K index)
        {
            Logger.LogTrace("Block");
            return input.ToActionResult(this);
        }*/

    }
}
