
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class EmailLogClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, email log");
                //
                // email log for only 365 days
                core.db.executeNonQuery("delete from ccemaillog where (dateadded < DATEADD(day,-" + env.emailDropArchiveAgeDays + ",CAST(GETDATE() AS DATE)))");
                //
                // clear email body field for emails older than 7 days
                LogController.logInfo(core, "Clear email body field for email logs older then " + env.emailLogBodyRetainDays + " days");
                DateTime emailLogBodyRetainDate = core.dateTimeNowMockable.AddDays(-env.emailLogBodyRetainDays).Date;
                core.db.executeNonQuery("update ccemaillog set body=null where dateadded<" + DbController.encodeSQLDate(emailLogBodyRetainDate));
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}