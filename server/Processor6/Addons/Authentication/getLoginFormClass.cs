//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Login {
    public class GetLoginFormClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// Login Form
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string returnHtml = "";
            try {
                bool forceDefaultLogin = cp.Doc.GetBoolean("Force Default Login");
                returnHtml = LoginController.getLoginForm(((CPClass)cp).core, forceDefaultLogin);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return returnHtml;
        }
    }
}
