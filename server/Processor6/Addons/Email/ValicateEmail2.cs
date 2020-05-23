//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Primitives {
    public class ValidateEmailClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            try {
                CoreController core = ((CPClass)cp).core;
                //
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return "";
        }
    }
}
