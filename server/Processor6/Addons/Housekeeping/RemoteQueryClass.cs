
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class RemoteQueryClass {

        //====================================================================================================
        //
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                string SQL = "";
                //
                // Remote Query Expiration
                //
                SQL = "delete from ccRemoteQueries where (DateExpires is not null)and(DateExpires<" + DbController.encodeSQLDate(core.dateTimeNowMockable) + ")";
                core.db.executeNonQuery(SQL);
                SQL = "delete from ccmenuEntries where id in (select m.ID from ccMenuEntries m left join ccAggregateFunctions a on a.id=m.AddonID where m.addonid<>0 and a.id is null)";
                core.db.executeNonQuery(SQL);
                //
                SQL = "delete from ccmenuEntries where id in (select m.ID from ccMenuEntries m left join ccAggregateFunctions a on a.id=m.helpaddonid where m.helpaddonid<>0 and a.id is null)";
                core.db.executeNonQuery(SQL);
                //
                SQL = "delete from ccmenuEntries where id in (select m.ID from ccMenuEntries m left join ccAddonCollections c on c.id=m.helpcollectionid Where m.helpcollectionid <> 0 And c.Id Is Null)";
                core.db.executeNonQuery(SQL);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
    }
}