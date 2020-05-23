
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class FieldHelpClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, fieldhelp");
                //
                // Field help with no field
                //
                LogController.logInfo(core, "Deleting field help with no field.");
                string sql = ""
                    + "delete from ccfieldhelp where id in ("
                    + " select h.id"
                    + " from ccfieldhelp h"
                    + " left join ccfields f on f.id=h.fieldid where f.id is null"
                    + ")";
                core.db.executeNonQuery(sql);
                //
                // Field help duplicates - messy, but I am not sure where they are coming from, and this patchs the edit page performance problem
                //
                LogController.logInfo(core, "Deleting duplicate field help records.");
                sql = ""
                    + "delete from ccfieldhelp where id in ("
                    + " select b.id"
                    + " from ccfieldhelp a"
                    + " left join ccfieldhelp b on a.fieldid=b.fieldid where a.id< b.id"
                    + ")";
                core.db.executeNonQuery(sql);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}