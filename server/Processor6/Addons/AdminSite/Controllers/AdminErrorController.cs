
using System;

using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.AdminSite {
    public static class AdminErrorController {
        //
        //===========================================================================
        //
        public static string get(CoreController core, string UserError, string DeveloperError) {
            string result = "";
            try {
                //
                if (!string.IsNullOrEmpty(DeveloperError)) {
                    throw (new Exception("error [" + DeveloperError + "], user error [" + UserError + "]"));
                }
                if (!string.IsNullOrEmpty(UserError)) {
                    Processor.Controllers.ErrorController.addUserError(core, UserError);
                    result = AdminDataModel.AdminFormErrorOpen + Processor.Controllers.ErrorController.getUserError(core) + AdminDataModel.AdminFormErrorClose;
                }
                //
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
    }
}
