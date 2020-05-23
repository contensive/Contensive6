
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class GroupRulesClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, grouprules");
                //
                //
                // GroupRules with bad ContentID
                //   Handled record by record removed to prevent CDEF reload
                //
                LogController.logInfo(core, "Deleting Group Rules with bad ContentID.");
                string sql = "Select ccGroupRules.ID"
                    + " From ccGroupRules LEFT JOIN ccContent on ccContent.ID=ccGroupRules.ContentID"
                    + " WHERE (ccContent.ID is null)";
                using (var csData = new CsModel(core)) {
                    csData.openSql(sql);
                    while (csData.ok()) {
                        MetadataController.deleteContentRecord(core, "Group Rules", csData.getInteger("ID"));
                        csData.goNext();
                    }
                }
                //
                // GroupRules with bad GroupID
                //
                LogController.logInfo(core, "Deleting Group Rules with bad GroupID.");
                sql = "delete ccGroupRules"
                    + " From ccGroupRules"
                    + " LEFT JOIN ccgroups on ccgroups.ID=ccGroupRules.GroupID"
                    + " WHERE (ccgroups.ID is null)";
                core.db.executeNonQuery(sql);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}