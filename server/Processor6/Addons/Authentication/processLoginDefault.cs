//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Primitives {
    public class ProcessLoginDefaultClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// process a username/password authentication with no success result.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string result = "";
            try {
                LoginController.processLoginFormDefault(((CPClass)cp).core);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}
