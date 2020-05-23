//
using System;
using System.Collections.Generic;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Constants;
using Contensive.BaseClasses;
//
namespace Contensive.Processor.Addons.Primitives {
    public class ProcessLogoutLoginMethodClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// getFieldEditorPreference remote method
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string result = "";
            try {
                CoreController core = ((CPClass)cp).core;
                //
                // -- login
                core.session.logout();
                core.doc.continueProcessing = false;
                Dictionary<string, string> addonArguments = new Dictionary<string, string>();
                addonArguments.Add("Force Default Login", "false");
                return core.addon.execute(addonGuidLoginPage, new CPUtilsBaseClass.addonExecuteContext {
                    addonType = CPUtilsBaseClass.addonContext.ContextPage,
                    errorContextMessage = "processing logout/login method"
                });
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}
