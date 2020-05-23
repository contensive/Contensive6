//
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class ContentWatchClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "HousekeepDaily, contentwatch");
                //
                using (var csData = new CsModel(core)) {
                    string sql = "select cccontentwatch.id from cccontentwatch left join cccontent on cccontent.id=cccontentwatch.contentid  where (cccontent.id is null)or(cccontent.active=0)or(cccontent.active is null)";
                    csData.openSql(sql);
                    while (csData.ok()) {
                        MetadataController.deleteContentRecord(core, "Content Watch", csData.getInteger("ID"));
                        csData.goNext();
                    }
                }

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}