
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class PageContentClass {

        //====================================================================================================
        //
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "HousekeepDaily, page content");
                {
                    //
                    // Move Archived pages from their current parent to their archive parent
                    //
                    bool NeedToClearCache = false;
                    string SQL = "select * from ccpagecontent where (( DateArchive is not null )and(DateArchive<" + core.sqlDateTimeMockable + "))and(active<>0)";
                    using (var csData = new CsModel(core)) {
                        csData.openSql(SQL);
                        while (csData.ok()) {
                            int RecordId = csData.getInteger("ID");
                            int ArchiveParentId = csData.getInteger("ArchiveParentID");
                            if (ArchiveParentId == 0) {
                                SQL = "update ccpagecontent set DateArchive=null where (id=" + RecordId + ")";
                                core.db.executeNonQuery(SQL);
                            } else {
                                SQL = "update ccpagecontent set ArchiveParentID=null,DateArchive=null,parentid=" + ArchiveParentId + " where (id=" + RecordId + ")";
                                core.db.executeNonQuery(SQL);
                                NeedToClearCache = true;
                            }
                            csData.goNext();
                        }
                        csData.close();
                    }
                    //
                    // Clear caches
                    //
                    if (NeedToClearCache) {
                        object emptyData = null;
                        core.cache.invalidate("Page Content");
                        core.cache.storeObject("PCC", emptyData);
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}