using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.Encodings.Web;
using System.Text.Unicode;
//using System.Web;

namespace AspNetCoreComponentLibrary
{
    
    public static class HtmlUtility
    {
        private static readonly Dictionary<string, string[]> ValidHtmlTags = new Dictionary<string, string[]>
        {
            {"p", new []          {"style", "class", "align"}},
            {"div", new []        {"style", "class", "align"}},
            {"span", new []       {"style", "class"}},
            {"br", new []         {"style", "class"}},
            {"hr", new []         {"style", "class"}},
            {"label", new []      {"style", "class"}},

            {"h1", new []         {"style", "class"}},
            {"h2", new []         {"style", "class"}},
            {"h3", new []         {"style", "class"}},
            {"h4", new []         {"style", "class"}},
            {"h5", new []         {"style", "class"}},
            {"h6", new []         {"style", "class"}},

            {"font", new []       {"style", "class", "color", "face", "size"}},
            {"strong", new []     {"style", "class"}},
            {"b", new []          {"style", "class"}},
            {"em", new []         {"style", "class"}},
            {"i", new []          {"style", "class"}},
            {"u", new []          {"style", "class"}},
            {"strike", new []     {"style", "class"}},
            {"ol", new []         {"style", "class"}},
            {"ul", new []         {"style", "class"}},
            {"li", new []         {"style", "class"}},
            {"blockquote", new [] {"style", "class"}},
            {"code", new []       {"style", "class"}},
            {"pre", new []       {"style", "class"}},

            {"a", new []          {"style", "class", "href", "title", "rel", "target"}},
            {"img", new []        {"style", "class", "src", "height", "width", "alt", "title", "hspace", "vspace", "border"}},

            {"table", new []      {"style", "class"}},
            {"thead", new []      {"style", "class"}},
            {"tbody", new []      {"style", "class"}},
            {"tfoot", new []      {"style", "class"}},
            {"th", new []         {"style", "class", "scope"}},
            {"tr", new []         {"style", "class"}},
            {"td", new []         {"style", "class", "colspan"}},

            {"q", new []          {"style", "class", "cite"}},
            {"cite", new []       {"style", "class"}},
            {"abbr", new []       {"style", "class"}},
            {"acronym", new []    {"style", "class"}},
            {"del", new []        {"style", "class"}},
            {"ins", new []        {"style", "class"}}
        };

        public static string SanitizeHtml(string source)
        {
            var enc = GetHtmlEncoder();
            if (string.IsNullOrWhiteSpace(source)) return "";
            var html = GetHtml(source);
            if (html == null) return string.Empty;
            var allNodes = html.DocumentNode;
            var whitelist = (from kv in ValidHtmlTags select kv.Key).ToArray();
            CleanNodes(allNodes, whitelist);
            foreach (var tag in ValidHtmlTags)
            {
                var key = tag.Key;
                var nodes = (from n in allNodes.DescendantsAndSelf() where n.Name == key select n);

                foreach (var a in from n in nodes where n.HasAttributes select n.Attributes.ToArray() into attr from a in attr select a)
                {
                    if (a.Name.StartsWith("data-"))
                    {
                        a.Value = enc.Encode(a.Value);// HttpUtility.HtmlAttributeEncode(a.Value);
                    }
                    else if (!tag.Value.Contains(a.Name))
                    {
                        a.Remove();
                    }
                    else
                    {
                        switch (a.Name)
                        {
                            case "src":
                            case "href":
                                a.Value = (!string.IsNullOrEmpty(a.Value)) ? a.Value.Replace("\r", "").Replace("\n", "") : "";
                                a.Value = (!string.IsNullOrEmpty(a.Value) && (a.Value.IndexOf("javascript", StringComparison.CurrentCultureIgnoreCase) < 10 || a.Value.IndexOf("eval", StringComparison.CurrentCultureIgnoreCase) < 10)) ? a.Value.Replace("javascript", "").Replace("eval", "") : a.Value;
                                if (a.Name == "href")
                                {
                                    //a.OwnerNode.Attributes.Add(attrrel);
                                    a.OwnerNode.SetAttributeValue("rel", "nofollow");
                                    a.OwnerNode.SetAttributeValue("target", "_blank");
                                }
                                break;
                            //case "style":
                            //case "class":
                            //    a.Value = System.Web.Security.AntiXss.AntiXssEncoder.CssEncode(a.Value);
                            //	  break;
                            default:
                                a.Value = enc.Encode(a.Value);//HttpUtility.HtmlAttributeEncode(a.Value);
                                break;
                        }
                    }
                }
            }

            return allNodes.InnerHtml;
        }

        public static HtmlEncoder _enc;
        public static HtmlEncoder GetHtmlEncoder()
        {
            if (_enc == null)
                _enc = HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All });
            return _enc;
        }


        public static string StripHtml(string source)
        {
            source = SanitizeHtml(source);

            if (string.IsNullOrEmpty(source)) return string.Empty;

            var html = GetHtml(source);
            var result = new StringBuilder();

            foreach (var node in html.DocumentNode.ChildNodes)
                result.Append(node.InnerText);

            return result.ToString();
        }

        private static void CleanNodes(HtmlNode node, string[] whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                if (!whitelist.Contains(node.Name))
                {
                    node.ParentNode.RemoveChild(node);
                    return; // We're done
                }
            }

            if (node.HasChildNodes) CleanChildren(node, whitelist);
        }

        private static void CleanChildren(HtmlNode parent, string[] whitelist)
        {
            for (var i = parent.ChildNodes.Count - 1; i >= 0; i--) CleanNodes(parent.ChildNodes[i], whitelist);
        }

        private static HtmlDocument GetHtml(string source)
        {
            var enc = GetHtmlEncoder();
            var html = new HtmlDocument
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };

            html.LoadHtml(source);

            foreach (var n in html.DocumentNode.DescendantsAndSelf())
            {
                if (n.Name != "code") continue;
                var attr = n.Attributes.ToArray();
                foreach (var a in attr.Where(a => a.Name != "style" && a.Name != "class")) a.Remove();
                n.InnerHtml = enc.Encode(n.InnerHtml);//HttpUtility.HtmlEncode(n.InnerHtml);
            }

            return html;
        }
    }
    /**/
}
