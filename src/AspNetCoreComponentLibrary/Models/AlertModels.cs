using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public enum EnumAlertType
    {
        danger,
        info,
        message,
        success
    }

    public class AlertVM
    {
        public EnumAlertType Type { get; set; }
        public HtmlString Body { get; set; }
        public bool IsDismissible { get; set; }

        public AlertVM(HtmlString body, EnumAlertType type = EnumAlertType.danger, bool isDismissble=true)
        {
            Type = type; Body = body; IsDismissible = isDismissble;
        }

        public AlertVM(string body, EnumAlertType type = EnumAlertType.danger, bool isDismissble = true)
        {
            Type = type; Body = new HtmlString(body); IsDismissible = isDismissble;
        }
    }
}
