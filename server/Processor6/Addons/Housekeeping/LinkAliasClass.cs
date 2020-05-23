
using System;
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class LinkAliasClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, linkalias");
                //
                // -- delete dups
                string sql = "delete from ccLinkAliases where id in ( select b.id from cclinkaliases a,cclinkaliases b where a.id<b.id and a.name=b.name )";
                core.db.executeNonQuery(sql);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }

        }
    }
}