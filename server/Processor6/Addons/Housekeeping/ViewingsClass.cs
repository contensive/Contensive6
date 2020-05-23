
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class ViewingsClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, viewings");
                //
                try {
                    //
                    // delete old viewings
                    core.db.sqlCommandTimeout = 1800;
                    core.db.executeNonQuery("delete from ccviewings where (dateadded < DATEADD(day,-" + env.visitArchiveAgeDays + ",CAST(GETDATE() AS DATE)))");
                } catch (Exception) {
                    LogController.logWarn(core, "exception deleting old viewings");
                }
                //
                if (env.archiveDeleteNoCookie) {
                    //
                    LogController.logInfo(core, "Deleting viewings from visits with no cookie support older than Midnight, Two Days Ago");
                    //
                    // if this fails, continue with the rest of the work
                    try {
                        string sql = "delete from ccviewings from ccviewings h,ccvisits v where h.visitid=v.id and(v.CookieSupport=0)and(v.LastVisitTime<DATEADD(day,-2,CAST(GETDATE() AS DATE)))";
                        core.db.sqlCommandTimeout = 1800;
                        core.db.executeNonQuery(sql);
                    } catch (Exception) {
                        LogController.logWarn(core, "exception deleting viewings with no cookie");
                    }
                }
                //
                LogController.logInfo(core, "Deleting viewings with null or invalid VisitID");
                //
                try {
                    string sql = "delete from ccviewings  where (visitid=0 or visitid is null)";
                    core.db.sqlCommandTimeout = 1800;
                    core.db.executeNonQuery(sql);
                } catch (Exception) {
                    LogController.logWarn(core, "exception deleting viewings with invalid visits");
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}