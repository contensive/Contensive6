
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Processor.Controllers {
    public static class ExportTemplateController {
        // 
        // ====================================================================================================

        public static string get(CPBaseClass cp, PageTemplateModel template) {
            try {
                string addonList = "";
                foreach (var rule in DbBaseModel.createList<AddonTemplateRuleModel>(cp, "(templateId=" + template.id + ")")) {
                    AddonModel addon = DbBaseModel.create<AddonModel>(cp, rule.addonId);
                    if (addon != null) {
                        addonList += System.Environment.NewLine + "\t\t" + "<IncludeAddon name=\"" + addon.name + "\" guid=\"" + addon.ccguid + "\" />";
                    }
                }
                return ""
                    + System.Environment.NewLine + "\t" + "<Template"
                        + " name=\"" + System.Net.WebUtility.HtmlEncode(template.name) + "\""
                        + " guid=\"" + template.ccguid + "\""
                        + " issecure=\"" + GenericController.getYesNo(template.isSecure) + "\""
                        + " >"
                        + addonList
                        + System.Environment.NewLine + "\t\t" + "<BodyHtml>" + ExportController.tabIndent(cp, ExportController.EncodeCData(template.bodyHTML)) + "</BodyHtml>"
                        + System.Environment.NewLine + "\t" + "</Template>";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetAddonNode");
                return string.Empty;
            }
        }
        // 
    }
}
