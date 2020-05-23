
using System;
using System.Collections.Generic;
using Contensive.Processor.Models.Domain;
using Contensive.Processor.Controllers;
using Nustache.Core;
using Contensive.Processor.Properties;
using Contensive.Models.Db;

namespace Contensive.Processor.Addons.AdminSite {
    public class GroupRuleEditor {
        //
        //========================================================================
        //
        public static string get(CoreController core, AdminDataModel adminData) {
            string result = null;
            try {
                string editorRow = "";
                editorRow = AdminUIEditorController.getGroupRuleEditor(core, adminData);
                result = AdminUIController.getEditPanel(core, true, "Group Membership", "", editorRow);
                adminData.editSectionPanelCount += 1;
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        public class GroupRuleEditorRowModel {
            public string idHidden;
            public string checkboxInput;
            public string groupCaption;
            public string expiresInput;
            public string roleInput;
            public string relatedButtonList;
        }
        public class GroupRuleEditorModel {
            public string listCaption;
            public string helpText;
            public List<GroupRuleEditorRowModel> rowList;
        }
    }
}
