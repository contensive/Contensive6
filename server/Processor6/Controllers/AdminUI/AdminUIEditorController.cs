﻿
using System;
using System.Collections.Generic;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
using Contensive.Processor.Models.Domain;
using System.Text;
using Contensive.Processor.Exceptions;
using Contensive.Models.Db;
using System.Globalization;
using Contensive.Processor.Addons.AdminSite.Models;
using Contensive.Processor.Addons.AdminSite;
using Nustache.Core;
using Contensive.Processor.Properties;

namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// UI rendering for Admin
    /// REFACTOR - add  try-catch
    /// not IDisposable - not contained classes that need to be disposed
    /// </summary>
    public static class AdminUIEditorController {
        //
        // ====================================================================================================
        /// <summary>
        /// return the default admin editor for this field type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <returns></returns>
        public static string getBooleanEditor(CoreController core, string htmlName, bool htmlValue, bool readOnly, string htmlId, bool required = false) {
            string result = HtmlController.div(HtmlController.checkbox(htmlName, htmlValue, htmlId, false, "", readOnly, "1", "", required), "checkbox");
            if (readOnly) result += HtmlController.inputHidden(htmlName, htmlValue);
            return result;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// return the default editor for this type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static string getCurrencyEditor(CoreController core, string fieldName, double? fieldValue, bool readOnly, string htmlId, bool required, string whyReadOnlyMsg) {
            if (readOnly) {
                double FieldValueNumber = GenericController.encodeNumber(fieldValue);
                string EditorString = (HtmlController.inputHidden(fieldName.ToLowerInvariant(), encodeText(FieldValueNumber)));
                EditorString += (HtmlController.inputNumber(core, fieldName.ToLowerInvariant(), FieldValueNumber, htmlId, "text form-control", readOnly, required));
                EditorString += whyReadOnlyMsg;
                return EditorString;
            } else {
                return (HtmlController.inputNumber(core, fieldName.ToLowerInvariant(), fieldValue, htmlId, "text form-control", readOnly, false));
            }
        }
        //
        // ====================================================================================================
        //
        public static string getDateTimeEditor(CoreController core, string fieldName, DateTime? FieldValueDate, bool readOnly, string htmlId, bool fieldRequired, string WhyReadOnlyMsg) {
            htmlId = !string.IsNullOrEmpty(htmlId) ? htmlId : "id" + getRandomInteger(core).ToString();
            string inputDate = HtmlController.inputDate(core, fieldName + "-date", FieldValueDate, "", "component-" + htmlId + "-date", "form-control", readOnly, fieldRequired, false);
            DateTime? FieldValueTime = FieldValueDate;
            if (FieldValueTime != null) {
                // if time is 12:00 AM, display a blank in the time field
                DateTime testTime = (DateTime)FieldValueTime;
                if (testTime.Hour.Equals(0) && testTime.Minute.Equals(0) && testTime.Second.Equals(0)) { FieldValueTime = null; }
            }
            string inputTime = HtmlController.inputTime(core, fieldName + "-time", FieldValueTime, "component-" + htmlId + "-time", "form-control", readOnly, fieldRequired, false);
            string dateTimeString = (FieldValueDate != null) ? ((DateTime)FieldValueDate).ToString("o", CultureInfo.InvariantCulture) : "";
            string inputDateTime = HtmlController.inputHidden(fieldName, dateTimeString, "", htmlId);
            // todo move to adminUI script
            string js = HtmlController.scriptCode(core, ""
                + "document.addEventListener('DOMContentLoaded', function(){"
                + "$('body').on('change','#component-" + htmlId + "-date,#component-" + htmlId + "-time',function(e){console.log('date/time change');setDateTimeEditorHidden('" + htmlId + "');});"
                + "});"
                + "");
            return HtmlController.div(HtmlController.div(inputDate + inputTime, "input-group") + inputDateTime + js);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// return form input element for this field type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="currentPathFilename"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="required"></param>
        /// <param name="whyReadOnlyMsg"></param>
        /// <returns></returns>
        public static string getFileEditor(CoreController core, string fieldName, string currentPathFilename, bool readOnly, string htmlId, bool required, string whyReadOnlyMsg) {
            if (readOnly && string.IsNullOrEmpty(currentPathFilename)) {
                return HtmlController.div("[no file]") + HtmlController.inputHidden(fieldName, currentPathFilename);
            }
            if (string.IsNullOrEmpty(currentPathFilename)) {
                return HtmlController.inputFile(fieldName, htmlId, "form-control-file");
            }
            string nonEncodedLink = getCdnFileLink(core, currentPathFilename);
            string encodedLink = HtmlController.encodeHtml(nonEncodedLink);
            string fieldValuefilename = "";
            string fieldValuePath = "";
            core.privateFiles.splitDosPathFilename(currentPathFilename, ref fieldValuePath, ref fieldValuefilename);
            if (readOnly) {
                return HtmlController.a("[" + fieldValuefilename + "]", encodedLink, "", "", "", "_blank");
            }
            string deleteCheckboxId = "deleteCheckbox" + getRandomInteger(core).ToString();
            string deleteCheckbox = ""
                + HtmlController.div(
                    HtmlController.checkbox(fieldName + ".DeleteFlag", false, deleteCheckboxId, false, "form-check-input")
                    + HtmlController.label("Delete", deleteCheckboxId, "form-check-label"),
                "form-check");
            return ""
                + HtmlController.a("[" + fieldValuefilename + "]", encodedLink, "", "", "", "_blank")
                + deleteCheckbox
                + HtmlController.inputFile(fieldName, htmlId, "form-control-file");
        }
        //
        // ====================================================================================================
        /// <summary>
        /// return the default admin editor for this field type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="isPassword"></param>
        /// <returns></returns>
        public static string getHtmlCodeEditor(CoreController core, string fieldName, string fieldValue, bool readOnly, string htmlId, bool required = false) {
            //
            // longer text data, or text that contains a CR
            return HtmlController.inputTextarea(core, fieldName, fieldValue, 10, -1, htmlId, false, readOnly, "text form-control", false, 0, required);
        }
        //
        // ====================================================================================================
        //
        public static string getHtmlEditor(CoreController core, string fieldName, string fieldValue, string editorAddonListJSON, string styleList, string styleOptionList, bool readONly)
            => getHtmlEditor(core, fieldName, fieldValue, editorAddonListJSON, styleList, styleOptionList, readONly, "");
        //
        public static string getHtmlEditor(CoreController core, string fieldName, string fieldValue, string editorAddonListJSON, string styleList, string styleOptionList, bool readOnly, string htmlId) {
            string result = "";
            if (readOnly) {
                result += HtmlController.inputHidden(fieldName, fieldValue);
                result += AdminUIEditorController.getHtmlCodeEditor(core, fieldName, fieldValue, readOnly, htmlId, false);
                return result;
            }
            if (string.IsNullOrEmpty(fieldValue)) {
                //
                // editor needs a starting p tag to setup correctly
                fieldValue = HTMLEditorDefaultCopyNoCr;
            }
            result += core.html.getFormInputHTML(fieldName.ToLowerInvariant(), fieldValue, "500", "", readOnly, true, editorAddonListJSON, styleList, styleOptionList);
            result = "<div style=\"width:95%\">" + result + "</div>";
            return result;
        }
        //
        // ====================================================================================================
        //
        public static string getImageEditor(CoreController core, string fieldName, string currentPathFilename, bool readOnly, string htmlId, bool required, string whyReadOnlyMsg) {
            if (readOnly && string.IsNullOrEmpty(currentPathFilename)) {
                return HtmlController.div("[no image]") + HtmlController.inputHidden(fieldName, currentPathFilename);
            }
            if (string.IsNullOrEmpty(currentPathFilename)) {
                return HtmlController.inputFile(fieldName, htmlId, "form-control-file");
            }
            string nonEncodedLink = getCdnFileLink(core, currentPathFilename);
            string encodedLink = HtmlController.encodeHtml(nonEncodedLink);
            string fieldValuefilename = "";
            string fieldValuePath = "";
            core.privateFiles.splitDosPathFilename(currentPathFilename, ref fieldValuePath, ref fieldValuefilename);
            string deleteCheckbox = "";
            string uploadControl = "";
            if (!readOnly) {
                string deleteCheckboxId = "deleteCheckbox" + getRandomInteger(core).ToString();
                deleteCheckbox = ""
                    + HtmlController.div(
                        HtmlController.checkbox(fieldName + ".DeleteFlag", false, deleteCheckboxId, false, "form-check-input")
                        + HtmlController.label("Delete", deleteCheckboxId, "form-check-label"),
                    "form-check");
                uploadControl = HtmlController.inputFile(fieldName, htmlId, "form-control-file");
            }
            string resultImage = HtmlController.a(HtmlController.img(encodedLink, "", 0, 0, "w-100"), encodedLink, "", "", "", "_blank");
            string resultAnchor = HtmlController.a("[" + fieldValuefilename + "]", encodedLink, "", "", "", "_blank");
            return ""
                + HtmlController.div(resultImage, "d-table-cell", "", "width:100px;max-height:200px;")
                + HtmlController.div(resultAnchor + deleteCheckbox + uploadControl, "d-table-cell pl-4 align-top");
        }
        //
        // ====================================================================================================
        //
        public static string getIntegerEditor(CoreController core, string fieldName, int? fieldValue, bool readOnly, string htmlId, bool required, string whyReadOnlyMsg) {
            if (readOnly) {
                string EditorString = (HtmlController.inputHidden(fieldName.ToLowerInvariant(), GenericController.encodeText(fieldValue)));
                EditorString += (HtmlController.inputInteger(core, fieldName.ToLowerInvariant(), fieldValue, htmlId, "text form-control", readOnly, required));
                EditorString += whyReadOnlyMsg;
                return EditorString;
            } else {
                return (HtmlController.inputInteger(core, fieldName.ToLowerInvariant(), fieldValue, htmlId, "text form-control", readOnly, false));
            }
        }
        //
        // ====================================================================================================
        //
        public static string getLinkEditor(CoreController core, string fieldName, string fieldValue, bool readOnly, string htmlId, bool required) {
            return getTextEditor(core, fieldName, fieldValue, readOnly, htmlId, required);
        }
        //
        // ====================================================================================================
        //
        public static string getLongTextEditor(CoreController core, string fieldName, string fieldValue, bool readOnly, string htmlId, bool required, string whyReadOnlyMsg) {
            //
            return HtmlController.inputTextarea(core, fieldName, fieldValue, 10, -1, htmlId, false, readOnly, "text form-control", false, 0, required);
        }
        //
        // ====================================================================================================
        //
        public static string getLookupContentEditor(CoreController core, string fieldName, int fieldValue, ContentMetadataModel lookupContentMetacontent, ref bool IsEmptyList, bool readOnly, string htmlId, string WhyReadOnlyMsg, bool fieldRequired, string sqlFilter) {
            string result = "";
            if (lookupContentMetacontent == null) {
                LogController.logWarn(core, "Lookup content not set, field [" + fieldName + "]");
                return string.Empty;
            }
            if (readOnly) {
                //
                // ----- Lookup ReadOnly
                result += (HtmlController.inputHidden(fieldName, GenericController.encodeText(fieldValue)));
                using (var csData = new CsModel(core)) {
                    csData.openRecord(lookupContentMetacontent.name, fieldValue, "Name,ContentControlID");
                    if (csData.ok()) {
                        if (csData.getText("Name") == "") {
                            result += AdminUIEditorController.getTextEditor(core, fieldName + "-readonly-fpo", "No Name", readOnly, htmlId);
                        } else {
                            result += AdminUIEditorController.getTextEditor(core, fieldName + "-readonly-fpo", csData.getText("Name"), readOnly, htmlId);
                        }
                        result += ("&nbsp;[<a TabIndex=-1 href=\"?" + rnAdminForm + "=4&cid=" + lookupContentMetacontent.id + "&id=" + fieldValue + "\" target=\"_blank\">View details in new window</a>]");
                    } else {
                        result += ("None");
                    }
                }
                result += ("&nbsp;[<a TabIndex=-1 href=\"?cid=" + lookupContentMetacontent.id + "\" target=\"_blank\">See all " + lookupContentMetacontent.name + "</a>]");
                result += WhyReadOnlyMsg;
            } else {
                //
                // -- not readonly
                string nonLabel = (fieldRequired) ? "" : "None";
                result += core.html.selectFromContent(fieldName, fieldValue, lookupContentMetacontent.name, sqlFilter, nonLabel, "", ref IsEmptyList, "select form-control");
                if (fieldValue != 0) {
                    using (var csData = new CsModel(core)) {
                        if (csData.openRecord(lookupContentMetacontent.name, fieldValue, "ID")) {
                            result += ("&nbsp;[<a TabIndex=-1 href=\"?" + rnAdminForm + "=4&cid=" + lookupContentMetacontent.id + "&id=" + fieldValue + "\" target=\"_blank\">Details</a>]");
                        }
                        csData.close();
                    }
                }
                result += ("&nbsp;[<a TabIndex=-1 href=\"?cid=" + lookupContentMetacontent.id + "\" target=\"_blank\">See all " + lookupContentMetacontent.name + "</a>]");

            }
            return result;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// admin editor for a lookup field into a content table
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="lookupContentId"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="WhyReadOnlyMsg"></param>
        /// <param name="fieldRequired"></param>
        /// <param name="IsEmptyList"></param>
        /// <returns></returns>
        public static string getLookupContentEditor(CoreController core, string fieldName, int fieldValue, int lookupContentId, ref bool IsEmptyList, bool readOnly, string htmlId, string WhyReadOnlyMsg, bool fieldRequired, string sqlFilter) {
            ContentMetadataModel lookupContentMetacontent = ContentMetadataModel.create(core, lookupContentId);
            if (lookupContentMetacontent == null) {
                LogController.logWarn(core, "Lookup content not set, field [" + fieldName + "], lookupContentId [" + lookupContentId + "]");
                return string.Empty;
            }
            return getLookupContentEditor(core, fieldName, fieldValue, lookupContentMetacontent, ref IsEmptyList, readOnly, htmlId, WhyReadOnlyMsg, fieldRequired, sqlFilter);
        }
        //
        public static string getLookupContentEditor(CoreController core, string fieldName, int fieldValue, string lookupContentName, ref bool IsEmptyList, bool readOnly, string htmlId, string WhyReadOnlyMsg, bool fieldRequired, string sqlFilter) {
            ContentMetadataModel lookupContentMetacontent = ContentMetadataModel.createByUniqueName(core, lookupContentName);
            if (lookupContentMetacontent == null) {
                LogController.logWarn(core, "Lookup content not set, field [" + fieldName + "], lookupContentName [" + lookupContentName + "]");
                return string.Empty;
            }
            return getLookupContentEditor(core, fieldName, fieldValue, lookupContentMetacontent, ref IsEmptyList, readOnly, htmlId, WhyReadOnlyMsg, fieldRequired, sqlFilter);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// admin editor for a lookup field into a static list
        /// </summary>
        /// <param name="core"></param>
        /// <param name="htmlName"></param>
        /// <param name="index"></param>
        /// <param name="lookupList"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="WhyReadOnlyMsg"></param>
        /// <param name="fieldRequired"></param>
        /// <returns></returns>
        public static string getLookupListEditor(CoreController core, string htmlName, int index, List<string> lookupList, bool readOnly, string htmlId, string WhyReadOnlyMsg, bool fieldRequired) {
            string result = "";
            if (readOnly) {
                //
                // ----- Lookup ReadOnly
                result += (HtmlController.inputHidden(htmlName, index.ToString()));
                if (index < 1) {
                    result += getTextEditor(core, htmlName + "-readonly-fpo", "None", readOnly, htmlId);
                } else if (index > (lookupList.Count)) {
                    result += getTextEditor(core, htmlName + "-readonly-fpo", "None", readOnly, htmlId);
                } else {
                    result += getTextEditor(core, htmlName + "-readonly-fpo", lookupList[index - 1], readOnly, htmlId);
                }
                result += WhyReadOnlyMsg;
            } else {
                if (!fieldRequired) {
                    result += HtmlController.selectFromList(core, htmlName, index, lookupList, "Select One", htmlId, "select form-control");
                } else {
                    result += HtmlController.selectFromList(core, htmlName, index, lookupList, "", htmlId, "select form-control");
                }

            }
            return result;
        }
        //
        public static string getLookupListEditor(CoreController core, string htmlName, string currentValue, List<string> lookupList, bool readOnly, string htmlId, string WhyReadOnlyMsg, bool fieldRequired) {
            string result = "";
            if (readOnly) {
                //
                // ----- Lookup ReadOnly
                result += (HtmlController.inputHidden(htmlName, GenericController.encodeText(currentValue)));
                if (currentValue == null) {
                    result += getTextEditor(core, htmlName + "-readonly-fpo", "None", readOnly, htmlId);
                } else {
                    result += getTextEditor(core, htmlName + "-readonly-fpo", currentValue, readOnly, htmlId);
                }
                result += WhyReadOnlyMsg;
            } else {
                if (!fieldRequired) {
                    result += HtmlController.selectFromList(core, htmlName, currentValue, lookupList, "Select One", htmlId, "select form-control");
                } else {
                    result += HtmlController.selectFromList(core, htmlName, currentValue, lookupList, "", htmlId, "select form-control");
                }

            }
            return result;
        }
        //
        // ====================================================================================================
        //
        public static string getManyToManyEditor(CoreController core, ContentFieldMetadataModel field, string htmlName, string currentValueCommaList, int editRecordId, bool readOnly, string WhyReadOnlyMsg) {
            string result = "";
            //
            string MTMContent0 = MetadataController.getContentNameByID(core, field.contentId);
            string MTMContent1 = MetadataController.getContentNameByID(core, field.manyToManyContentId);
            string MTMRuleContent = MetadataController.getContentNameByID(core, field.manyToManyRuleContentId);
            string MTMRuleField0 = field.manyToManyRulePrimaryField;
            string MTMRuleField1 = field.manyToManyRuleSecondaryField;
            result += core.html.getCheckList(htmlName, MTMContent0, editRecordId, MTMContent1, MTMRuleContent, MTMRuleField0, MTMRuleField1, "", "", readOnly, false, currentValueCommaList);
            result += WhyReadOnlyMsg;
            return result;
        }
        //
        // ====================================================================================================
        //
        public static string getMemberSelectEditor(CoreController core, string htmlName, int selectedRecordId, GroupModel group, bool readOnly, string htmlId, bool fieldRequired, string WhyReadOnlyMsg) {
            string EditorString = "";
            if (readOnly) {
                //
                // -- readOnly
                EditorString += HtmlController.inputHidden(htmlName, selectedRecordId.ToString());
                if (selectedRecordId == 0) {
                    EditorString += "None";
                } else {
                    var selectedUser = DbBaseModel.create<PersonModel>(core.cpParent, selectedRecordId);
                    if (selectedUser == null) {
                        EditorString += AdminUIEditorController.getTextEditor(core, htmlName + "-readonly-fpo", "(deleted)", readOnly, htmlId);
                    } else {
                        EditorString += AdminUIEditorController.getTextEditor(core, htmlName + "-readonly-fpo", (string.IsNullOrWhiteSpace(selectedUser.name)) ? "No Name" : HtmlController.encodeHtml(selectedUser.name), readOnly, htmlId);
                        EditorString += ("&nbsp;[<a TabIndex=-1 href=\"?af=4&cid=" + selectedUser.contentControlId + "&id=" + selectedRecordId + "\" target=\"_blank\">View details in new window</a>]");
                    }
                }
                EditorString += WhyReadOnlyMsg;
            } else {
                //
                // -- editable
                EditorString += core.html.selectUserFromGroup(htmlName, selectedRecordId, group.id, "", (fieldRequired) ? "" : "None", htmlId, "select form-control");
                if (selectedRecordId != 0) {
                    var selectedUser = DbBaseModel.create<PersonModel>(core.cpParent, selectedRecordId);
                    if (selectedUser == null) {
                        EditorString += "Deleted";
                    } else {
                        string recordName = (string.IsNullOrWhiteSpace(selectedUser.name)) ? "No Name" : HtmlController.encodeHtml(selectedUser.name);
                        EditorString += "&nbsp;[Edit <a TabIndex=-1 href=\"?af=4&cid=" + selectedUser.contentControlId + "&id=" + selectedRecordId + "\">" + HtmlController.encodeHtml(recordName) + "</a>]";
                    }
                }
                EditorString += ("&nbsp;[Select from members of <a TabIndex=-1 href=\"?cid=" + ContentMetadataModel.getContentId(core, "groups") + "\">" + group.name + "</a>]");
            }
            return EditorString;
        }
        //
        // ====================================================================================================
        //
        public static string getMemberSelectEditor(CoreController core, string htmlName, int selectedRecordId, int groupId, string groupName, bool readOnly, string htmlId, bool fieldRequired, string WhyReadOnlyMsg) {
            GroupModel group = null;
            if (groupId > 0) {
                group = DbBaseModel.create<GroupModel>(core.cpParent, groupId);
            }
            if ((group == null) && string.IsNullOrWhiteSpace(groupName)) {
                //
                // -- groupId invalid and groupname empty
                return "No selection can be made because this Member Select field does not have a group assigned." + HtmlController.inputHidden(htmlName, groupId);
            }
            if (group == null) {
                //
                // -- groupId invalid but valid name
                group = DbBaseModel.createByUniqueName<GroupModel>(core.cpParent, groupName);
                if (group == null) {
                    //
                    // -- no group with this name, create a new one
                    group = DbBaseModel.addDefault<GroupModel>(core.cpParent);
                    group.name = groupName;
                    group.save(core.cpParent);
                }
            }
            return getMemberSelectEditor(core, htmlName, selectedRecordId, group, readOnly, htmlId, fieldRequired, WhyReadOnlyMsg);
        }
        //
        public static string getMemberSelectEditor(CoreController core, string htmlName, int selectedRecordId, string groupGuid, string groupName, bool readOnly, string htmlId, bool fieldRequired, string WhyReadOnlyMsg) {
            GroupModel group = null;
            if (!string.IsNullOrWhiteSpace(groupGuid)) {
                group = DbBaseModel.create<GroupModel>(core.cpParent, groupGuid);
            }
            if ((group == null) && string.IsNullOrWhiteSpace(groupName)) {
                //
                // -- groupGuid invalid and groupname empty
                return string.Empty;
            }
            if (group == null) {
                //
                // -- groupGuid invalid but valid name
                group = DbBaseModel.createByUniqueName<GroupModel>(core.cpParent, groupName);
                if (group == null) {
                    //
                    // -- no group with this name, create a new one
                    group = DbBaseModel.addDefault<GroupModel>(core.cpParent);
                    group.name = groupName;
                    group.save(core.cpParent);
                }
            }
            return getMemberSelectEditor(core, htmlName, selectedRecordId, group, readOnly, htmlId, fieldRequired, WhyReadOnlyMsg);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// return the default editor for this type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="required"></param>
        /// <param name="whyReadOnlyMsg"></param>
        /// <returns></returns>
        public static string getNumberEditor(CoreController core, string fieldName, double? fieldValue, bool readOnly, string htmlId, bool required, string whyReadOnlyMsg) {
            if (readOnly) {
                string EditorString = (HtmlController.inputHidden(fieldName.ToLowerInvariant(), GenericController.encodeText(fieldValue)));
                EditorString += (HtmlController.inputNumber(core, fieldName.ToLowerInvariant(), fieldValue, htmlId, "text form-control", readOnly, required));
                EditorString += whyReadOnlyMsg;
                return EditorString;
            } else {
                return (HtmlController.inputNumber(core, fieldName.ToLowerInvariant(), fieldValue, htmlId, "text form-control", readOnly, false));
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// return form input element for this field type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public static string getPasswordEditor(CoreController core, string fieldName, string fieldValue, bool readOnly, string htmlId) {
            return HtmlController.inputText_Legacy(core, fieldName, fieldValue, -1, -1, htmlId, true, readOnly, "password form-control", 255);
        }
        //
        //====================================================================================================
        //
        public static string getSelectorStringEditor(CoreController core, string SitePropertyName, string SitePropertyValue, string selector) {
            var result = new StringBuilder();
            try {
                Dictionary<string, string> instanceOptions = new Dictionary<string, string> {
                    { SitePropertyName, SitePropertyValue }
                };
                //
                // -- 
                string ExpandedSelector = "";
                Dictionary<string, string> addonInstanceProperties = new Dictionary<string, string>();
                core.addon.buildAddonOptionLists(ref addonInstanceProperties, ref ExpandedSelector, SitePropertyName + "=" + selector, instanceOptions, "0", true);
                int Pos = GenericController.strInstr(1, ExpandedSelector, "[");
                if (Pos == 0) {
                    //
                    // -- EXIT with Text addon_execute_result
                    selector = GenericController.decodeNvaArgument(selector);
                    return AdminUIEditorController.getTextEditor(core, SitePropertyName, selector);
                }
                //
                // List of Options, might be select, radio or checkbox
                //
                string LCaseOptionDefault = GenericController.toLCase(ExpandedSelector.left(Pos - 1));
                int PosEqual = GenericController.strInstr(1, LCaseOptionDefault, "=");
                if (PosEqual > 0) {
                    LCaseOptionDefault = LCaseOptionDefault.Substring(PosEqual);
                }
                LCaseOptionDefault = GenericController.decodeNvaArgument(LCaseOptionDefault);
                ExpandedSelector = ExpandedSelector.Substring(Pos);
                Pos = GenericController.strInstr(1, ExpandedSelector, "]");
                string OptionSuffix = "";
                if (Pos > 0) {
                    if (Pos < ExpandedSelector.Length) {
                        OptionSuffix = GenericController.toLCase((ExpandedSelector.Substring(Pos)).Trim(' '));
                    }
                    ExpandedSelector = ExpandedSelector.left(Pos - 1);
                }
                string[] OptionValues = ExpandedSelector.Split('|');
                int OptionCnt = OptionValues.GetUpperBound(0) + 1;
                int OptionPtr = 0;
                for (OptionPtr = 0; OptionPtr < OptionCnt; OptionPtr++) {
                    string OptionValue_AddonEncoded = OptionValues[OptionPtr].Trim(' ');
                    if (!string.IsNullOrEmpty(OptionValue_AddonEncoded)) {
                        Pos = GenericController.strInstr(1, OptionValue_AddonEncoded, ":");
                        string OptionCaption = null;
                        string OptionValue = null;
                        if (Pos == 0) {
                            OptionValue = GenericController.decodeNvaArgument(OptionValue_AddonEncoded);
                            OptionCaption = OptionValue;
                        } else {
                            OptionCaption = GenericController.decodeNvaArgument(OptionValue_AddonEncoded.left(Pos - 1));
                            OptionValue = GenericController.decodeNvaArgument(OptionValue_AddonEncoded.Substring(Pos));
                        }
                        switch (OptionSuffix) {
                            case "checkbox": {
                                    //
                                    // Create checkbox addon_execute_getFormContent_decodeSelector
                                    //
                                    bool selected = (GenericController.strInstr(1, "," + LCaseOptionDefault + ",", "," + GenericController.toLCase(OptionValue) + ",") != 0);
                                    result.Append(HtmlController.checkbox(SitePropertyName + OptionPtr, selected, "", false, "", false, OptionValue, OptionCaption));
                                    break;
                                }
                            case "radio": {
                                    //
                                    // Create Radio addon_execute_getFormContent_decodeSelector
                                    //
                                    if (GenericController.toLCase(OptionValue) == LCaseOptionDefault) {
                                        result.Append("<div style=\"white-space:nowrap\"><input type=\"radio\" name=\"" + SitePropertyName + "\" value=\"" + OptionValue + "\" checked=\"checked\" >" + OptionCaption + "</div>");
                                    } else {
                                        result.Append("<div style=\"white-space:nowrap\"><input type=\"radio\" name=\"" + SitePropertyName + "\" value=\"" + OptionValue + "\" >" + OptionCaption + "</div>");
                                    }
                                    break;
                                }
                            default: {
                                    //
                                    // Create select addon_execute_result
                                    //
                                    if (GenericController.toLCase(OptionValue) == LCaseOptionDefault) {
                                        result.Append("<option value=\"" + OptionValue + "\" selected>" + OptionCaption + "</option>");
                                    } else {
                                        result.Append("<option value=\"" + OptionValue + "\">" + OptionCaption + "</option>");
                                    }
                                    break;
                                }
                        }
                    }
                }
                //
                // -- finish off each type
                switch (OptionSuffix) {
                    case "checkbox": {
                            //
                            //
                            result.Append(HtmlController.inputHidden(SitePropertyName + "CheckBoxCnt", OptionCnt));
                            return result.ToString();
                        }
                    case "radio": {
                            //
                            // Create Radio addon_execute_result
                            //
                            return result.ToString();
                        }
                    default: {
                            //
                            // Create select addon_execute_result
                            //
                            return "<select name=\"" + SitePropertyName + "\" class=\"select form-control\">" + result + "</select>";
                        }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                return string.Empty;
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// return the default admin editor for this field type
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="readOnly"></param>
        /// <param name="htmlId"></param>
        /// <param name="isPassword"></param>
        /// <returns></returns>
        public static string getTextEditor(CoreController core, string fieldName, string fieldValue, bool readOnly, string htmlId, bool required = false) {
            if ((fieldValue.IndexOf("\n", StringComparison.InvariantCulture) == -1) && (fieldValue.Length < 80)) {
                //
                // text field shorter then 40 characters without a CR
                return HtmlController.inputText_Legacy(core, fieldName, fieldValue, 1, -1, htmlId, false, readOnly, "text form-control", 255, false, "", required);
            }
            return AdminUIEditorController.getHtmlCodeEditor(core, fieldName, fieldValue, readOnly, htmlId, required);
        }
        //
        // ====================================================================================================
        //
        public static string getTextEditor(CoreController core, string fieldName, string fieldValue)
            => getTextEditor(core, fieldName, fieldValue, false, "", false);
        //
        // ====================================================================================================
        //
        public static string getTextEditor(CoreController core, string fieldName, string fieldValue, bool readOnly)
            => getTextEditor(core, fieldName, fieldValue, readOnly, "", false);
        //
        // ====================================================================================================
        //
        public static string getRedirectEditor(CoreController core, ContentFieldMetadataModel field, AdminDataModel adminData, EditRecordModel editRecord, string fieldValue, bool readOnly, string htmlId, bool required) {
            try {
                if (!string.IsNullOrEmpty(field.redirectPath)) {
                    //
                    // -- if hardcoded redirect link, create open-in-new-windows
                    return HtmlController.a("Open in New Window", field.redirectPath, "", "", "", "_blank");
                }
                //
                // -- redirect goes to a list of records (one-to-many or many-to-many
                var gridData = new AdminDataModel(core, new AdminDataRequest() {
                    adminAction = 0,
                    adminButton = "",
                    adminForm = 0,
                    adminSourceForm = 0,
                    contentId = field.redirectContentId,
                    fieldEditorPreference = "",
                    guid = "",
                    id = 0,
                    ignore_legacyMenuDepth = 0,
                    recordsPerPage = 100,
                    recordTop = 0,
                    titleExtension = "",
                    wherePairDict = new Dictionary<string, string>()
                });
                IndexConfigClass indexConfig = IndexConfigClass.get(core, gridData);
                var userContentPermissions = PermissionController.getUserContentPermissions(core, ContentMetadataModel.create(core, field.redirectContentId));
                List<string> tmp = default;
                DataSourceModel datasource = DataSourceModel.create(core.cpParent, gridData.adminContent.dataSourceId, ref tmp);
                //
                // Get the SQL parts
                bool AllowAccessToContent = false;
                string ContentAccessLimitMessage = "";
                bool IsLimitedToSubContent = false;
                string sqlWhere = "";
                string sqlOrderBy = "";
                string sqlFieldList = "";
                string sqlFrom = "";
                Dictionary<string, bool> FieldUsedInColumns = new Dictionary<string, bool>(); // used to prevent select SQL from being sorted by a field that does not appear
                Dictionary<string, bool> IsLookupFieldValid = new Dictionary<string, bool>();
                ListView.setIndexSQL(core, gridData, indexConfig, ref AllowAccessToContent, ref sqlFieldList, ref sqlFrom, ref sqlWhere, ref sqlOrderBy, ref IsLimitedToSubContent, ref ContentAccessLimitMessage, ref FieldUsedInColumns, IsLookupFieldValid);
                bool allowAdd = gridData.adminContent.allowAdd && (!IsLimitedToSubContent) && (userContentPermissions.allowAdd);
                bool allowDelete = (gridData.adminContent.allowDelete) && (userContentPermissions.allowDelete);
                if ((!userContentPermissions.allowEdit) || (!AllowAccessToContent)) {
                    //
                    // two conditions should be the same -- but not time to check - This user does not have access to this content
                    ErrorController.addUserError(core, "Your account does not have access to any records in '" + gridData.adminContent.name + "'.");
                    return "Your account does not have access to the requested content";
                } else {
                    //
                    // -- for redirect fields, only include connected records that match the redirect criteria
                    sqlWhere += (string.IsNullOrEmpty(field.redirectId)) ? "" : "and(" + field.redirectId + "=" + editRecord.id + ")";
                    //
                    // Get the total record count
                    string sql = "select count(" + gridData.adminContent.tableName + ".ID) as cnt from " + sqlFrom;
                    if (!string.IsNullOrEmpty(sqlWhere)) {
                        sql += " where " + sqlWhere;
                    }
                    int recordCnt = 0;
                    using (var csData = new CsModel(core)) {
                        if (csData.openSql(sql, datasource.name)) {
                            recordCnt = csData.getInteger("cnt");
                        }
                    }
                    if (recordCnt < 100) {
                        //
                        // -- under 100 records, show them here
                        sql = "select";
                        if (datasource.dbTypeId != DataSourceTypeODBCMySQL) {
                            sql += " Top " + (indexConfig.recordTop + indexConfig.recordsPerPage);
                        }
                        sql += " " + sqlFieldList + " From " + sqlFrom;
                        if (!string.IsNullOrEmpty(sqlWhere)) {
                            sql += " WHERE " + sqlWhere;
                        }
                        if (!string.IsNullOrEmpty(sqlOrderBy)) {
                            sql += " Order By" + sqlOrderBy;
                        }
                        if (datasource.dbTypeId == DataSourceTypeODBCMySQL) {
                            sql += " Limit " + (indexConfig.recordTop + indexConfig.recordsPerPage);
                        }
                        indexConfig.allowDelete = false;
                        indexConfig.allowFind = false;
                        indexConfig.allowAddRow = true;
                        indexConfig.allowColumnSort = false;
                        gridData.wherePair.Add(field.redirectId.ToLower(), editRecord.id.ToString());
                        return ListGridController.get(core, gridData, indexConfig, userContentPermissions, sql, datasource, FieldUsedInColumns, IsLookupFieldValid);
                    } else {
                        //
                        // -- too many rows, setup a redirect
                        if (editRecord.id == 0) {
                            return "[available after save]";
                        }
                        string RedirectPath = (string.IsNullOrEmpty(field.redirectPath)) ? core.appConfig.adminRoute : field.redirectPath;
                        RedirectPath += "?" + RequestNameTitleExtension + "=" + GenericController.encodeRequestVariable(" For " + editRecord.nameLc + adminData.titleExtension) + "&" + RequestNameAdminDepth + "=" + (adminData.ignore_legacyMenuDepth + 1) + "&wl0=" + field.redirectId + "&wr0=" + editRecord.id;
                        if (field.redirectContentId != 0) {
                            RedirectPath += "&cid=" + field.redirectContentId;
                        } else {
                            RedirectPath += "&cid=" + ((editRecord.contentControlId.Equals(0)) ? adminData.adminContent.id : editRecord.contentControlId);
                        }
                        RedirectPath = strReplace(RedirectPath, "'", "\\'");
                        return HtmlController.a("Open in New Window", RedirectPath, "", "", "", "_blank");
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                return HtmlController.div("Ther was an error displaying the related data for this field.");
            }
        }
        //
        // ====================================================================================================
        //
        public static string getGroupRuleEditor(CoreController core, AdminDataModel adminData) {
            try {
                //
                var groupRuleEditor = new GroupRuleEditorModel {
                    listCaption = "Groups",
                    rowList = new List<GroupRuleEditorRowModel>()
                };
                //
                // -- build default Role select
                bool isEmpty = false;
                string RoleSelectDefault = core.html.selectFromContent("{htmlName}", -1, "Group Roles", "", "No Role", "", ref isEmpty, "pt-2 select form-control");
                //
                int GroupCount = 0;
                {
                    //
                    // ----- read in the groups that this member has subscribed (exclude new member records)
                    int[] membershipListGroupId = Array.Empty<int>();
                    DateTime[] membershipListDateExpires = Array.Empty<DateTime>();
                    bool[] membershipListActive = Array.Empty<bool>();
                    int[] membershipListRoleId = Array.Empty<int>();
                    //
                    int membershipCount = 0;
                    if (adminData.editRecord.id != 0) {
                        var memberRuleList = DbBaseModel.createList<MemberRuleModel>(core.cpParent, "memberid=" + adminData.editRecord.id);
                        int membershipSize = 0;
                        foreach (var memberRule in memberRuleList) {
                            if (membershipCount >= membershipSize) {
                                membershipSize = membershipSize + 100;
                                Array.Resize(ref membershipListGroupId, membershipSize + 1);
                                Array.Resize(ref membershipListDateExpires, membershipSize + 1);
                                Array.Resize(ref membershipListActive, membershipSize + 1);
                                Array.Resize(ref membershipListRoleId, membershipSize + 1);
                            }
                            membershipListGroupId[membershipCount] = memberRule.groupId;
                            membershipListDateExpires[membershipCount] = GenericController.encodeDate(memberRule.dateExpires);
                            membershipListActive[membershipCount] = memberRule.active;
                            membershipListRoleId[membershipCount] = memberRule.groupRoleId;
                            membershipCount += 1;
                        }
                    }
                    //
                    // ----- read in all the groups, sorted by ContentName
                    using (var csGroups = new CsModel(core)) {
                        bool canSeeHiddenGroups = core.session.isAuthenticatedDeveloper();
                        csGroups.openSql("select id,name as groupName,caption as groupCaption from ccgroups where (active>0) order by caption,name,id");
                        while (csGroups.ok()) {
                            string GroupName = csGroups.getText("GroupName");
                            if ((GroupName.left(1) != "_") || canSeeHiddenGroups) {
                                string GroupCaption = csGroups.getText("GroupCaption");
                                int GroupID = csGroups.getInteger("ID");
                                if (string.IsNullOrEmpty(GroupCaption)) {
                                    GroupCaption = GroupName;
                                    if (string.IsNullOrEmpty(GroupCaption)) {
                                        GroupCaption = "Group&nbsp;" + GroupID;
                                    }
                                }
                                bool GroupActive = false;
                                DateTime? DateExpire = default;
                                string DateExpireValue = "";
                                int groupRoleId = 0;
                                if (membershipCount != 0) {
                                    for (int MembershipPointer = 0; MembershipPointer < membershipCount; MembershipPointer++) {
                                        if (membershipListGroupId[MembershipPointer] == GroupID) {
                                            GroupActive = membershipListActive[MembershipPointer];
                                            if (membershipListDateExpires[MembershipPointer] > DateTime.MinValue) {
                                                DateExpire = membershipListDateExpires[MembershipPointer];
                                                DateExpireValue = GenericController.encodeText(DateExpire);
                                            }
                                            groupRoleId = membershipListRoleId[MembershipPointer];
                                            break;
                                        }
                                    }
                                }
                                string relatedButtonList = "";
                                relatedButtonList += AdminUIController.getButtonPrimaryAnchor("Edit", "?af=4&cid=" + ContentMetadataModel.getContentId(core, "Groups") + "&id=" + GroupID);
                                relatedButtonList += AdminUIController.getButtonPrimaryAnchor("Members", "?af=1&cid=" + ContentMetadataModel.getContentId(core, "people") + "&IndexFilterAddGroup=" + GenericController.encodeURL(GroupName));
                                //
                                var row = new GroupRuleEditorRowModel {
                                    idHidden = HtmlController.inputHidden("Memberrules." + GroupCount + ".ID", GroupID),
                                    checkboxInput = HtmlController.checkbox("MemberRules." + GroupCount, GroupActive),
                                    groupCaption = GroupCaption,
                                    expiresInput = getDateTimeEditor(core, "MemberRules." + GroupCount + ".DateExpires", DateExpire,false,"",false,""),
                                    //expiresInput = HtmlController.inputDate(core, "MemberRules." + GroupCount + ".DateExpires", DateExpire, "","", "text form-control", false,false,false),
                                    //expiresInput = HtmlController.inputText_Legacy(core, "MemberRules." + GroupCount + ".DateExpires", DateExpireValue, 1, 20, "", false, false, "text form-control", -1, false, "expires"),
                                    relatedButtonList = relatedButtonList,
                                    roleInput = getRoleSelect(core, RoleSelectDefault, groupRoleId, GroupCount)
                                };
                                groupRuleEditor.rowList.Add(row);
                                GroupCount += 1;
                            }
                            csGroups.goNext();
                        }
                    }
                }
                //
                // -- add a row for group count and Add Group button
                groupRuleEditor.rowList.Add(new GroupRuleEditorRowModel {
                    idHidden = HtmlController.inputHidden("MemberRules.RowCount", GroupCount),
                    checkboxInput = AdminUIController.getButtonPrimaryAnchor("Add Group", "?af=4&cid=" + ContentMetadataModel.getContentId(core, "Groups")),
                    groupCaption = "",
                    expiresInput = "",
                    relatedButtonList = "",
                    roleInput = AdminUIController.getButtonPrimaryAnchor("Add Role", "?af=4&cid=" + ContentMetadataModel.getContentId(core, "Group Roles"))
                });
                return Render.StringToString(Resources.GroupRuleEditorRow2, groupRuleEditor);
            } catch (Exception ex) {
                LogController.logError(core, ex);
                return string.Empty;
            }
        }
        //
        // ====================================================================================================
        //
        private static string getRoleSelect(CoreController core, string RoleSelectDefault, int groupRoleId, int GroupCount) {
            string find = "value=\"" + groupRoleId + "\"";
            return RoleSelectDefault
                .Replace("{htmlName}", "MemberRules." + GroupCount + ".RoleId")
                .Replace(find, find + " selected");
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
