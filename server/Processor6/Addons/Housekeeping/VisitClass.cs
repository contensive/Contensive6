
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Controllers.GenericController;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class VisitClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, visits");
                {
                    //
                    LogController.logInfo(core, "Deleting visits with no DateAdded");
                    //
                    core.db.sqlCommandTimeout = 180;
                    core.db.executeNonQuery("delete from ccvisits where (DateAdded is null)or(DateAdded<DATEADD(year,-10,CAST(GETDATE() AS DATE)))");
                }
                {
                    //
                    LogController.logInfo(core, "Deleting visits with no visitor");
                    //
                    core.db.executeNonQuery("delete from ccvisits from ccvisits v left join ccvisitors r on r.id=v.visitorid where (r.id is null)");
                }
                if (env.archiveDeleteNoCookie) {
                    //
                    LogController.logInfo(core, "Deleting visits with no cookie support older than Midnight, Two Days Ago");
                    //
                    core.db.sqlCommandTimeout = 180;
                    core.db.executeNonQuery("delete from ccvisits where (CookieSupport=0)and(LastVisitTime<DATEADD(day,-2,CAST(GETDATE() AS DATE)))");
                }
                DateTime OldestVisitDate = default(DateTime);
                //
                // Get Oldest Visit
                using (var csData = new CsModel(core)) {
                    if (csData.openSql(core.db.getSQLSelect("ccVisits", "DateAdded", "", "dateadded", "", 1))) {
                        OldestVisitDate = csData.getDate("DateAdded").Date;
                    }
                }
                //
                // Remove old visit records
                //   if > 30 days in visit table, limit one pass to just 30 days
                //   this is to prevent the entire server from being bogged down for one site change
                //
                if (OldestVisitDate == DateTime.MinValue) {
                    LogController.logInfo(core, "No visit records were removed because no visit records were found while requesting the oldest visit.");
                } else {
                    DateTime ArchiveDate = core.dateTimeNowMockable.AddDays(-env.visitArchiveAgeDays).Date;
                    int DaystoRemove = encodeInteger(ArchiveDate.Subtract(OldestVisitDate).TotalDays);
                    if (DaystoRemove > 30) {
                        ArchiveDate = OldestVisitDate.AddDays(30);
                    }
                    if (OldestVisitDate >= ArchiveDate) {
                        LogController.logInfo(core, "No records were removed because Oldest Visit Date [" + OldestVisitDate + "] >= ArchiveDate [" + ArchiveDate + "].");
                    } else {
                        LogController.logInfo(core, "Removing records from [" + OldestVisitDate + "] to [" + ArchiveDate + "].");
                        DateTime SingleDate = default(DateTime);
                        SingleDate = OldestVisitDate;
                        do {
                            houseKeep_App_Daily_RemoveVisitRecords(core, SingleDate);
                            SingleDate = SingleDate.AddDays(1);
                        } while (SingleDate < ArchiveDate);
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }

        //====================================================================================================
        //
        public static void houseKeep_App_Daily_RemoveVisitRecords(CoreController core, DateTime DeleteBeforeDate) {
            try {
                //
                int TimeoutSave = 0;
                string SQL = null;
                string DeleteBeforeDateSQL = null;
                string appName = null;
                string SQLTablePeople = null;
                //
                // Set long timeout (30 min) needed for heavy work on big tables
                TimeoutSave = core.db.sqlCommandTimeout;
                core.db.sqlCommandTimeout = 1800;
                //
                SQLTablePeople = MetadataController.getContentTablename(core, "People");
                //
                appName = core.appConfig.name;
                DeleteBeforeDateSQL = DbController.encodeSQLDate(DeleteBeforeDate);
                //
                // Visits older then archive age
                //
                LogController.logInfo(core, "Deleting visits before [" + DeleteBeforeDateSQL + "]");
                core.db.deleteTableRecordChunks("ccVisits", "(DateAdded<" + DeleteBeforeDateSQL + ")", 1000, 10000);
                //
                // Viewings with visits before the first
                //
                LogController.logInfo(core, "Deleting viewings with visitIDs lower then the lowest ccVisits.ID");
                core.db.deleteTableRecordChunks("ccviewings", "(visitid<(select min(ID) from ccvisits))", 1000, 10000);

                //
                // restore sved timeout
                //
                core.db.sqlCommandTimeout = TimeoutSave;
                return;
                //
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}