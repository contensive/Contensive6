//
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class ActivityLogClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, activitylog");
                {
                    //
                    //
                    LogController.logInfo(core, "Deleting activities older than 30 days.");
                    //
                    core.db.executeNonQuery("delete from ccactivitylog where (DateAdded is null)or(DateAdded<DATEADD(day,-30,CAST(GETDATE() AS DATE)))");

                }
                {
                    //
                    LogController.logInfo(core, "Deleting activities with no member record.");
                    //
                    core.db.executeNonQuery("delete ccactivitylog from ccactivitylog left join ccmembers on ccmembers.id=ccactivitylog.memberid where (ccmembers.id is null)");
                }

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}