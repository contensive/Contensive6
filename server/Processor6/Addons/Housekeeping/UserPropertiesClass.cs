
using System;
using Contensive.Processor.Controllers;
using Contensive.Processor.Models.Domain;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class UserProperyClass {

        //====================================================================================================
        /// <summary>
        /// delete orphan user properties
        /// </summary>
        /// <param name="core"></param>
        public static void housekeep(CoreController core) {
            try {
                //
                LogController.logInfo(core, "Housekeep, userproperites");
                //
                string sql = "delete from ccProperties from ccProperties p left join ccmembers m on m.id=p.KeyID where (p.TypeID=" + (int)PropertyModelClass.PropertyTypeEnum.user + ") and (m.ID is null)";
                core.db.sqlCommandTimeout = 180;
                core.db.executeNonQuery(sql);
                //
                // Member Properties with no member
                //
                LogController.logInfo(core, "Deleting member properties with no member record.");
                sql = "delete ccproperties from ccproperties left join ccmembers on ccmembers.id=ccproperties.keyid where (ccproperties.typeid=0) and (ccmembers.id is null)";
                core.db.sqlCommandTimeout = 180;
                core.db.executeNonQuery(sql);

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
    }
}