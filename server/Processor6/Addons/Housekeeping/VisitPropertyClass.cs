
using System;
using Contensive.Processor.Controllers;
using Contensive.Processor.Models.Domain;
using System.Threading.Tasks;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class VisitPropertyClass {

        //
        //====================================================================================================
        /// <summary>
        /// Delete stale visit properties (older than 24 hrs)
        /// </summary>
        /// <param name="core"></param>
        public static void housekeep(CoreController core) {
            try {
                //
                LogController.logInfo(core, "Housekeep, visitproperites");
                //
                {
                    //
                    // Visit Properties with no visits
                    string sql = "delete ccproperties from ccproperties left join ccvisits on ccvisits.id=ccproperties.keyid where (ccproperties.typeid=1) and (ccvisits.id is null)";
                    core.db.sqlCommandTimeout = 180;
                    Task.Run(() => core.db.executeNonQueryAsync(sql));
                }
                {
                    //
                    // -- delete properties of visits over 1 hour old
                    string sql = "delete from ccproperties from ccproperties p left join  ccvisits v on (v.id=p.keyid and p.typeid=1) where v.lastvisittime<dateadd(hour, -1, getdate())";
                    core.db.sqlCommandTimeout = 180;
                    Task.Run(() => core.db.executeNonQueryAsync(sql));

                }
                {
                    //
                    // -- fallback, delete all visit properties over 24 hours old
                    string sql = "delete from ccProperties where (TypeID=1)and(dateAdded<dateadd(hour, -24, getdate()))";
                    core.db.sqlCommandTimeout = 180;
                    Task.Run(() => core.db.executeNonQueryAsync(sql));
                }

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
    }
}