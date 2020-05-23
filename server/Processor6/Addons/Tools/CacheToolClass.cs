
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Constants;
//
namespace Contensive.Processor.Addons.Tools {
    //
    public class CacheToolClass : Contensive.BaseClasses.AddonBaseClass {
        //
        private static readonly string ButtonCacheGet = "Get Cache";
        private static readonly string ButtonCacheStore = "Store Cache";
        private static readonly string ButtonCacheInvalidate = "Invalidate Cache";
        private static readonly string ButtonCacheInvalidateAll = "Invalidate All Cache";
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cpBase) {
            return getForm_CacheTool((CPClass)cpBase);
        }
        //
        //=============================================================================
        //   Print the manual query form
        //=============================================================================
        //
        public static string getForm_CacheTool(CPClass cp) {
            CoreController core = cp.core;
            try {
                var form = cp.AdminUI.NewToolForm();
                form.Title = "Cache Tool";
                form.Description = "Use this tool to get/store/invalidate the application's cache.";
                //Stream.add(AdminUIController.getHeaderTitleDescription("Cache Tool", "Use this tool to get/store/invalidate the application's cache."));
                //
                string cacheKey = cp.Doc.GetText("cacheKey");
                string cacheValue = cp.Doc.GetText("cacheValue");
                string button = cp.Doc.GetText("button");
                //
                StringBuilderLegacyController formBody = new StringBuilderLegacyController();
                if (button == ButtonCacheGet) {
                    //
                    // -- Get Cache
                    formBody.add("<div>" + core.dateTimeNowMockable + " cache.getObject(" + cacheKey + ")</div>");
                    object resultObj = cp.Cache.GetObject(cacheKey);
                    if (resultObj == null) {
                        formBody.add("<div>" + core.dateTimeNowMockable + " NULL returned</div>");
                    } else {
                        try {
                            cacheValue = Newtonsoft.Json.JsonConvert.SerializeObject(resultObj);
                            formBody.add("<div>" + core.dateTimeNowMockable + "CacheValue object returned, json serialized, length [" + cacheValue.Length + "]</div>");
                        } catch (Exception ex) {
                            formBody.add("<div>" + core.dateTimeNowMockable + " exception during serialization, ex [" + ex + "]</div>");
                        }
                    }
                    formBody.add("<p>" + core.dateTimeNowMockable + " Done</p>");
                } else if (button == ButtonCacheStore) {
                    //
                    // -- Store Cache
                    formBody.add("<div>" + core.dateTimeNowMockable + " cache.store(" + cacheKey + "," + cacheValue + ")</div>");
                    cp.Cache.Store(cacheKey, cacheValue);
                    formBody.add("<p>" + core.dateTimeNowMockable + " Done</p>");
                } else if (button == ButtonCacheInvalidate) {
                    //
                    // -- Invalidate
                    cacheValue = "";
                    formBody.add("<div>" + core.dateTimeNowMockable + " cache.Invalidate(" + cacheKey + ")</div>");
                    cp.Cache.Invalidate(cacheKey);
                    formBody.add("<p>" + core.dateTimeNowMockable + " Done</p>");
                } else if (button == ButtonCacheInvalidateAll) {
                    //
                    // -- Store Cache
                    cacheValue = "";
                    formBody.add("<div>" + core.dateTimeNowMockable + " cache.InvalidateAll()</div>");
                    cp.Cache.InvalidateAll();
                    formBody.add("<p>" + core.dateTimeNowMockable + " Done</p>");
                }
                //
                // Display form
                {
                    //
                    // -- cache key
                    formBody.add(cp.Html5.H4("Cache Key"));
                    formBody.add(cp.Html5.Div(cp.AdminUI.GetTextEditor("cacheKey", cacheKey, "cacheKey", false)));
                }
                {
                    //
                    // -- cache value
                    formBody.add(cp.Html5.H4("Cache Value"));
                    formBody.add(cp.Html5.Div(cp.AdminUI.GetTextEditor("cacheValue", cacheValue, "cacheValue", false)));
                }
                //
                // -- assemble form
                form.Body = formBody.text;
                form.AddFormButton(ButtonCancel);
                form.AddFormButton(ButtonCacheGet);
                form.AddFormButton(ButtonCacheStore);
                form.AddFormButton(ButtonCacheInvalidate);
                form.AddFormButton(ButtonCacheInvalidateAll);
                return form.GetHtml(cp);
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
    }
}

