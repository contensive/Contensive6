//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class AddonContentFieldTypeRuleClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "HousekeepDaily, contentfieldtype rules");
                //
                core.db.executeNonQuery("delete from ccAddonContentFieldTypeRules where id in (select r.id from ccAddonContentFieldTypeRules r left join ccaggregatefunctions a on a.id=r.addonid where a.Id Is Null)");
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}