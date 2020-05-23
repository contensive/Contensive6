
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class ContentFieldClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "HousekeepDaily, content fields");
                //
                LogController.logInfo(core, "Deleting content fields with no content.");
                string sql = "delete from ccfields from ccfields left join cccontent on cccontent.id=ccfields.contentId where cccontent.id is null";
                core.db.executeNonQuery(sql);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}