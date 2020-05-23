
using System;
using Contensive.Processor.Controllers;
using System.Xml;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class SoftwareUpdatesClass {
        //====================================================================================================
        //
        public static bool downloadAndInstall(CoreController core) {
            bool loadOK = true;
            try {
                //
                LogController.logInfo(core, "Housekeep, download and install");
                //
                var Doc = new XmlDocument() { XmlResolver = null};
                string URL = "http://support.contensive.com/GetUpdates?iv=" + CoreController.codeVersion();
                loadOK = true;
                Doc.Load(URL);
                if ((Doc.DocumentElement.Name.ToLowerInvariant() == GenericController.toLCase("ContensiveUpdate")) && (Doc.DocumentElement.ChildNodes.Count != 0)) {
                    foreach (XmlNode CDefSection in Doc.DocumentElement.ChildNodes) {
                        string Copy = CDefSection.InnerText;
                        switch (GenericController.toLCase(CDefSection.Name)) {
                            case "mastervisitnamelist":
                                //
                                // Read in the interfaces and save to Add-ons
                                core.privateFiles.saveFile("config\\VisitNameList.txt", Copy);
                                break;
                            case "masteremailbouncefilters":
                                //
                                // save the updated filters file
                                core.privateFiles.saveFile("config\\EmailBounceFilters.txt", Copy);
                                break;
                            case "mastermobilebrowserlist":
                                //
                                // save the updated filters file
                                //
                                core.privateFiles.saveFile("config\\MobileBrowserList.txt", Copy);
                                break;
                        }
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return loadOK;
        }
        //
    }
}