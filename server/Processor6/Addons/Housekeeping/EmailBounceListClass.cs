
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
using Contensive.Processor;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public class EmailBounceListClass  {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            //
            core.cpParent.Db.ExecuteNonQuery("update ccmembers set allowbulkemail=1 from ccmembers m left join emailbouncelist b on b.name LIKE CONCAT('%', m.[email], '%') where b.id is not null and m.email is not null");
            //
        }
    }
}