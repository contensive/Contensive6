
using System;
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Addons.RobotsTxt {
    public class RobotsTxtClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// Return robots.exe. NOTE - this route requires an exception added to web.config
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string result = "";
            try {
                CoreController core = ((CPClass)cp).core;
                //
                // -- Robots.txt
                string Filename = "config/RobotsTxtBase.txt";
                // 
                // set this way because the preferences page needs a filename in a site property (enhance later)
                core.siteProperties.setProperty("RobotsTxtFilename", Filename);
                result = core.cdnFiles.readFileText(Filename);
                if (string.IsNullOrEmpty(result)) {
                    //
                    // save default robots.txt
                    //
                    result = "User-agent: *\r\nDisallow: /admin/\r\nDisallow: /images/";
                    core.wwwFiles.saveFile(Filename, result);
                }
                result += core.addonCache.robotsTxt;
                core.webServer.setResponseContentType("text/plain");
                core.doc.continueProcessing = false;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}
