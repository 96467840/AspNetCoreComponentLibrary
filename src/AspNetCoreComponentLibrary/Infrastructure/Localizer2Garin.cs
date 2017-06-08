using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Localizer2Garin: ILocalizer2Garin
    {
        private readonly ILoggerFactory LoggerFactory;
        private readonly IStringLocalizerFactory LocalizerFactory;
        private IStringLocalizer Localizer;
        public string DefaultCulture { get; set; }
        private readonly ILogger Logger;


        public Localizer2Garin(ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory, IStringLocalizer localizer, IOptions<LocalizerConfigure> LocalizerOptionsAccessor)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            LocalizerFactory = localizerFactory;
            //Localizer = localizer;
            DefaultCulture = LocalizerOptionsAccessor.Value.DefaultCulture;

            var type = typeof(SharedResource);

            LocalizerOriginal = LocalizerFactory.Create(type);
            Localizer = LocalizerOriginal;

            LocalizerControllerOriginal = localizer;
            LocalizerController = LocalizerControllerOriginal;

            if (string.IsNullOrWhiteSpace(DefaultCulture)) DefaultCulture = DefaultLibraryCulture;

            if (LocalizerOriginal != null)
            {
                Logger.LogTrace("------------LocalizerDefault LoadCulture");
                // установка дефолтного локализатора из библиотеки
                if (LibraryCultures.Contains(DefaultCulture))
                {
                    LocalizerDefault = LocalizerOriginal.LoadCulture(DefaultCulture, Logger);
                }
                else
                {
                    LocalizerDefault = LocalizerOriginal.LoadCulture(DefaultLibraryCulture, Logger);
                }
            }

            if (LocalizerControllerOriginal != null)
            {
                Logger.LogTrace("------------LocalizerControllerDefault LoadCulture");
                LocalizerControllerDefault = LocalizerControllerOriginal.LoadCulture(DefaultCulture, Logger);
            }
        }

        #region Localize

        //public readonly IStringLocalizerFactory LocalizerFactory;

        /// <summary>
        /// Язык по умолчанию для админки и системных сообщений. Загружается из конфига.
        /// </summary>
        //protected string DefaultCulture { get; set; }

        // языки определенные в этой либе (в будущем переделать на рефлексию)
        private List<string> LibraryCultures = new List<string>() { "en", "ru" };
        private string DefaultLibraryCulture { get { return "en"; } }

        /// <summary>
        /// локализатор из сборки библиотеки
        /// </summary>
        //public IStringLocalizer Localizer;
        private readonly IStringLocalizer LocalizerOriginal;
        /// <summary>
        /// локализатор из сборки где определен контроллер
        /// </summary>
        public IStringLocalizer LocalizerController;
        private readonly IStringLocalizer LocalizerControllerOriginal;
        /// <summary>
        /// локализатор из сборки библиотеки (с культурой по умолчанию)
        /// </summary>
        public readonly IStringLocalizer LocalizerDefault;
        /// <summary>
        /// локализатор из сборки где определен контроллер (с культурой по умолчанию)
        /// </summary>
        public readonly IStringLocalizer LocalizerControllerDefault;

        /// <summary>
        /// Текущий язык. Может быть нулом, только если у сайта нет языков.
        /// </summary>
        public Languages Language { get; set; }
        /// <summary>
        /// Язык с урла. Нулл если язык в урле не определен.
        /// </summary>
        public string CultureFromGet { get; set; }

        public void ResolveCulture(ActionExecutingContext context, List<Languages> SiteLanguages, IUrlHelper Url)
        {
            Logger.LogTrace("============================= ResolveCulture in");
            //using (new BLog(LoggerMEF, "Controller2Garin::ResolveCulture() Languages.GetUnblocked", GetType().FullName))
            //SiteLanguages = Languages.GetUnblocked(Site.Id).ToList();

            var provider = new AcceptLanguageHeaderRequestCultureProvider();
            var languagePreferences = provider.DetermineProviderCultureResult(context.HttpContext).Result;

            var cult = context.RouteData.Values["Culture"] as string;
            // приведем к корректному виду (в урле могут быть маленькие буквы во второй части ru-ru)
            if (string.IsNullOrWhiteSpace(cult) || !cult.TestCulture())
            {
                // ни в коем случае так не делать! если тут будет послана фигня, то будет сделан редирект на правильный урл => просто игнорируем
                //cult = null;
            }
            else
            {
                cult = cult.ToLower();
                if (cult.Length > 2)
                {
                    // мы прошли проверку TestCulture и значит елси больше 2 букв то етсь 1 дефис
                    var tmp = cult.Explode("-");
                    cult = tmp[0] + "-" + tmp[1].ToUpper();
                }
            }
            CultureFromGet = cult;

            if (!string.IsNullOrWhiteSpace(cult))
            {
                var l = SiteLanguages.FirstOrDefault(i => i.Lang == cult);
                if (l != null) // указанный язык есть на сайте все ок
                {
                    SetLang(l);
                    return;
                }
                else
                {
                    // языка указанного в запросе нет на сайте => делаем редирект (постоянный, чтобы выбить из поискового индекса)
                    var url = Url.CurrentUrl(new { culture = (string)null });
                    Logger.LogTrace("ResolveCulture redirect to {0}", url);
                    context.Result = new RedirectResult(url, true);
                    return;
                }
            }
            else // культуры в запросе нет, ее надо поискать и установить. Вот тут никаких редиректов быть не должно! Редирект на язык делаем исключительно на js (для кеширования страниц)
            {
                if (languagePreferences != null && languagePreferences.Cultures != null)
                {
                    Logger.LogInformation("AcceptLanguageHeaderRequestCultureProvider Cultures={0}", string.Join(", ", languagePreferences.Cultures));

                    foreach (var c in languagePreferences.Cultures)
                    {
                        // пробуем найти предпочитаемый юзером язык в списке языков сайта
                        var l = SiteLanguages.FirstOrDefault(i => i.Lang == c);
                        if (l != null)
                        {
                            if (l.IsDefault) // если язык по умолчанию, то показываем контент сайта на этом языке. причем без указания культуры в запросе
                            {
                                SetLang(l);
                                return;
                            }
                            else
                            {
                                // редирект тут делать не будем. редирект сделаем на уровне js. (для кеширования страниц)
                                // условие пока оставим иначе захочется пооптимизировать
                                SetLang(l);
                                return;

                                // предпочитаемый юзером язык определен на сайте и так как язык не является дефолтным, то нам надо сделать редирект
                                //currentRV["culture"] = l.Lang;
                                /*var url = Url.CurrentUrl(new { culture = l.Lang });//Url.RouteUrlWithCulture(routeName, currentRV.ToDictionary(i=>i.Key, i=>i.Value)); //new { culture = l.Lang }
                                Logger.LogTrace("ResolveCulture redirect to {0}", url);
                                context.Result = new RedirectResult(url, true);
                                return;
                                */
                            }
                        }
                    }
                }
            }

            // у юзера нет предпочтений или они полностью не совпадают с языками сайта
            // скорее всего это не юзер, а бот и потому никаких редиректов тут не делаем
            if (SiteLanguages.Any())
            {
                var l = SiteLanguages.FirstOrDefault(i => i.IsDefault);
                if (l == null) // в теории тут ошибка! как минимум один из языков должен быть дефолтным!
                {
                    Logger.LogWarning("Site {0} have not default language.", SiteLanguages[0].SiteId);
                    l = SiteLanguages.First();
                }
                SetLang(l);
                return;
            }
            else // на сайте вообще нет языков
            {
                // надо бы попробовать установить локализацию по предпочтениям юзера. 
                // НЕТ! кеширование контента накрывается. Страницы должны выдаваться независимо от предпочтений юзера!
                /*if (languagePreferences != null && languagePreferences.Cultures != null)
                    foreach (var c in languagePreferences.Cultures)
                    {
                        if (TrySetCulture(c)) return;
                    }
                /**/
                Localizer = null;
                LocalizerController = null;
            }
        }

        public HtmlString Localize(string key)
        {
            //Logger.LogTrace("Controller2Garin::Localize {0}", key);
            if (string.IsNullOrWhiteSpace(key)) return new HtmlString(string.Empty);

            string res = key;

            if (Language != null)
            {
                res = Language[key];
                //Logger.LogTrace("------------- Controller2Garin::Language {0}->[{1}]", key, res);
            }

            // пробуем загрузить строку из сборки с контролерами
            if (LocalizerController != null)
            {
                res = LocalizerController[key];
                //Logger.LogTrace("------------- Controller2Garin::LocalizerController {0}->[{1}]", key, res);
            }
            if (res == key && Localizer != null) // нет такой строки
            {
                res = Localizer[key];
                //Logger.LogTrace("------------- Controller2Garin::Localizer {0}->[{1}]", key, res);
            }

            if (res == key && LocalizerControllerDefault != null)
            {
                res = LocalizerControllerDefault[key];
                //Logger.LogTrace("------------- Controller2Garin::LocalizerControllerDefault {0}->[{1}]", key, res);
            }
            if (res == key && LocalizerDefault != null)
            {
                res = LocalizerDefault[key];
                //Logger.LogTrace("------------- Controller2Garin::LocalizerDefault {0}->[{1}]", key, res);
            }

            //Logger.LogTrace("Controller2Garin::Localize {0}->{1}", key, res);
            return new HtmlString(res);
        }

        /// <summary>
        /// Данная функция только инициализирует локализаторы
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        private bool TrySetCulture(string culture)
        {
            IStringLocalizer LC = null, L = null;
            if (LocalizerControllerOriginal != null)
            {
                Logger.LogTrace("===================== LocalizerController LoadCulture");
                LC = LocalizerControllerOriginal.LoadCulture(culture, Logger);
            }

            if (LocalizerOriginal != null)
            {
                Logger.LogTrace("===================== Localizer LoadCulture");
                L = LocalizerOriginal.LoadCulture(culture, Logger);
            }
            // один из локализаторов должен существовать
            if (L != null || LC != null)
            {
                // это влияет тока на параметры форматирования 
                var cultureInfo = new CultureInfo(culture);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
                /**/
                Localizer = L;
                LocalizerController = LC;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверка и установка запрашиваемого языка
        /// </summary>
        /// <param name="lang"></param>
        private void SetLang(Languages lang)
        {
            Logger.LogTrace("SetLang {0}", lang.Lang);

            try
            {
                TrySetCulture(lang.Lang);

                Language = lang;
            }
            catch (Exception e)
            {
                Logger.LogInformation("Cann't set language to {0}: {1}", lang.Lang, e);
            }
        }

        #endregion

    }
}
