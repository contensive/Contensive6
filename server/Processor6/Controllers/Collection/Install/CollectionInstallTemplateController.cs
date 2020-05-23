﻿
using System;
using System.Xml;
using System.Collections.Generic;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
using Contensive.Processor.Models.Domain;
using System.Linq;
using static Contensive.BaseClasses.CPFileSystemBaseClass;
using Contensive.Processor.Exceptions;
using Contensive.BaseClasses;
using System.Reflection;
using NLog;
using Contensive.Models.Db;
using System.Globalization;
using System.Text;

namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// install addon collections.
    /// </summary>
    public class CollectionInstallTemplateController {
        //
        //======================================================================================================
        //
        public static void installNode(CoreController core, XmlNode templateNode, int collectionId, ref bool return_UpgradeOK, ref string return_ErrorMessage, ref bool collectionIncludesDiagnosticAddons) {
            return_ErrorMessage = "";
            return_UpgradeOK = true;
            try {
                string Basename = toLCase(templateNode.Name);
                if (Basename == "template") {
                    bool IsFound = false;
                    string templateName = XmlController.getXMLAttribute(core, ref IsFound, templateNode, "name", "No Name");
                    if (string.IsNullOrEmpty(templateName)) {
                        templateName = "No Name";
                    }
                    string recordGuid = XmlController.getXMLAttribute(core, ref IsFound, templateNode, "guid", templateName);
                    if (string.IsNullOrEmpty(recordGuid)) {
                        recordGuid = templateName;
                    }
                    var template = DbBaseModel.create<PageTemplateModel>(core.cpParent, recordGuid);
                    if (template == null) {
                        template = DbBaseModel.createByUniqueName<PageTemplateModel>(core.cpParent, templateName);
                    }
                    if (template == null) {
                        template = DbBaseModel.addDefault<PageTemplateModel>(core.cpParent);
                    }
                    foreach (XmlNode childNode in templateNode.ChildNodes) {
                        switch (childNode.Name.ToLowerInvariant()) {
                            case "includeaddon": {
                                    string addonGuid = XmlController.getXMLAttribute(core, ref IsFound, childNode, "guid", "");
                                    string addonName = XmlController.getXMLAttribute(core, ref IsFound, childNode, "name", "No Name");
                                    if (!string.IsNullOrEmpty(addonGuid)) {
                                        var addon = DbBaseModel.create<AddonModel>(core.cpParent, addonGuid);
                                        if (addon == null) {
                                            return_ErrorMessage += "Addon dependency [" + addonName + "] for template [" + templateName + "] could not be found by its guid [" + addonGuid + "]";
                                        }
                                        var ruleList = DbBaseModel.createList<AddonTemplateRuleModel>(core.cpParent, "(addonId=" + addon.id + ")and(addonId=" + template.id + ")");
                                        if (ruleList.Count.Equals(0)) {
                                            var rule = DbBaseModel.addDefault<AddonTemplateRuleModel>(core.cpParent);
                                            rule.addonId = addon.id;
                                            rule.templateId = template.id;
                                            rule.save(core.cpParent);
                                        }
                                    }
                                    break;
                                }
                            case "bodyhtml": {
                                    template.bodyHTML = childNode.InnerText;
                                    break;
                                }
                        }
                    }
                    template.ccguid = recordGuid;
                    template.name = templateName;
                    //record.bodyHTML = templateNode.InnerText;
                    template.collectionId = collectionId;
                    template.isSecure = XmlController.getXMLAttributeBoolean(core, ref IsFound, templateNode, "issecure", false);
                    template.save(core.cpParent);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
    }
}
