
using System;
using Contensive.BaseClasses;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.Housekeeping {
    /// <summary>
    /// support for housekeeping functions
    /// </summary>
    public class HouseKeepClass : AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// addon interface
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                CoreController core = ((CPClass)cp).core;
                //
                LogController.logInfo(core, "Housekeep");
                //
                var env = new HouseKeepEnvironmentModel(core);
                int TimeoutSave = core.db.sqlCommandTimeout;
                core.db.sqlCommandTimeout = 1800;
                //
                // -- hourly tasks
                HourlyTasksClass.housekeep(core, env);
                //
                // -- daily tasks
                if (env.forceHousekeep || env.runDailyTasks) {
                    DailyTasksClass.housekeepDaily(core, env);
                }
                core.db.sqlCommandTimeout = TimeoutSave;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}
