﻿
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
using Contensive.Processor.Models.Domain;
using Contensive.Processor.Exceptions;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using Contensive.Processor.Addons.Tools;
using System.Collections.Generic;

namespace Contensive.Processor.Addons.AdminSite {
    public class GetHtmlBodyClass : AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cpBase) {
            string result = "";
            CPClass cp = (CPClass)cpBase;
            try {
                if (!cp.core.session.isAuthenticated) {
                    //
                    // --- must be authenticated to continue. Force a local login
                    return cp.core.addon.execute(addonGuidLoginPage, new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                        errorContextMessage = "get Login Page for Html Body",
                        addonType = CPUtilsBaseClass.addonContext.ContextPage
                    });
                }
                if (!cp.core.session.isAuthenticatedContentManager()) {
                    //
                    // --- member must have proper access to continue
                    result += ""
                        + "<p>You are attempting to enter an area which your account does not have access.</p>"
                        + "<ul class=\"ccList\">"
                        + "<li class=\"ccListItem\">To return to the public web site, use your back button, or <a href=\"" + "/" + "\">Click Here</A>."
                        + "<li class=\"ccListItem\">To login under a different account, <a href=\"/" + cp.core.appConfig.adminRoute + "?method=logout\" rel=\"nofollow\">Click Here</A>"
                        + "<li class=\"ccListItem\">To have your account access changed to include this area, please contact the <a href=\"mailto:" + cp.core.siteProperties.getText("EmailAdmin") + "\">system administrator</A>. "
                        + "\r</ul>"
                        + "";
                    result += ""
                        + "<div style=\"display:table;padding:100px 0 0 0;margin:0 auto;\">"
                        + cp.core.html.getPanelHeader("Unauthorized Access")
                        + cp.core.html.getPanel(result, "ccPanel", "ccPanelHilite", "ccPanelShadow", "400", 15)
                        + "</div>";
                    cp.core.doc.setMetaContent(0, 0);
                    cp.core.html.addTitle("Unauthorized Access", "adminSite");
                    return HtmlController.div(result, "container-fluid ccBodyAdmin ccCon");
                }
                //
                // get admin content
                result += getHtmlBody(cp);
                result = HtmlController.div(result, "container-fluid ccBodyAdmin ccCon");


            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// return the html body for the admin site
        /// </summary>
        /// <param name="forceAdminContent"></param>
        /// <returns></returns>
        private string getHtmlBody(CPClass cp, string forceAdminContent = "") {
            string result = "";
            try {
                // todo convert to jquery bind
                cp.core.doc.setMetaContent(0, 0);
                //
                // turn off chrome protection against submitting html content
                cp.core.webServer.addResponseHeader("X-XSS-Protection", "0");
                //
                // check for member login, if logged in and no admin, lock out, Do CheckMember here because we need to know who is there to create proper blocked menu
                if (cp.core.doc.continueProcessing) {
                    //
                    // -- read wl+wr values into wherePair dictionary
                    // -- wherepairs are used to:
                    // ---- prepopulate inserted records
                    // ---- create filters for gridList
                    // -- wherepair wr0=id, wl=1 (means id=1)
                    Dictionary<string, string> wherePairs = new Dictionary<string, string>();
                    for (int wpCnt = 0; wpCnt <= 99; wpCnt++) {
                        string key = cp.Doc.GetText("wl" + wpCnt);
                        if (string.IsNullOrEmpty(key)) { break; }
                        wherePairs.Add(key.ToLower(), cp.Doc.GetText("wr" + wpCnt));
                    }
                    //
                    // -- read wc (whereclause) into wherepair dictionary also
                    // -- whereclause wc=id%3D1 (means id=1)
                    string WhereClauseContent = GenericController.encodeText(cp.Doc.GetText("wc"));
                    if (!string.IsNullOrEmpty(WhereClauseContent)) {
                        string[] QSSplit = WhereClauseContent.Split(',');
                        for (int QSPointer = 0; QSPointer <= QSSplit.GetUpperBound(0); QSPointer++) {
                            string NameValue = QSSplit[QSPointer];
                            if (!string.IsNullOrEmpty(NameValue)) {
                                if ((NameValue.left(1) == "(") && (NameValue.Substring(NameValue.Length - 1) == ")") && (NameValue.Length > 2)) {
                                    NameValue = NameValue.Substring(1, NameValue.Length - 2);
                                }
                                string[] NVSplit = NameValue.Split('=');
                                if (NVSplit.GetUpperBound(0) > 0) {
                                    wherePairs.Add(NVSplit[0].ToLower(), NVSplit[1]);
                                }
                            }
                        }
                    }

                    int adminForm = cp.Doc.GetInteger(rnAdminForm);
                    var adminData = new AdminDataModel(cp.core, new AdminDataRequest() {
                        contentId = cp.Doc.GetInteger("cid"),
                        id = cp.Doc.GetInteger("id"),
                        guid = cp.Doc.GetText("guid"),
                        titleExtension = cp.Doc.GetText(RequestNameTitleExtension),
                        recordTop = cp.Doc.GetInteger("RT"),
                        recordsPerPage = cp.Doc.GetInteger("RS"),
                        wherePairDict = wherePairs,
                        adminAction = cp.Doc.GetInteger(rnAdminAction),
                        adminSourceForm = cp.Doc.GetInteger(rnAdminSourceForm),
                        adminForm = adminForm,
                        adminButton = cp.Doc.GetText(RequestNameButton),
                        ignore_legacyMenuDepth = (adminForm == AdminFormEdit) ? 0 : cp.Doc.GetInteger(RequestNameAdminDepth),
                        fieldEditorPreference = cp.Doc.GetText("fieldEditorPreference")
                    });
                    cp.core.db.sqlCommandTimeout = 300;
                    adminData.buttonObjectCount = 0;
                    adminData.javaScriptString = "";
                    adminData.contentWatchLoaded = false;
                    //
                    string buildVersion = cp.core.siteProperties.dataBuildVersion;
                    if (string.Compare(buildVersion, cp.Version) < 0) {
                        LogController.logWarn(cp.core, new GenericException("Application code (v" + cp.Version + ") is newer than database (v" + cp.core.siteProperties.dataBuildVersion + "). Upgrade the database with the command line 'cc.exe -a " + cp.core.appConfig.name + " -u'."));
                    }
                    //
                    if (string.Compare(buildVersion, cp.Version) > 0) {
                        LogController.logWarn(cp.core, new GenericException("Database upgrade (v" + cp.core.siteProperties.dataBuildVersion + ") is newer than the Application code (v" + cp.Version + "). Upgrade the website code."));
                    }
                    //
                    // Process SourceForm/Button into Action/Form, and process
                    if (adminData.requestButton == ButtonCancelAll) {
                        adminData.adminForm = AdminFormRoot;
                    } else {
                        ProcessForms(cp, adminData);
                        ProcessActionController.processActions(cp, adminData, cp.core.siteProperties.useContentWatchLink);
                    }
                    //
                    // Normalize values to be needed
                    if (adminData.editRecord.id != 0) {
                        var table = DbBaseModel.createByUniqueName<TableModel>(cp, adminData.adminContent.tableName);
                        if (table != null) {
                            WorkflowController.clearEditLock(cp.core, table.id, adminData.editRecord.id);
                        }
                    }
                    if (adminData.adminForm < 1) {
                        //
                        // No form was set, use default form
                        if (adminData.adminContent.id <= 0) {
                            adminData.adminForm = AdminFormRoot;
                        } else {
                            adminData.adminForm = AdminFormIndex;
                        }
                    }
                    int addonId = cp.core.docProperties.getInteger("addonid");
                    string AddonGuid = cp.core.docProperties.getText("addonguid");
                    if (adminData.adminForm == AdminFormLegacyAddonManager) {
                        //
                        // patch out any old links to the legacy addon manager
                        adminData.adminForm = 0;
                        AddonGuid = addonGuidAddonManager;
                    }
                    //
                    //-------------------------------------------------------------------------------
                    // Edit form but not valid record case
                    // Put this here so we can display the error without being stuck displaying the edit form
                    // Putting the error on the edit form is confusing because there are fields to fill in
                    //-------------------------------------------------------------------------------
                    //
                    if (adminData.adminSourceForm == AdminFormEdit) {
                        if (cp.core.doc.userErrorList.Count.Equals(0) && (adminData.requestButton.Equals(ButtonOK) || adminData.requestButton.Equals(ButtonCancel) || adminData.requestButton.Equals(ButtonDelete))) {
                            string EditReferer = cp.core.docProperties.getText("EditReferer");
                            string CurrentLink = GenericController.modifyLinkQuery(cp.core.webServer.requestUrl, "editreferer", "", false);
                            CurrentLink = GenericController.toLCase(CurrentLink);
                            //
                            // check if this editreferer includes cid=thisone and id=thisone -- if so, go to index form for this cid
                            //
                            if ((!string.IsNullOrEmpty(EditReferer)) && (EditReferer.ToLowerInvariant() != CurrentLink)) {
                                //
                                // return to the page it came from
                                //
                                return cp.core.webServer.redirect(EditReferer, "Admin Edit page returning to the EditReferer setting");
                            } else {
                                //
                                // return to the index page for this content
                                //
                                adminData.adminForm = AdminFormIndex;
                            }
                        }
                        if (adminData.blockEditForm) {
                            adminData.adminForm = AdminFormIndex;
                        }
                    }
                    int HelpLevel = cp.core.docProperties.getInteger("helplevel");
                    int HelpAddonId = cp.core.docProperties.getInteger("helpaddonid");
                    int HelpCollectionId = cp.core.docProperties.getInteger("helpcollectionid");
                    if (HelpCollectionId == 0) {
                        HelpCollectionId = cp.core.visitProperty.getInteger("RunOnce HelpCollectionID");
                        if (HelpCollectionId != 0) {
                            cp.core.visitProperty.setProperty("RunOnce HelpCollectionID", "");
                        }
                    }
                    //
                    //-------------------------------------------------------------------------------
                    // build refresh string
                    //-------------------------------------------------------------------------------
                    //
                    if (adminData.adminContent.id != 0) {
                        cp.core.doc.addRefreshQueryString("cid", GenericController.encodeText(adminData.adminContent.id));
                    }
                    if (adminData.editRecord.id != 0) {
                        cp.core.doc.addRefreshQueryString("id", GenericController.encodeText(adminData.editRecord.id));
                    }
                    if (adminData.titleExtension != "") {
                        cp.core.doc.addRefreshQueryString(RequestNameTitleExtension, GenericController.encodeRequestVariable(adminData.titleExtension));
                    }
                    if (adminData.recordTop != 0) {
                        cp.core.doc.addRefreshQueryString("rt", GenericController.encodeText(adminData.recordTop));
                    }
                    if (adminData.recordsPerPage != Constants.RecordsPerPageDefault) {
                        cp.core.doc.addRefreshQueryString("rs", GenericController.encodeText(adminData.recordsPerPage));
                    }
                    if (adminData.adminForm != 0) {
                        cp.core.doc.addRefreshQueryString(rnAdminForm, GenericController.encodeText(adminData.adminForm));
                    }
                    if (adminData.ignore_legacyMenuDepth != 0) {
                        cp.core.doc.addRefreshQueryString(RequestNameAdminDepth, GenericController.encodeText(adminData.ignore_legacyMenuDepth));
                    }
                    //
                    // normalize guid
                    //
                    if (!string.IsNullOrEmpty(AddonGuid)) {
                        if ((AddonGuid.Length == 38) && (AddonGuid.left(1) == "{") && (AddonGuid.Substring(AddonGuid.Length - 1) == "}")) {
                            //
                            // Good to go
                            //
                        } else if (AddonGuid.Length == 36) {
                            //
                            // might be valid with the brackets, add them
                            //
                            AddonGuid = "{" + AddonGuid + "}";
                        } else if (AddonGuid.Length == 32) {
                            //
                            // might be valid with the brackets and the dashes, add them
                            //
                            AddonGuid = "{" + AddonGuid.left(8) + "-" + AddonGuid.Substring(8, 4) + "-" + AddonGuid.Substring(12, 4) + "-" + AddonGuid.Substring(16, 4) + "-" + AddonGuid.Substring(20) + "}";
                        } else {
                            //
                            // not valid
                            //
                            AddonGuid = "";
                        }
                    }
                    //
                    //-------------------------------------------------------------------------------
                    // Create the content
                    //-------------------------------------------------------------------------------
                    //
                    string adminBody = "";
                    StringBuilderLegacyController Stream = new StringBuilderLegacyController();
                    string AddonName = "";
                    if (!string.IsNullOrEmpty(forceAdminContent)) {
                        //
                        // Use content passed in as an argument
                        //
                        adminBody = forceAdminContent;
                    } else if (HelpAddonId != 0) {
                        //
                        // display Addon Help
                        //
                        cp.core.doc.addRefreshQueryString("helpaddonid", HelpAddonId.ToString());
                        adminBody = GetAddonHelp(cp, HelpAddonId, "");
                    } else if (HelpCollectionId != 0) {
                        //
                        // display Collection Help
                        //
                        cp.core.doc.addRefreshQueryString("helpcollectionid", HelpCollectionId.ToString());
                        adminBody = GetCollectionHelp(cp, HelpCollectionId, "");
                    } else if (adminData.adminForm != 0) {
                        //
                        // -- formindex requires valkid content
                        if ((adminData.adminContent.tableName == null) && ((adminData.adminForm == AdminFormIndex) || (adminData.adminForm == AdminFormIndex))) { adminData.adminForm = AdminFormRoot; }
                        //
                        // No content so far, try the forms
                        // todo - convert this to switch
                        switch (adminData.adminForm) {
                            //
                            case AdminFormIndex: {
                                    adminBody = ListView.get(cp, cp.core, adminData, (adminData.adminContent.tableName.ToLowerInvariant() == "ccemail"));
                                    break;
                                }
                            case AdminFormEdit: {
                                    adminBody = EditView.get(cp.core, adminData);
                                    break;
                                }
                            case AdminFormToolSyncTables: {
                                    adminBody = SyncTablesClass.get(cp.core);
                                    break;
                                }
                            case AdminFormToolSchema: {
                                    adminBody = DbSchemaToolClass.get(cp.core);
                                    break;
                                }
                            case AdminFormToolDbIndex: {
                                    adminBody = DbIndexToolClass.get(cp.core);
                                    break;
                                }
                            case AdminformToolFindAndReplace: {
                                    adminBody = FindAndReplaceToolClass.get(cp.core);
                                    break;
                                }
                            case AdminformToolCreateGUID: {
                                    adminBody = CreateGUIDToolClass.get(cp.core);
                                    break;
                                }
                            case AdminformToolIISReset: {
                                    adminBody = IISResetToolClass.get(cp.core);
                                    break;
                                }
                            case AdminFormToolContentSchema: {
                                    adminBody = ContentSchemaToolClass.get(cp.core);
                                    break;
                                }
                            case AdminFormToolManualQuery: {
                                    adminBody = ManualQueryClass.get(cp);
                                    break;
                                }
                            case AdminFormToolDefineContentFieldsFromTable: {
                                    adminBody = DefineContentFieldsFromTableClass.get(cp.core);
                                    break;
                                }
                            case AdminFormToolCreateContentDefinition: {
                                    adminBody = CreateContentDefinitionClass.get(cp.core);
                                    break;
                                }
                            case AdminFormToolConfigureEdit: {
                                    adminBody = ConfigureEditClass.get(cp);
                                    break;
                                }
                            case AdminFormToolConfigureListing: {
                                    adminBody = ConfigureListClass.get(cp.core);
                                    break;
                                }
                            case AdminFormClearCache: {
                                    adminBody = cp.core.addon.execute("{7B5B8150-62BE-40F4-A66A-7CC74D99BA76}", new CPUtilsBaseClass.addonExecuteContext() {
                                        addonType = CPUtilsBaseClass.addonContext.ContextAdmin
                                    });
                                    break;
                                }
                            case AdminFormResourceLibrary: {
                                    adminBody = cp.core.html.getResourceLibrary("", false, "", "", true);
                                    break;
                                }
                            case AdminFormQuickStats: {
                                    adminBody = QuickStatsView.get(cp.core);
                                    break;
                                }
                            case AdminFormClose: {
                                    Stream.add("<Script Language=\"JavaScript\" type=\"text/javascript\"> window.close(); </Script>");
                                    break;
                                }
                            case AdminFormContentChildTool: {
                                    adminBody = ContentChildToolClass.get(cp);
                                    break;
                                }
                            case AdminformHousekeepingControl: {
                                    adminBody = HouseKeepingControlClass.get(cp);
                                    break;
                                }
                            case AdminFormDownloads: {
                                    adminBody = (ToolDownloads.get(cp.core));
                                    break;
                                }
                            case AdminFormImportWizard: {
                                    adminBody = cp.core.addon.execute(addonGuidImportWizard, new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                                        addonType = BaseClasses.CPUtilsBaseClass.addonContext.ContextAdmin,
                                        errorContextMessage = "get Import Wizard for Admin"
                                    });
                                    break;
                                }
                            case AdminFormCustomReports: {
                                    adminBody = ToolCustomReports.get(cp.core);
                                    break;
                                }
                            case AdminFormFormWizard: {
                                    adminBody = cp.core.addon.execute(addonGuidFormWizard, new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                                        addonType = BaseClasses.CPUtilsBaseClass.addonContext.ContextAdmin,
                                        errorContextMessage = "get Form Wizard for Admin"
                                    });
                                    break;
                                }
                            case AdminFormLegacyAddonManager: {
                                    adminBody = AddonController.getAddonManager(cp.core);
                                    break;
                                }
                            case AdminFormEditorConfig: {
                                    adminBody = EditorConfigView.get(cp.core);
                                    break;
                                }
                            default: {
                                    adminBody = "<p>The form requested is not supported</p>";
                                    break;
                                }
                        }
                    } else if ((addonId != 0) || (!string.IsNullOrEmpty(AddonGuid)) || (!string.IsNullOrEmpty(AddonName))) {
                        //
                        // execute an addon
                        //
                        if ((AddonGuid == addonGuidAddonManager) || (AddonName.ToLowerInvariant() == "add-on manager") || (AddonName.ToLowerInvariant() == "addon manager")) {
                            //
                            // Special case, call the routine that provides a backup
                            //
                            cp.core.doc.addRefreshQueryString("addonguid", addonGuidAddonManager);
                            adminBody = AddonController.getAddonManager(cp.core);
                        } else {
                            AddonModel addon = null;
                            string executeContextErrorCaption = "unknown";
                            if (addonId != 0) {
                                executeContextErrorCaption = " addon id:" + addonId + " for Admin";
                                cp.core.doc.addRefreshQueryString("addonid", addonId.ToString());
                                addon = DbBaseModel.create<AddonModel>(cp, addonId);
                            } else if (!string.IsNullOrEmpty(AddonGuid)) {
                                executeContextErrorCaption = "addon guid:" + AddonGuid + " for Admin";
                                cp.core.doc.addRefreshQueryString("addonguid", AddonGuid);
                                addon = DbBaseModel.create<AddonModel>(cp, AddonGuid);
                            } else if (!string.IsNullOrEmpty(AddonName)) {
                                executeContextErrorCaption = "addon name:" + AddonName + " for Admin";
                                cp.core.doc.addRefreshQueryString("addonname", AddonName);
                                addon = AddonModel.createByUniqueName(cp, AddonName);
                            }
                            if (addon != null) {
                                addonId = addon.id;
                                AddonName = addon.name;
                                string AddonHelpCopy = addon.help;
                                cp.core.doc.addRefreshQueryString(RequestNameRunAddon, addonId.ToString());
                            }
                            string InstanceOptionString = cp.core.userProperty.getText("Addon [" + AddonName + "] Options", "");
                            int DefaultWrapperId = -1;
                            adminBody = cp.core.addon.execute(addon, new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                                addonType = Contensive.BaseClasses.CPUtilsBaseClass.addonContext.ContextAdmin,
                                instanceGuid = adminSiteInstanceId,
                                argumentKeyValuePairs = GenericController.convertQSNVAArgumentstoDocPropertiesList(cp.core, InstanceOptionString),
                                wrapperID = DefaultWrapperId,
                                errorContextMessage = executeContextErrorCaption
                            });
                            if (string.IsNullOrEmpty(adminBody)) {
                                //
                                // empty returned, display desktop
                                adminBody = RootView.getForm_Root(cp.core);
                            }

                        }
                    } else {
                        //
                        // nothing so far, display desktop
                        adminBody = RootView.getForm_Root(cp.core);
                    }
                    //
                    // add user errors
                    if (!cp.core.doc.userErrorList.Count.Equals(0)) {
                        adminBody = HtmlController.div(Processor.Controllers.ErrorController.getUserError(cp.core), "ccAdminMsg") + adminBody;
                    }
                    Stream.add(getAdminHeader(cp, adminData));
                    Stream.add(adminBody);
                    Stream.add(adminData.adminFooter);
                    adminData.javaScriptString += "ButtonObjectCount = " + adminData.buttonObjectCount + ";";
                    cp.core.html.addScriptCode(adminData.javaScriptString, "Admin Site");
                    result = Stream.text;
                }
                if (cp.core.session.user.developer) {
                    result = Processor.Controllers.ErrorController.getDocExceptionHtmlList(cp.core) + result;
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        //
        private string GetAddonHelp(CPClass cp, int HelpAddonID, string UsedIDString) {
            string addonHelp = "";
            try {
                string IconFilename = null;
                int IconWidth = 0;
                int IconHeight = 0;
                int IconSprites = 0;
                bool IconIsInline = false;
                string AddonName = "";
                string AddonHelpCopy = "";
                DateTime AddonDateAdded = default(DateTime);
                DateTime AddonLastUpdated = default(DateTime);
                string IncludeHelp = "";
                string IconImg = "";
                string helpLink = "";
                bool FoundAddon = false;
                //
                if (GenericController.strInstr(1, "," + UsedIDString + ",", "," + HelpAddonID + ",") == 0) {
                    using (var csData = new CsModel(cp.core)) {
                        csData.openRecord(AddonModel.tableMetadata.contentName, HelpAddonID);
                        if (csData.ok()) {
                            FoundAddon = true;
                            AddonName = csData.getText("Name");
                            AddonHelpCopy = csData.getText("help");
                            AddonDateAdded = csData.getDate("dateadded");
                            AddonLastUpdated = csData.getDate("lastupdated");
                            if (AddonLastUpdated == DateTime.MinValue) {
                                AddonLastUpdated = AddonDateAdded;
                            }
                            IconFilename = csData.getText("Iconfilename");
                            IconWidth = csData.getInteger("IconWidth");
                            IconHeight = csData.getInteger("IconHeight");
                            IconSprites = csData.getInteger("IconSprites");
                            IconIsInline = csData.getBoolean("IsInline");
                            IconImg = AddonController.getAddonIconImg("/" + cp.core.appConfig.adminRoute, IconWidth, IconHeight, IconSprites, IconIsInline, "", IconFilename, cp.core.appConfig.cdnFileUrl, AddonName, AddonName, "", 0);
                            helpLink = csData.getText("helpLink");
                        }
                    }
                    //
                    if (FoundAddon) {
                        //
                        // Included Addons
                        //
                        foreach (var addonon in cp.core.addonCache.getDependsOnList(HelpAddonID)) {
                            IncludeHelp += GetAddonHelp(cp, addonon.id, HelpAddonID + "," + addonon.id.ToString());
                        }
                        if (!string.IsNullOrEmpty(helpLink)) {
                            if (!string.IsNullOrEmpty(AddonHelpCopy)) {
                                AddonHelpCopy = AddonHelpCopy + "<p>For additional help with this add-on, please visit <a href=\"" + helpLink + "\">" + helpLink + "</a>.</p>";
                            } else {
                                AddonHelpCopy = AddonHelpCopy + "<p>For help with this add-on, please visit <a href=\"" + helpLink + "\">" + helpLink + "</a>.</p>";
                            }
                        }
                        if (string.IsNullOrEmpty(AddonHelpCopy)) {
                            AddonHelpCopy = AddonHelpCopy + "<p>Please refer to the help resources available for this collection. More information may also be available in the Contensive online Learning Center <a href=\"http://support.contensive.com/Learning-Center\">http://support.contensive.com/Learning-Center</a> or contact Contensive Support support@contensive.com for more information.</p>";
                        }
                        addonHelp = ""
                            + "<div class=\"ccHelpCon\">"
                            + "<div class=\"title\"><div style=\"float:right;\"><a href=\"?addonid=" + HelpAddonID + "\">" + IconImg + "</a></div>" + AddonName + " Add-on</div>"
                            + "<div class=\"byline\">"
                                + "<div>Installed " + AddonDateAdded + "</div>"
                                + "<div>Last Updated " + AddonLastUpdated + "</div>"
                            + "</div>"
                            + "<div class=\"body\" style=\"clear:both;\">" + AddonHelpCopy + "</div>"
                            + "</div>";
                        addonHelp = addonHelp + IncludeHelp;
                    }
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
                throw;
            }
            return addonHelp;
        }
        //
        //====================================================================================================
        //
        private string GetCollectionHelp(CPClass cp, int HelpCollectionID, string UsedIDString) {
            string returnHelp = "";
            try {
                string Collectionname = "";
                string CollectionHelpCopy = "";
                string CollectionHelpLink = "";
                DateTime CollectionDateAdded = default(DateTime);
                DateTime CollectionLastUpdated = default(DateTime);
                string IncludeHelp = "";
                //
                if (GenericController.strInstr(1, "," + UsedIDString + ",", "," + HelpCollectionID + ",") == 0) {
                    using (var csData = new CsModel(cp.core)) {
                        csData.openRecord("Add-on Collections", HelpCollectionID);
                        if (csData.ok()) {
                            Collectionname = csData.getText("Name");
                            CollectionHelpCopy = csData.getText("help");
                            CollectionDateAdded = csData.getDate("dateadded");
                            CollectionLastUpdated = csData.getDate("lastupdated");
                            CollectionHelpLink = csData.getText("helplink");
                            if (CollectionLastUpdated == DateTime.MinValue) {
                                CollectionLastUpdated = CollectionDateAdded;
                            }
                        }
                    }
                    //
                    // Add-ons
                    //
                    using (var csData = new CsModel(cp.core)) {
                        csData.open(AddonModel.tableMetadata.contentName, "CollectionID=" + HelpCollectionID, "name");
                        while (csData.ok()) {
                            IncludeHelp = IncludeHelp + "<div style=\"clear:both;\">" + GetAddonHelp(cp, csData.getInteger("ID"), "") + "</div>";
                            csData.goNext();
                        }
                    }
                    //
                    if ((string.IsNullOrEmpty(CollectionHelpLink)) && (string.IsNullOrEmpty(CollectionHelpCopy))) {
                        CollectionHelpCopy = "<p>No help information could be found for this collection. Please use the online resources at <a href=\"http://support.contensive.com/Learning-Center\">http://support.contensive.com/Learning-Center</a> or contact Contensive Support support@contensive.com by email.</p>";
                    } else if (!string.IsNullOrEmpty(CollectionHelpLink)) {
                        CollectionHelpCopy = ""
                            + "<p>For information about this collection please visit <a href=\"" + CollectionHelpLink + "\">" + CollectionHelpLink + "</a>.</p>"
                            + CollectionHelpCopy;
                    }
                    //
                    returnHelp = ""
                        + "<div class=\"ccHelpCon\">"
                        + "<div class=\"title\">" + Collectionname + " Collection</div>"
                        + "<div class=\"byline\">"
                            + "<div>Installed " + CollectionDateAdded + "</div>"
                            + "<div>Last Updated " + CollectionLastUpdated + "</div>"
                        + "</div>"
                        + "<div class=\"body\">" + CollectionHelpCopy + "</div>";
                    if (!string.IsNullOrEmpty(IncludeHelp)) {
                        returnHelp = returnHelp + IncludeHelp;
                    }
                    returnHelp = returnHelp + "</div>";
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
                throw;
            }
            return returnHelp;
        }
        //
        //==============================================================================================
        /// <summary>
        /// If this field has no help message, check the field with the same name from it's inherited parent
        /// </summary>
        /// <param name="ContentID"></param>
        /// <param name="FieldName"></param>
        /// <param name="return_Default"></param>
        /// <param name="return_Custom"></param>
        private void getFieldHelpMsgs(CPClass cp, int ContentID, string FieldName, ref string return_Default, ref string return_Custom) {
            try {
                //
                string SQL = null;
                bool Found = false;
                int ParentId = 0;
                //
                Found = false;
                using (var csData = new CsModel(cp.core)) {
                    SQL = "select h.HelpDefault,h.HelpCustom from ccfieldhelp h left join ccfields f on f.id=h.fieldid where f.contentid=" + ContentID + " and f.name=" + DbController.encodeSQLText(FieldName);
                    csData.openSql(SQL);
                    if (csData.ok()) {
                        Found = true;
                        return_Default = csData.getText("helpDefault");
                        return_Custom = csData.getText("helpCustom");
                    }
                }
                //
                if (!Found) {
                    ParentId = 0;
                    using (var csData = new CsModel(cp.core)) {
                        SQL = "select parentid from cccontent where id=" + ContentID;
                        csData.openSql(SQL);
                        if (csData.ok()) {
                            ParentId = csData.getInteger("parentid");
                        }
                    }
                    if (ParentId != 0) {
                        getFieldHelpMsgs(cp, ParentId, FieldName, ref return_Default, ref return_Custom);
                    }
                }
                //
                return;
                //
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
        }
        // 
        //   Save Whats New values if present
        //
        //   does NOT check AuthoringLocked -- you must check before calling
        //========================================================================
        //
        ////
        //========================================================================
        // GetForm_Top
        //   Prints the admin page before the content form window.
        //   After this, print the content window, then PrintFormBottom()
        //========================================================================
        //
        private string getAdminHeader(CPClass cp, AdminDataModel adminData, string BackgroundColor = "") {
            string result = "";
            try {
                string leftSide = cp.core.siteProperties.getText("AdminHeaderHTML", "Administration Site");
                string rightSide = HtmlController.a(cp.User.Name, "?af=4&cid=" + cp.Content.GetID("people") + "&id=" + cp.User.Id);
                string rightSideNavHtml = ""
                    + "<form class=\"form-inline\" method=post action=\"?method=logout\">"
                    + "<button class=\"btn btn-warning btn-sm ml-2\" type=\"submit\">Logout</button>"
                    + "</form>";
                //
                // Assemble header
                //
                StringBuilderLegacyController Stream = new StringBuilderLegacyController();
                Stream.add(AdminUIController.getHeader(cp.core, leftSide, rightSide, rightSideNavHtml));
                //
                // --- Content Definition
                adminData.adminFooter = "";
                //
                // -- Admin Navigator
                string AdminNavFull = cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, AdminNavigatorGuid), new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                    addonType = BaseClasses.CPUtilsBaseClass.addonContext.ContextAdmin,
                    errorContextMessage = "executing Admin Navigator in Admin"
                });
                //
                Stream.add("<table border=0 cellpadding=0 cellspacing=0><tr>\r<td class=\"ccToolsCon\" valign=top>" + AdminNavFull + "</td>\r<td id=\"desktop\" class=\"ccContentCon\" valign=top>");
                adminData.adminFooter = adminData.adminFooter + "</td></tr></table>";
                //
                result = Stream.text;
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
                throw;
            }
            return result;
        }
        //
        //========================================================================
        // Get Menu Link
        //========================================================================
        //
        private string GetMenuLink(CPClass cp, string LinkPage, int LinkCID) {
            string tempGetMenuLink = null;
            try {
                //
                int ContentId = 0;
                //
                if (!string.IsNullOrEmpty(LinkPage) || (LinkCID != 0)) {
                    tempGetMenuLink = LinkPage;
                    if (!string.IsNullOrEmpty(tempGetMenuLink)) {
                        if (tempGetMenuLink.left(1) == "?" || tempGetMenuLink.left(1) == "#") {
                            tempGetMenuLink = "/" + cp.core.appConfig.adminRoute + tempGetMenuLink;
                        }
                    } else {
                        tempGetMenuLink = "/" + cp.core.appConfig.adminRoute;
                    }
                    ContentId = GenericController.encodeInteger(LinkCID);
                    if (ContentId != 0) {
                        tempGetMenuLink = GenericController.modifyLinkQuery(tempGetMenuLink, "cid", ContentId.ToString(), true);
                    }
                }
                return tempGetMenuLink;
                //
                // ----- Error Trap
                //
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
            return tempGetMenuLink;
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminData.content"></param>
        /// <param name="editRecord"></param>
        //

        private void ProcessForms(CPClass cp, AdminDataModel adminData) {
            try {
                // todo
                Contensive.Processor.Addons.AdminSite.Models.EditRecordModel editRecord = adminData.editRecord;
                //
                //
                if (adminData.adminSourceForm != 0) {
                    string EditorStyleRulesFilename = null;
                    switch (adminData.adminSourceForm) {
                        case AdminFormReports: {
                                //
                                // Reports form cancel button
                                //
                                switch (adminData.requestButton) {
                                    case ButtonCancel: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = AdminFormRoot;
                                            break;
                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                break;
                            }
                        case AdminFormQuickStats: {
                                switch (adminData.requestButton) {
                                    case ButtonCancel: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = AdminFormRoot;
                                            break;
                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                break;
                            }
                        case AdminFormPublishing: {
                                //
                                // Publish Form
                                //
                                switch (adminData.requestButton) {
                                    case ButtonCancel: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = AdminFormRoot;
                                            break;
                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                break;
                            }
                        case AdminFormIndex: {

                                switch (adminData.requestButton) {
                                    case ButtonCancel: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = AdminFormRoot;
                                            adminData.adminContent = new ContentMetadataModel();
                                            break;
                                        }
                                    case ButtonClose: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = AdminFormRoot;
                                            adminData.adminContent = new ContentMetadataModel();
                                            break;
                                        }
                                    case ButtonAdd: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = AdminFormEdit;
                                            break;
                                        }

                                    case ButtonFind: {
                                            adminData.admin_Action = Constants.AdminActionFind;
                                            adminData.adminForm = adminData.adminSourceForm;
                                            break;
                                        }

                                    case ButtonFirst: {
                                            adminData.recordTop = 0;
                                            adminData.adminForm = adminData.adminSourceForm;
                                            break;
                                        }
                                    case ButtonPrevious: {
                                            adminData.recordTop = adminData.recordTop - adminData.recordsPerPage;
                                            if (adminData.recordTop < 0) {
                                                adminData.recordTop = 0;
                                            }
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = adminData.adminSourceForm;
                                            break;
                                        }

                                    case ButtonNext: {
                                            adminData.admin_Action = Constants.AdminActionNext;
                                            adminData.adminForm = adminData.adminSourceForm;
                                            break;
                                        }

                                    case ButtonDelete: {
                                            adminData.admin_Action = Constants.AdminActionDeleteRows;
                                            adminData.adminForm = adminData.adminSourceForm;
                                            break;
                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                // end case
                                break;

                            }
                        case AdminFormEdit: {
                                //
                                // Edit Form
                                //
                                switch (adminData.requestButton) {
                                    case ButtonRefresh: {
                                            //
                                            // this is a test operation. need this so the user can set editor preferences without saving the record
                                            //   during refresh, the edit page is redrawn just was it was, but no save
                                            //
                                            adminData.admin_Action = Constants.AdminActionEditRefresh;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }

                                    case ButtonMarkReviewed: {
                                            adminData.admin_Action = Constants.AdminActionMarkReviewed;
                                            adminData.adminForm = GetForm_Close(cp, adminData.ignore_legacyMenuDepth, adminData.adminContent.name, editRecord.id);
                                            break;

                                        }

                                    case ButtonSaveandInvalidateCache: {
                                            adminData.admin_Action = Constants.AdminActionReloadCDef;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }

                                    case ButtonDelete: {
                                            adminData.admin_Action = Constants.AdminActionDelete;
                                            adminData.adminForm = GetForm_Close(cp, adminData.ignore_legacyMenuDepth, adminData.adminContent.name, editRecord.id);
                                            break;

                                        }

                                    case ButtonSave: {
                                            adminData.admin_Action = Constants.AdminActionSave;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }

                                    case ButtonSaveAddNew: {
                                            adminData.admin_Action = Constants.AdminActionSaveAddNew;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }

                                    case ButtonOK: {
                                            adminData.admin_Action = Constants.AdminActionSave;
                                            adminData.adminForm = GetForm_Close(cp, adminData.ignore_legacyMenuDepth, adminData.adminContent.name, editRecord.id);
                                            break;

                                        }

                                    case ButtonCancel: {
                                            adminData.admin_Action = Constants.AdminActionNop;
                                            adminData.adminForm = GetForm_Close(cp, adminData.ignore_legacyMenuDepth, adminData.adminContent.name, editRecord.id);
                                            break;

                                        }

                                    case ButtonSend: {
                                            //
                                            // Send a Group Email
                                            //
                                            adminData.admin_Action = Constants.AdminActionSendEmail;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }

                                    case ButtonActivate: {
                                            //
                                            // Activate (submit) a conditional Email
                                            //
                                            adminData.admin_Action = Constants.AdminActionActivateEmail;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }
                                    case ButtonDeactivate: {
                                            //
                                            // Deactivate (clear submit) a conditional Email
                                            //
                                            adminData.admin_Action = Constants.AdminActionDeactivateEmail;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }
                                    case ButtonSendTest: {
                                            //
                                            // Test an Email (Group, System, or Conditional)
                                            //
                                            adminData.admin_Action = Constants.AdminActionSendEmailTest;
                                            adminData.adminForm = AdminFormEdit;
                                            break;
                                        }
                                    case ButtonCreateDuplicate: {
                                            //
                                            // Create a Duplicate record (for email)
                                            //
                                            adminData.admin_Action = Constants.AdminActionDuplicate;
                                            adminData.adminForm = AdminFormEdit;
                                            break;

                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                break;
                            }
                        case AdminFormStyleEditor: {
                                //
                                // Process actions
                                //
                                switch (adminData.requestButton) {
                                    case ButtonSave:
                                    case ButtonOK: {
                                            //
                                            cp.core.siteProperties.setProperty("Allow CSS Reset", cp.core.docProperties.getBoolean(RequestNameAllowCSSReset));
                                            cp.core.cdnFiles.saveFile(DynamicStylesFilename, cp.core.docProperties.getText("StyleEditor"));
                                            if (cp.core.docProperties.getBoolean(RequestNameInlineStyles)) {
                                                //
                                                // Inline Styles
                                                //
                                                cp.core.siteProperties.setProperty("StylesheetSerialNumber", "0");
                                            } else {
                                                // mark to rebuild next fetch
                                                cp.core.siteProperties.setProperty("StylesheetSerialNumber", "-1");
                                            }
                                            //
                                            // delete all templateid based editorstylerule files, build on-demand
                                            //
                                            EditorStyleRulesFilename = GenericController.strReplace(EditorStyleRulesFilenamePattern, "$templateid$", "0", 1, 99, 1);
                                            cp.core.cdnFiles.deleteFile(EditorStyleRulesFilename);
                                            //
                                            using (var csData = new CsModel(cp.core)) {
                                                csData.openSql("select id from cctemplates");
                                                while (csData.ok()) {
                                                    EditorStyleRulesFilename = GenericController.strReplace(EditorStyleRulesFilenamePattern, "$templateid$", csData.getText("ID"), 1, 99, 1);
                                                    cp.core.cdnFiles.deleteFile(EditorStyleRulesFilename);
                                                    csData.goNext();
                                                }
                                                csData.close();
                                            }
                                            break;
                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                switch (adminData.requestButton) {
                                    case ButtonCancel:
                                    case ButtonOK: {
                                            //
                                            // Process redirects
                                            //
                                            adminData.adminForm = AdminFormRoot;
                                            break;
                                        }
                                    default: {
                                            // do nothing
                                            break;
                                        }
                                }
                                break;
                            }
                        default: {
                                // end case
                                break;
                            }
                    }
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
        }
        //
        //       
        //=============================================================================================
        //   Get
        //=============================================================================================
        //
        private int GetForm_Close(CPClass cp, int MenuDepth, string ContentName, int RecordID) {
            int tempGetForm_Close = 0;
            try {
                //
                if (MenuDepth > 0) {
                    tempGetForm_Close = AdminFormClose;
                } else {
                    tempGetForm_Close = AdminFormIndex;
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
            return tempGetForm_Close;
        }
        //
        //=============================================================================================
        ////
        //========================================================================
        //
        //=================================================================================
        //
        //=================================================================================
        //
        public static void setIndexSQL_SaveIndexConfig(CPClass cp, CoreController core, IndexConfigClass IndexConfig) {
            //
            // --Find words
            string SubList = "";
            foreach (var kvp in IndexConfig.findWords) {
                IndexConfigFindWordClass findWord = kvp.Value;
                if ((!string.IsNullOrEmpty(findWord.Name)) && (findWord.MatchOption != FindWordMatchEnum.MatchIgnore)) {
                    SubList = SubList + Environment.NewLine + findWord.Name + "\t" + findWord.Value + "\t" + (int)findWord.MatchOption;
                }
            }
            string FilterText = "";
            if (!string.IsNullOrEmpty(SubList)) {
                FilterText += Environment.NewLine + "FindWordList" + SubList + Environment.NewLine;
            }
            //
            // --CDef List
            if (IndexConfig.subCDefID > 0) {
                FilterText += Environment.NewLine + "CDefList\r\n" + IndexConfig.subCDefID + Environment.NewLine;
            }
            //
            // -- Group List
            SubList = "";
            if (IndexConfig.groupListCnt > 0) {
                //
                for (int ptr = 0; ptr < IndexConfig.groupListCnt; ptr++) {
                    if (!string.IsNullOrEmpty(IndexConfig.groupList[ptr])) {
                        SubList = SubList + Environment.NewLine + IndexConfig.groupList[ptr];
                    }
                }
            }
            if (!string.IsNullOrEmpty(SubList)) {
                FilterText += Environment.NewLine + "GroupList" + SubList + Environment.NewLine;
            }
            //
            // PageNumber and Records Per Page
            FilterText += Environment.NewLine
                + Environment.NewLine + "pagenumber"
                + Environment.NewLine + IndexConfig.pageNumber;
            FilterText += Environment.NewLine
                + Environment.NewLine + "recordsperpage"
                + Environment.NewLine + IndexConfig.recordsPerPage;
            //
            // misc filters
            if (IndexConfig.activeOnly) {
                FilterText += Environment.NewLine
                    + Environment.NewLine + "IndexFilterActiveOnly";
            }
            if (IndexConfig.lastEditedByMe) {
                FilterText += Environment.NewLine
                    + Environment.NewLine + "IndexFilterLastEditedByMe";
            }
            if (IndexConfig.lastEditedToday) {
                FilterText += Environment.NewLine
                    + Environment.NewLine + "IndexFilterLastEditedToday";
            }
            if (IndexConfig.lastEditedPast7Days) {
                FilterText += Environment.NewLine
                    + Environment.NewLine + "IndexFilterLastEditedPast7Days";
            }
            if (IndexConfig.lastEditedPast30Days) {
                FilterText += Environment.NewLine
                    + Environment.NewLine + "IndexFilterLastEditedPast30Days";
            }
            if (IndexConfig.open) {
                FilterText += Environment.NewLine
                    + Environment.NewLine + "IndexFilterOpen";
            }
            //
            cp.core.visitProperty.setProperty(AdminDataModel.IndexConfigPrefix + encodeText(IndexConfig.contentID), FilterText);
            //
            //   Member Properties (persistant)
            //
            // Save Admin Column
            SubList = "";
            foreach (var column in IndexConfig.columns) {
                if (!string.IsNullOrEmpty(column.Name)) {
                    SubList = SubList + Environment.NewLine + column.Name + "\t" + column.Width;
                }
            }
            FilterText = "";
            if (!string.IsNullOrEmpty(SubList)) {
                FilterText += Environment.NewLine + "Columns" + SubList + Environment.NewLine;
            }
            //
            // Sorts
            //
            SubList = "";
            foreach (var kvp in IndexConfig.sorts) {
                IndexConfigSortClass sort = kvp.Value;
                if (!string.IsNullOrEmpty(sort.fieldName)) {
                    SubList = SubList + Environment.NewLine + sort.fieldName + "\t" + sort.direction;
                }
            }
            if (!string.IsNullOrEmpty(SubList)) {
                FilterText += Environment.NewLine + "Sorts" + SubList + Environment.NewLine;
            }
            cp.core.userProperty.setProperty(AdminDataModel.IndexConfigPrefix + encodeText(IndexConfig.contentID), FilterText);
            //

        }
    }
}
