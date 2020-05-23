
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class PersonClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, people");
                //
                // Any member records that were created outside contensive need to have CreatedByVisit=0 (past v4.1.152)
                core.db.executeNonQuery("update ccmembers set CreatedByVisit=0 where createdbyvisit is null");
                //
                // delete members from the non-cookie visits
                // legacy records without createdbyvisit will have to be corrected by hand (or upgrade)
                //
                LogController.logInfo(core, "Deleting members from visits with no cookie support older than Midnight, Two Days Ago");
                string sql = "delete from ccmembers from ccmembers m,ccvisits v where v.memberid=m.id and(m.visits=1) and(m.createdbyvisit=1) and(m.username is null) and(m.email is null) and(v.cookiesupport=0)and(v.lastvisittime<DATEADD(hour, -2, GETDATE()))";
                try {
                    core.db.sqlCommandTimeout = 1800;
                    core.db.executeNonQuery(sql);
                } catch (Exception ex) {
                    LogController.logError(core, ex);
                }
                //
                // -- Remove old guest records
                DateTime ArchiveDate = core.dateTimeNowMockable.AddDays(-env.guestArchiveAgeDays).Date;
                string SQLTablePeople = MetadataController.getContentTablename(core, "People");
                string DeleteBeforeDateSQL = DbController.encodeSQLDate(ArchiveDate);
                //
                LogController.logInfo(core, "Deleting members with  LastVisit before DeleteBeforeDate [" + ArchiveDate + "], exactly one total visit, a null username and a null email address.");
                //
                string SQLCriteria = "(LastVisit<" + DeleteBeforeDateSQL + ")and(createdbyvisit=1)and(Visits=1)and(Username is null)and(email is null)";
                core.db.sqlCommandTimeout = 1800;
                core.db.deleteTableRecordChunks("ccmembers", SQLCriteria, 1000, 10000);
                //
                // delete 'guests' Members with one visits but no valid visit record
                //
                LogController.logInfo(core, "Deleting 'guest' members with no visits (name is default name, visits=1, username null, email null,dateadded=lastvisit)");
                sql = "delete from ccmembers from ccmembers m,ccvisits v"
                    + " where v.memberid=m.id"
                    + " and(m.createdbyvisit=1)"
                    + " and(m.Visits=1)"
                    + " and(m.Username is null)"
                    + " and(m.email is null)"
                    + " and(m.dateadded=m.lastvisit)"
                    + " and(v.id is null)";
                core.db.sqlCommandTimeout = 1800;
                core.db.executeNonQuery(sql);
                //
                // delete 'guests' Members created before ArchivePeopleAgeDays
                //
                LogController.logInfo(core, "Deleting 'guest' members with no visits (name is default name, visits=1, username null, email null,dateadded=lastvisit)");
                sql = "delete from ccmembers from ccmembers m left join ccvisits v on v.memberid=m.id"
                    + " where(m.createdbyvisit=1)"
                    + " and(m.Visits=1)"
                    + " and(m.Username is null)"
                    + " and(m.email is null)"
                    + " and(m.dateadded=m.lastvisit)"
                    + " and(v.id is null)";
                core.db.sqlCommandTimeout = 1800;
                core.db.executeNonQuery(sql);
                //
                // -- mark all people allowbulkemail if their email address is in the emailbouncelist
                sql = "update ccmembers set allowbulkemail=0 from ccmembers m left join emailbouncelist b on b.name LIKE CONCAT('%', m.[email], '%') where b.id is not null and m.email is not null";
                core.db.sqlCommandTimeout = 1800;
                core.cpParent.Db.ExecuteNonQuery(sql);
                //
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}