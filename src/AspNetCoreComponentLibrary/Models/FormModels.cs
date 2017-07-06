using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Form
    {
        public Controller2Garin Controller { get; set; }
        // с контролера возмем
        //public HttpRequest Request { get; set; }

        public List<IField> Fields { get; set; }
        // наша форма не всегда привязана к итему! например формы с БД (контакты)
        //public Type Type { get; set; }

        public Dictionary<string, string> Errors { get; set; }

        public Form(Controller2Garin controller)
        {
            Controller = controller;
            Fields = new List<IField>();
        }

        public Form(Controller2Garin controller, List<IField> fields)
        {
            Controller = controller;
            Fields = fields; 
        }

        //public Form(Controller2Garin controller, Dictionary<string, List<string>> values, Type EntityType, string KeysPrefix="form")
        //{
        //    Values = values; Controller = controller;
        //    Fields = new List<IField>();

        //    //EntityType.GetProperties().Select(i => new FilterFieldVM(
        //    //    (FilterAttribute)i.GetCustomAttribute(typeof(FilterAttribute)),
        //    //    i,
        //    //    FilterValues?.ContainsKey(i.Name) == true ? FilterValues[i.Name] : null,
        //    //    Controller
        //    //)).Where(i => i.Attribute != null);
        //}

        public void Load(List<Languages> languages)
        {
            foreach (var field in Fields)
            {
                field.Load(languages);
            }
        }

        public bool Check()
        {
            Errors = new Dictionary<string, string>();
            foreach (var field in Fields)
            {
                var err = field.Check();
                if (!string.IsNullOrWhiteSpace(err)) Errors[field.PropertyName] = err;
            }
            return !Errors.Any();
        }
    }

}
