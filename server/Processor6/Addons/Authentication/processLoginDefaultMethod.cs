//
using System;
using System.Collections.Generic;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Constants;
using Contensive.BaseClasses;
//
namespace Contensive.Processor.Addons.Primitives {
    public class ProcessLoginDefaultMethodClass : Contensive.BaseClasses.AddonBaseClass {
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
                // -- default login page
                core.doc.continueProcessing = false;
                Dictionary<string, string> addonArguments = new Dictionary<string, string>();
                addonArguments.Add("Force Default Login", "true");
                return core.addon.execute(addonGuidLoginPage, new CPUtilsBaseClass.addonExecuteContext {
                    addonType = CPUtilsBaseClass.addonContext.ContextPage,
                    argumentKeyValuePairs = addonArguments,
                    errorContextMessage = "processing field editor preference remote"
                });
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}
