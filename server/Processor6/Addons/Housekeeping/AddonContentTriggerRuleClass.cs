//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class AddonContentTriggerRuleClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "HousekeepDaily, addoncontenttrigger rules");
                //
                // -- addon trigger rules
                core.db.executeNonQuery("delete from ccAddonContentTriggerRules where id in (select r.id from ccAddonContentTriggerRules r left join ccaggregatefunctions a on a.id = r.addonid where a.Id Is Null)");
                core.db.executeNonQuery("delete from ccAddonContentTriggerRules where id in (select r.id from ccAddonContentTriggerRules r left join cccontent c on c.id = r.contentid where c.id is null)");

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}