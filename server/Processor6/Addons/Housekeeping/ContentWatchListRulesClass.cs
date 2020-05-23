//
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class ContentWatchListRulesClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                // ContentWatchListRules with bad ContentWatchID
                //
                LogController.logInfo(core, "Deleting ContentWatchList Rules with bad ContentWatchID.");
                string sql = "delete ccContentWatchListRules"
                    + " From ccContentWatchListRules"
                    + " LEFT JOIN ccContentWatch on ccContentWatch.ID=ccContentWatchListRules.ContentWatchID"
                    + " WHERE (ccContentWatch.ID is null)";
                core.db.executeNonQuery(sql);
                //
                // ContentWatchListRules with bad ContentWatchListID
                //
                LogController.logInfo(core, "Deleting ContentWatchList Rules with bad ContentWatchListID.");
                sql = "delete ccContentWatchListRules"
                    + " From ccContentWatchListRules"
                    + " LEFT JOIN ccContentWatchLists on ccContentWatchLists.ID=ccContentWatchListRules.ContentWatchListID"
                    + " WHERE (ccContentWatchLists.ID is null)";
                core.db.executeNonQuery(sql);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}