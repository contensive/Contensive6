//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Login {
    //
    public class GetLoginPageClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string returnHtml = "";
            try {
                CoreController core = ((CPClass)cp).core;
                returnHtml = LoginController.getLoginPage(core, false);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return returnHtml;
        }
    }
}
