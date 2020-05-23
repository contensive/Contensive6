﻿
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Contensive.BaseClasses;
using Contensive.Processor.Exceptions;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
using System.Text;
using Contensive.Processor.Models.Domain;
using Contensive.Models.Db;
using System.Threading.Tasks;
//
namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// build page content system. Persistence is the docController.
    /// </summary>
    public class PageContentController : IDisposable {
        /// <summary>
        /// Constructor
        /// </summary>
        public PageContentController() {
            page = new PageContentModel();
            pageToRootList = new List<PageContentModel>();
            template = new PageTemplateModel();
            childPageIdsListed = new List<int>();
        }
        /// <summary>
        /// current page to it's root. List.First is current page, List.Last is rootpage
        /// </summary>
        public List<PageContentModel> pageToRootList { get; set; }
        /// <summary>
        /// current template
        /// </summary>
        public PageTemplateModel template { get; set; }
        /// <summary>
        /// the reason the current template was selected, displayed only as comment for debug mode
        /// </summary>
        public string templateReason { get; set; } = "";
        /// <summary>
        /// current page for website documents, blank for others
        /// </summary>
        public PageContentModel page { get; set; }
        /// <summary>
        /// Child pages can be listed on Child Page List addons.
        /// If a page has has this page set 
        /// </summary>
        public List<int> childPageIdsListed { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// Process the content of the body tag. Adds required head tags
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        public static string getHtmlBody(CoreController core) {
            string result = "";
            try {
                bool IsPageNotFound = false;
                //
                if (core.doc.continueProcessing) {
                    //
                    // -- setup domain
                    string domainTest = core.webServer.requestDomain.Trim().ToLowerInvariant().Replace("..", ".");
                    core.doc.domain = null;
                    if (!string.IsNullOrEmpty(domainTest)) {
                        int posDot = 0;
                        int loopCnt = 10;
                        do {
                            core.doc.domain = DbBaseModel.createByUniqueName<DomainModel>(core.cpParent, domainTest);
                            posDot = domainTest.IndexOf('.');
                            if ((posDot >= 0) && (domainTest.Length > 1)) {
                                domainTest = domainTest.Substring(posDot + 1);
                            }
                            loopCnt -= 1;
                        } while ((core.doc.domain == null) && (posDot >= 0) && (loopCnt > 0));
                    }
                    //
                    // -- load requested page/template
                    loadPage(core, core.docProperties.getInteger(rnPageId), core.doc.domain);
                    //
                    // -- create context object to use for page and template dependencies
                    CPUtilsBaseClass.addonExecuteContext executeContext = new CPUtilsBaseClass.addonExecuteContext {
                        addonType = CPUtilsBaseClass.addonContext.ContextSimple,
                        cssContainerClass = "",
                        cssContainerId = "",
                        hostRecord = new CPUtilsBaseClass.addonExecuteHostRecordContext {
                            contentName = PageContentModel.tableMetadata.contentName,
                            fieldName = "copyfilename",
                            recordId = core.doc.pageController.page.id
                        },
                        isIncludeAddon = false
                    };
                    //
                    // -- execute template Dependencies
                    List<AddonModel> templateAddonList = AddonModel.createList_templateDependencies(core.cpParent, core.doc.pageController.template.id);
                    if (templateAddonList.Count > 0) {
                        string addonContextMessage = executeContext.errorContextMessage;
                        foreach (AddonModel addon in templateAddonList) {
                            executeContext.errorContextMessage = "executing template dependency [" + addon.name + "]";
                            result += core.addon.executeDependency(addon, executeContext);
                        }
                        executeContext.errorContextMessage = addonContextMessage;
                    }
                    //
                    // -- execute on page start addons (after body tag)//
                    // 
                    // -- execute page addons
                    //
                    // -- execute page Dependencies
                    List<AddonModel> pageAddonList = AddonModel.createList_pageDependencies(core.cpParent, core.doc.pageController.page.id);
                    if (pageAddonList.Count > 0) {
                        string addonContextMessage = executeContext.errorContextMessage;
                        foreach (AddonModel addon in pageAddonList) {
                            executeContext.errorContextMessage = "executing page dependency [" + addon.name + "]";
                            result += core.addon.executeDependency(addon, executeContext);
                        }
                        executeContext.errorContextMessage = addonContextMessage;
                    }
                    //
                    core.doc.adminWarning = core.docProperties.getText("AdminWarningMsg");
                    core.doc.adminWarningPageID = core.docProperties.getInteger("AdminWarningPageID");
                    //
                    // todo move cookie test to htmlDoc controller
                    // -- Add cookie test
                    bool AllowCookieTest = core.siteProperties.allowVisitTracking && (core.session.visit.pageVisits == 1);
                    if (AllowCookieTest) {
                        core.html.addScriptCode_onLoad("if (document.cookie && document.cookie != null){cj.ajax.qs('" + rnCookieDetect + "=" + SecurityController.encodeToken(core, core.session.visit.id, core.doc.profileStartTime.AddSeconds(30)) + "')};", "Cookie Test");
                    }
                    //
                    // -- Contensive Form Page Processing
                    if (core.docProperties.getInteger("ContensiveFormPageID") != 0) {
                        processForm(core, core.docProperties.getInteger("ContensiveFormPageID"));
                    }
                    //
                    // -- Automatic Redirect to a full URL, If the link field of the record is an absolution address, rc = redirect contentID, ri = redirect content recordid
                    core.doc.redirectContentID = (core.docProperties.getInteger(rnRedirectContentId));
                    if (core.doc.redirectContentID != 0) {
                        core.doc.redirectRecordID = (core.docProperties.getInteger(rnRedirectRecordId));
                        if (core.doc.redirectRecordID != 0) {
                            string contentName = MetadataController.getContentNameByID(core, core.doc.redirectContentID);
                            if (!string.IsNullOrEmpty(contentName)) {
                                if (core.webServer.redirectByRecord_ReturnStatus(contentName, core.doc.redirectRecordID, "")) {
                                    core.doc.continueProcessing = false;
                                    return string.Empty;
                                } else {
                                    core.doc.adminWarning = "<p>The site attempted to automatically jump to another page, but there was a problem with the page that included the link.<p>";
                                    core.doc.adminWarningPageID = core.doc.redirectRecordID;
                                }
                            }
                        }
                    }
                    //
                    // -- Active Download hook
                    LibraryFilesModel file = LibraryFilesModel.create<LibraryFilesModel>(core.cpParent, core.docProperties.getText(RequestNameDownloadFileGuid));
                    if (file == null) {
                        //
                        // -- compatibility mode, downloadid, this exposes all library files because it exposes the sequential id number
                        int downloadId = core.docProperties.getInteger(RequestNameDownloadFileId);
                        if ((downloadId > 0) && (core.siteProperties.getBoolean("Allow library file download by id", false))) {
                            file = LibraryFilesModel.create<LibraryFilesModel>(core.cpParent, downloadId);
                        }
                    }
                    if (file != null) {
                        //
                        // -- lookup record and set clicks
                        if (file != null) {
                            file.clicks += 1;
                            file.save(core.cpParent);
                            if (file.filename != "") {
                                //
                                // -- create log entry
                                LibraryFileLogModel log = LibraryFileLogModel.addEmpty<LibraryFileLogModel>(core.cpParent);
                                if (log != null) {
                                    log.name = core.dateTimeNowMockable.ToString() + " user [#" + core.session.user.name + ", " + core.session.user.name + "]";
                                    log.fileId = file.id;
                                    log.visitId = core.session.visit.id;
                                    log.memberId = core.session.user.id;
                                    log.fromUrl = core.webServer.requestPageReferer;
                                    log.save(core.cpParent);
                                }
                                //
                                // -- and go
                                string link = GenericController.getCdnFileLink(core, file.filename);
                                return core.webServer.redirect(link, "Redirecting because the active download request variable is set to a valid Library Files record.");
                            }
                        }
                    }
                    //
                    // -- Process clipboard cut/paste
                    string Clip = core.docProperties.getText(RequestNameCut);
                    if (!string.IsNullOrEmpty(Clip)) {
                        //
                        // -- clicked cut, save the cut in the clipboard
                        core.visitProperty.setProperty("Clipboard", Clip);
                        GenericController.modifyLinkQuery(core.doc.refreshQueryString, RequestNameCut, "");
                    }
                    int ClipParentContentId = core.docProperties.getInteger(RequestNamePasteParentContentId);
                    int ClipParentRecordId = core.docProperties.getInteger(RequestNamePasteParentRecordId);
                    if ((ClipParentContentId != 0) && (ClipParentRecordId != 0)) {
                        //
                        // -- clicked paste, do the paste and clear the cliboard
                        attemptClipboardPaste(core, ClipParentContentId, ClipParentRecordId);
                    }
                    //
                    Clip = core.docProperties.getText(RequestNameCutClear);
                    if (!string.IsNullOrEmpty(Clip)) {
                        //
                        // if a cut clear, clear the clipboard
                        core.visitProperty.setProperty("Clipboard", "");
                        Clip = core.visitProperty.getText("Clipboard", "");
                        GenericController.modifyLinkQuery(core.doc.refreshQueryString, RequestNameCutClear, "");
                    }
                    //
                    //--------------------------------------------------------------------------
                    // link alias and link forward
                    //--------------------------------------------------------------------------
                    //
                    string linkAliasTest1 = core.webServer.requestPathPage;
                    if (linkAliasTest1.left(1) == "/") {
                        linkAliasTest1 = linkAliasTest1.Substring(1);
                    }
                    if (linkAliasTest1.Length > 0) {
                        if (linkAliasTest1.Substring(linkAliasTest1.Length - 1, 1) == "/") {
                            linkAliasTest1 = linkAliasTest1.left(linkAliasTest1.Length - 1);
                        }
                    }
                    string linkAliasTest2 = linkAliasTest1 + "/";
                    string sql = "";
                    if ((!IsPageNotFound) && (core.webServer.requestPathPage != "")) {
                        //
                        // build link variations needed later
                        int PosProtocol = GenericController.strInstr(1, core.webServer.requestPathPage, "://", 1);
                        string LinkNoProtocol = "";
                        string LinkFullPath = "";
                        string LinkFullPathNoSlash = "";
                        if (PosProtocol != 0) {
                            LinkNoProtocol = core.webServer.requestPathPage.Substring(PosProtocol + 2);
                            int Pos = GenericController.strInstr(PosProtocol + 3, core.webServer.requestPathPage, "/", 2);
                            if (Pos != 0) {
                                string linkDomain = core.webServer.requestPathPage.left(Pos - 1);
                                LinkFullPath = core.webServer.requestPathPage.Substring(Pos - 1);
                                //
                                // strip off leading or trailing slashes, and return only the string between the leading and secton slash
                                //
                                if (GenericController.strInstr(1, LinkFullPath, "/") != 0) {
                                    string[] LinkSplit = LinkFullPath.Split('/');
                                    LinkFullPathNoSlash = LinkSplit[0];
                                    if (string.IsNullOrEmpty(LinkFullPathNoSlash)) {
                                        if (LinkSplit.GetUpperBound(0) > 0) {
                                            LinkFullPathNoSlash = LinkSplit[1];
                                        }
                                    }
                                }
                                linkAliasTest1 = LinkFullPath;
                                linkAliasTest2 = LinkFullPathNoSlash;
                            }
                        }
                        //
                        //   if this has not already been recognized as a pagenot found, and the custom404source is present, try all these
                        //   Build LinkForwardCritia and LinkAliasCriteria
                        //   sample: http://www.a.com/kb/test
                        //   LinkForwardCriteria = (Sourcelink='http://www.a.com/kb/test')or(Sourcelink='http://www.a.com/kb/test/')
                        //
                        bool IsInLinkForwardTable = false;
                        bool isLinkForward = false;
                        string LinkForwardCriteria = ""
                            + "(active<>0)"
                            + "and("
                            + "(SourceLink=" + DbController.encodeSQLText(core.webServer.requestPathPage) + ")"
                            + "or(SourceLink=" + DbController.encodeSQLText(LinkNoProtocol) + ")"
                            + "or(SourceLink=" + DbController.encodeSQLText(LinkFullPath) + ")"
                            + "or(SourceLink=" + DbController.encodeSQLText(LinkFullPathNoSlash) + ")"
                            + ")";
                        sql = core.db.getSQLSelect("ccLinkForwards", "ID,DestinationLink,Viewings,GroupID", LinkForwardCriteria, "ID", "", 1);
                        using (var csData = new CsModel(core)) {
                            csData.openSql(sql);
                            if (csData.ok()) {
                                //
                                // Link Forward found - update count
                                //
                                string tmpLink = null;
                                int GroupId = 0;
                                string groupName = null;
                                //
                                IsInLinkForwardTable = true;
                                int Viewings = csData.getInteger("Viewings") + 1;
                                sql = "update ccLinkForwards set Viewings=" + Viewings + " where ID=" + csData.getInteger("ID");
                                Task.Run(() => core.db.executeNonQueryAsync(sql));
                                tmpLink = csData.getText("DestinationLink");
                                if (!string.IsNullOrEmpty(tmpLink)) {
                                    //
                                    // Valid Link Forward (without link it is just a record created by the autocreate function
                                    //
                                    isLinkForward = true;
                                    tmpLink = csData.getText("DestinationLink");
                                    GroupId = csData.getInteger("GroupID");
                                    if (GroupId != 0) {
                                        groupName = GroupController.getGroupName(core, GroupId);
                                        if (!string.IsNullOrEmpty(groupName)) {
                                            GroupController.addUser(core, groupName);
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(tmpLink)) {
                                        core.doc.redirectLink = tmpLink;
                                    }
                                }
                            }
                        }
                        //
                        if ((string.IsNullOrEmpty(core.doc.redirectLink)) && !isLinkForward) {
                            //
                            // Test for Link Alias
                            //
                            if (!string.IsNullOrEmpty(linkAliasTest1 + linkAliasTest2)) {
                                string sqlLinkAliasCriteria = "(name=" + DbController.encodeSQLText(linkAliasTest1) + ")or(name=" + DbController.encodeSQLText(linkAliasTest2) + ")";
                                List<LinkAliasModel> linkAliasList = DbBaseModel.createList<LinkAliasModel>(core.cpParent, sqlLinkAliasCriteria, "id desc");
                                if (linkAliasList.Count > 0) {
                                    LinkAliasModel linkAlias = linkAliasList.First();
                                    string LinkQueryString = rnPageId + "=" + linkAlias.pageId + "&" + linkAlias.queryStringSuffix;
                                    core.docProperties.setProperty(rnPageId, linkAlias.pageId, DocPropertyModel.DocPropertyTypesEnum.userDefined);
                                    string[] nameValuePairs = linkAlias.queryStringSuffix.Split('&');
                                    foreach (string nameValuePair in nameValuePairs) {
                                        string[] nameValueThing = nameValuePair.Split('=');
                                        if (nameValueThing.GetUpperBound(0) == 0) {
                                            core.docProperties.setProperty(nameValueThing[0], "", DocPropertyModel.DocPropertyTypesEnum.userDefined);
                                        } else {
                                            core.docProperties.setProperty(nameValueThing[0], nameValueThing[1], DocPropertyModel.DocPropertyTypesEnum.userDefined);
                                        }
                                    }
                                }
                            }
                            //
                            // No Link Forward, no Link Alias, no RemoteMethodFromPage, not Robots.txt
                            //
                            if ((core.doc.errorList.Count == 0) && core.siteProperties.getBoolean("LinkForwardAutoInsert") && (!IsInLinkForwardTable)) {
                                //
                                // Add a new Link Forward entry
                                //
                                using (var csData = new CsModel(core)) {
                                    csData.insert("Link Forwards");
                                    if (csData.ok()) {
                                        csData.set("Name", core.webServer.requestPathPage);
                                        csData.set("sourcelink", core.webServer.requestPathPage);
                                        csData.set("Viewings", 1);
                                    }
                                }
                            }
                        }
                    }
                    //
                    // ----- do anonymous access blocking
                    if (!core.session.isAuthenticated) {
                        if ((core.webServer.requestPath != "/") && GenericController.strInstr(1, "/" + core.appConfig.adminRoute, core.webServer.requestPath, 1) != 0) {
                            //
                            // admin page is excluded from custom blocking
                        } else {
                            int AnonymousUserResponseId = GenericController.encodeInteger(core.siteProperties.getText("AnonymousUserResponseID", "0"));
                            switch (AnonymousUserResponseId) {
                                case 1: {
                                        //
                                        // -- block with login
                                        core.doc.continueProcessing = false;
                                        return core.addon.execute(DbBaseModel.create<AddonModel>(core.cpParent, addonGuidLoginPage), new CPUtilsBaseClass.addonExecuteContext {
                                            addonType = CPUtilsBaseClass.addonContext.ContextPage,
                                            errorContextMessage = "calling login page addon [" + addonGuidLoginPage + "] because page is blocked"
                                        });
                                    }
                                case 2: {
                                        //
                                        // -- block with custom content
                                        core.doc.continueProcessing = false;
                                        core.doc.setMetaContent(0, 0);
                                        core.html.addScriptCode_onLoad("document.body.style.overflow='scroll'", "Anonymous User Block");
                                        return core.html.getHtmlDoc('\r' + core.html.getContentCopy("AnonymousUserResponseCopy", "<p style=\"width:250px;margin:100px auto auto auto;\">The site is currently not available for anonymous access.</p>", core.session.user.id, true, core.session.isAuthenticated), TemplateDefaultBodyTag, true, true);
                                    }
                                default: {
                                        // do nothing
                                        break;
                                    }
                            }
                        }
                    }
                    //
                    // -- build body content
                    string htmlDocBody = getHtmlBody_BodyTag(core);
                    //
                    // -- check secure certificate required
                    bool SecureLink_Template_Required = core.doc.pageController.template.isSecure;
                    bool SecureLink_Page_Required = false;
                    foreach (PageContentModel page in core.doc.pageController.pageToRootList) {
                        if (core.doc.pageController.page.isSecure) {
                            SecureLink_Page_Required = true;
                            break;
                        }
                    }
                    bool SecureLink_Required = SecureLink_Template_Required || SecureLink_Page_Required;
                    bool SecureLink_CurrentURL = (core.webServer.requestUrl.ToLowerInvariant().left(8) == "https://");
                    if (SecureLink_CurrentURL && (!SecureLink_Required)) {
                        //
                        // -- redirect to non-secure
                        core.doc.redirectLink = GenericController.strReplace(core.webServer.requestUrl, "https://", "http://");
                        core.doc.redirectReason = "Redirecting because neither the page or the template requires a secure link.";
                        core.doc.redirectBecausePageNotFound = false;
                        return core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                    } else if ((!SecureLink_CurrentURL) && SecureLink_Required) {
                        //
                        // -- redirect to secure
                        core.doc.redirectLink = GenericController.strReplace(core.webServer.requestUrl, "http://", "https://");
                        if (SecureLink_Page_Required) {
                            core.doc.redirectReason = "Redirecting because this page [" + core.doc.pageController.pageToRootList[0].name + "] requires a secure link.";
                        } else {
                            core.doc.redirectReason = "Redirecting because this template [" + core.doc.pageController.template.name + "] requires a secure link.";
                        }
                        core.doc.redirectBecausePageNotFound = false;
                        return core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                    }
                    //
                    // -- check that this template exists on this domain
                    // -- if endpoint is just domain -> the template is automatically compatible by default (domain determined the landing page)
                    // -- if endpoint is domain + route (link alias), the route determines the page, which may determine the core.doc.pageController.template. If this template is not allowed for this domain, redirect to the domain's landingcore.doc.pageController.page.
                    //
                    sql = "(domainId=" + core.doc.domain.id + ")";
                    List<TemplateDomainRuleModel> allowTemplateRuleList = DbBaseModel.createList<TemplateDomainRuleModel>(core.cpParent, sql);
                    if (allowTemplateRuleList.Count == 0) {
                        //
                        // -- current template has no domain preference, use current
                    } else {
                        bool allowTemplate = false;
                        foreach (TemplateDomainRuleModel rule in allowTemplateRuleList) {
                            if (rule.templateId == core.doc.pageController.template.id) {
                                allowTemplate = true;
                                break;
                            }
                        }
                        if (!allowTemplate) {
                            //
                            // -- must redirect to a domain's landing page
                            core.doc.redirectLink = core.webServer.requestProtocol + core.doc.domain.name;
                            core.doc.redirectBecausePageNotFound = false;
                            core.doc.redirectReason = "Redirecting because this domain has template requiements set, and this template is not configured [" + core.doc.pageController.template.name + "].";
                            core.doc.redirectBecausePageNotFound = false;
                            return core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                        }
                    }
                    result += htmlDocBody;
                }
                //
                if (IsPageNotFound) {
                    //
                    // new way -- if a (real) 404 page is received, just convert this hit to the page-not-found page, do not redirect to it
                    //
                    LogController.addSiteWarning(core, "Page Not Found", "Page Not Found", "", 0, "Page Not Found from [" + core.webServer.requestUrlSource + "]", "Page Not Found", "Page Not Found");
                    core.webServer.setResponseStatus(WebServerController.httpResponseStatus404_NotFound);
                    core.docProperties.setProperty(rnPageId, getPageNotFoundPageId(core));
                    if (core.session.isAuthenticatedAdmin()) {
                        string PageNotFoundReason = "";
                        core.doc.adminWarning = PageNotFoundReason;
                        core.doc.adminWarningPageID = 0;
                    }
                }
                //
                // add exception list header
                if (core.session.user.developer) {
                    result = ErrorController.getDocExceptionHtmlList(core) + result;
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public static string getHtmlBody_BodyTag(CoreController core) {
            string result = "";
            try {
                if (core.doc.domain == null) {
                    //
                    // -- If no domain, block content with message
                    LogController.logError(core, "Domain not recognized:" + core.webServer.requestUrlSource);
                    return HtmlController.div("This domain name is not configured for this site.", "ccDialogPageNotFound");
                }
                //
                // -- get contentBoxHtml (page related content that should only be visible if the content is visible)
                bool blockSiteWithLogin = false;
                string contentBoxHtml = getHtmlBody_BodyTag_ContentBox(core, ref blockSiteWithLogin);
                if (!core.doc.continueProcessing) { return string.Empty; }
                if (blockSiteWithLogin) { return "<div class=\"ccLoginPageCon\">" + contentBoxHtml + "\r</div>"; }
                //
                // -- get template
                {
                    string templateHtml = core.doc.pageController.template.bodyHTML;
                    if (string.IsNullOrEmpty(templateHtml)) { templateHtml = Properties.Resources.DefaultTemplateHtml; }
                    string templateName = core.doc.pageController.template.name;
                    if (string.IsNullOrEmpty(templateName)) { templateName = "Template " + core.doc.pageController.template.id; }
                    //
                    // -- Encode Template
                    result += ActiveContentController.renderHtmlForWeb(core, templateHtml, "Page Templates", core.doc.pageController.template.id, 0, core.webServer.requestProtocol + core.webServer.requestDomain, core.siteProperties.defaultWrapperID, CPUtilsBaseClass.addonContext.ContextTemplate);
                    //
                    // -- add content into template
                    if (result.IndexOf(fpoContentBox) != -1) {
                        //
                        // -- replace page content into templatecontent
                        result = strReplace(result, fpoContentBox, contentBoxHtml);
                    } else {
                        //
                        // If Content was not found, add it to the end
                        result += contentBoxHtml;
                    }
                    //
                    // -- add template edit link
                    if (core.session.isTemplateEditing()) {
                        result = AdminUIController.getRecordEditAndCutAnchorTag(core, "Page Templates", core.doc.pageController.template.id, false, templateName) + result;
                        result = AdminUIController.getEditWrapper(core, result);
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public static string getHtmlBody_BodyTag_ContentBox(CoreController core, ref bool return_blockSiteWithLogin) {
            string result = "";
            try {
                //
                // -- validate page
                if (core.doc.pageController.page.id == 0) {
                    //
                    // -- landing page is not valid -- display error
                    LogController.logInfo(core, "Requested page/document not found:" + core.webServer.requestUrlSource);
                    core.doc.redirectBecausePageNotFound = true;
                    core.doc.redirectReason = "Redirecting because the page selected could not be found.";
                    core.doc.redirectLink = PageContentController.getPageNotFoundRedirectLink(core, core.doc.redirectReason, "", "", 0);
                    return core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                }
                //
                // -- If Link field populated, do redirect
                if (core.doc.pageController.page.link != "") {
                    core.doc.pageController.page.clicks += 1;
                    core.doc.pageController.page.save(core.cpParent);
                    core.doc.redirectLink = core.doc.pageController.page.link;
                    core.doc.redirectReason = "Redirect required because this page [# " + core.doc.pageController.page.id + "] has a menu link override [" + core.doc.pageController.page.link + "].";
                    core.doc.redirectBecausePageNotFound = false;
                    return core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                }
                //
                // -- build list of blocked pages
                // todo convert to a list, then join when needed (for performance when many blocked pages)
                string BlockedRecordIDList = "";
                if (string.IsNullOrWhiteSpace(core.doc.redirectLink)) {
                    foreach (PageContentModel testPage in core.doc.pageController.pageToRootList) {
                        if (testPage.blockContent || testPage.blockPage) {
                            BlockedRecordIDList = BlockedRecordIDList + "," + testPage.id;
                        }
                    }
                    if (!string.IsNullOrEmpty(BlockedRecordIDList)) {
                        BlockedRecordIDList = BlockedRecordIDList.Substring(1);
                    }
                }
                //
                // -- Determine if Content Blocking
                bool ContentBlocked = false;
                if (!string.IsNullOrEmpty(BlockedRecordIDList)) {
                    if (core.session.isAuthenticatedAdmin()) {
                        //
                        // Administrators are never blocked
                        //
                    } else if (!core.session.isAuthenticated) {
                        //
                        // non-authenticated are always blocked
                        //
                        ContentBlocked = true;
                    } else {
                        //
                        // Check Access Groups, if in access groups, remove group from BlockedRecordIDList
                        //
                        string SQL = "SELECT DISTINCT ccPageContentBlockRules.RecordID"
                            + " FROM (ccPageContentBlockRules"
                            + " LEFT JOIN ccgroups ON ccPageContentBlockRules.GroupId = ccgroups.ID)"
                            + " LEFT JOIN ccMemberRules ON ccgroups.Id = ccMemberRules.GroupID"
                            + " WHERE (((ccMemberRules.memberId)=" + DbController.encodeSQLNumber(core.session.user.id) + ")"
                            + " AND ((ccPageContentBlockRules.RecordID) In (" + BlockedRecordIDList + "))"
                            + " AND ((ccPageContentBlockRules.Active)<>0)"
                            + " AND ((ccgroups.Active)<>0)"
                            + " AND ((ccMemberRules.Active)<>0)"
                            + " AND ((ccMemberRules.DateExpires) Is Null Or (ccMemberRules.DateExpires)>" + DbController.encodeSQLDate(core.doc.profileStartTime) + "));";
                        using (var csData = new CsModel(core)) {
                            csData.openSql(SQL);
                            BlockedRecordIDList = "," + BlockedRecordIDList;
                            while (csData.ok()) {
                                BlockedRecordIDList = GenericController.strReplace(BlockedRecordIDList, "," + csData.getText("RecordID"), "");
                                csData.goNext();
                            }
                            csData.close();
                        }
                        if (!string.IsNullOrEmpty(BlockedRecordIDList)) {
                            //
                            // ##### remove the leading comma
                            BlockedRecordIDList = BlockedRecordIDList.Substring(1);
                            // Check the remaining blocked records against the members Content Management
                            // ##### removed hardcoded mistakes from the sql
                            SQL = "SELECT DISTINCT ccPageContent.ID as RecordID"
                                + " FROM ((ccPageContent"
                                + " LEFT JOIN ccGroupRules ON ccPageContent.ContentControlId = ccGroupRules.ContentID)"
                                + " LEFT JOIN ccgroups AS ManagementGroups ON ccGroupRules.GroupId = ManagementGroups.ID)"
                                + " LEFT JOIN ccMemberRules AS ManagementMemberRules ON ManagementGroups.Id = ManagementMemberRules.GroupID"
                                + " WHERE (((ccPageContent.ID) In (" + BlockedRecordIDList + "))"
                                + " AND ((ccGroupRules.Active)<>0)"
                                + " AND ((ManagementGroups.Active)<>0)"
                                + " AND ((ManagementMemberRules.Active)<>0)"
                                + " AND ((ManagementMemberRules.DateExpires) Is Null Or (ManagementMemberRules.DateExpires)>" + DbController.encodeSQLDate(core.doc.profileStartTime) + ")"
                                + " AND ((ManagementMemberRules.memberId)=" + core.session.user.id + " ));";
                            using (var csData = new CsModel(core)) {
                                csData.openSql(SQL);
                                while (csData.ok()) {
                                    BlockedRecordIDList = GenericController.strReplace(BlockedRecordIDList, "," + csData.getText("RecordID"), "");
                                    csData.goNext();
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(BlockedRecordIDList)) {
                            ContentBlocked = true;
                        }
                    }
                }
                //
                // -- Implement Content Blocking
                if (ContentBlocked) {
                    string CustomBlockMessageFilename = "";
                    int BlockSourceId = main_BlockSourceDefaultMessage;
                    int ContentPadding = 20;
                    string[] BlockedPages = BlockedRecordIDList.Split(',');
                    int BlockedPageRecordId = GenericController.encodeInteger(BlockedPages[BlockedPages.GetUpperBound(0)]);
                    int RegistrationGroupId = 0;
                    if (BlockedPageRecordId != 0) {
                        using (var csData = new CsModel(core)) {
                            csData.openRecord("Page Content", BlockedPageRecordId, "CustomBlockMessage,BlockSourceID,RegistrationGroupID,ContentPadding");
                            if (csData.ok()) {
                                BlockSourceId = csData.getInteger("BlockSourceID");
                                CustomBlockMessageFilename = csData.getFilename("CustomBlockMessage");
                                RegistrationGroupId = csData.getInteger("RegistrationGroupID");
                                ContentPadding = csData.getInteger("ContentPadding");
                            }
                        }
                    }
                    //
                    // -- Block Appropriately
                    switch (BlockSourceId) {
                        case ContentBlockWithCustomMessage: {
                                //
                                // ----- Custom Message
                                //
                                result = string.IsNullOrWhiteSpace(CustomBlockMessageFilename) ? "" : core.cdnFiles.readFileText(CustomBlockMessageFilename);
                                break;
                            }
                        case ContentBlockWithLogin: {
                                //
                                // ----- Login page
                                //
                                string BlockForm = "";
                                if (!core.session.isAuthenticated) {
                                    if (!core.session.isRecognized()) {
                                        //
                                        // -- not recognized
                                        BlockForm = ""
                                            + "<p>This page has limited access. If you have an account, please login.</p>"
                                            + core.addon.execute(DbBaseModel.create<AddonModel>(core.cpParent, addonGuidLoginForm), new CPUtilsBaseClass.addonExecuteContext {
                                                addonType = CPUtilsBaseClass.addonContext.ContextPage,
                                                errorContextMessage = "calling login form addon [" + addonGuidLoginPage + "] because content box is blocked and user not recognized"
                                            });
                                    } else {
                                        //
                                        // -- recognized, not authenticated
                                        BlockForm = ""
                                            + "<p>This page has limited access. You were recognized as \"<b>" + core.session.user.name + "</b>\", but for security reasons you need to login to continue.</p>"
                                            + core.addon.execute(DbBaseModel.create<AddonModel>(core.cpParent, addonGuidLoginForm), new CPUtilsBaseClass.addonExecuteContext {
                                                addonType = CPUtilsBaseClass.addonContext.ContextPage,
                                                errorContextMessage = "calling login form addon [" + addonGuidLoginPage + "] because content box is blocked and user not authenticated"
                                            });
                                    }
                                } else {
                                    //
                                    // -- authenticated
                                    BlockForm = ""
                                        + "<p>You are currently logged in as \"<b>" + core.session.user.name + "</b>\". If this is not you, please <a href=\"?" + core.doc.refreshQueryString + "&method=logout\" rel=\"nofollow\">Click Here</a>.</p>"
                                        + "<p>This account does not have access to this page. For security reasons you need to login with a different account to continue.</p>"
                                        + core.addon.execute(DbBaseModel.create<AddonModel>(core.cpParent, addonGuidLoginForm), new CPUtilsBaseClass.addonExecuteContext {
                                            addonType = CPUtilsBaseClass.addonContext.ContextPage,
                                            errorContextMessage = "calling login form addon [" + addonGuidLoginPage + "] because content box is blocked and user does not have access to content"
                                        });
                                }
                                result = ""
                                    + "<div style=\"margin: 100px, auto, auto, auto;text-align:left;\">"
                                    + ErrorController.getUserError(core) + BlockForm + "</div>";
                                break;
                            }
                        case ContentBlockWithRegistration: {
                                //
                                // ----- Registration
                                string BlockForm = "";
                                if (core.docProperties.getInteger("subform") == ContentBlockWithLogin) {
                                    //
                                    // login subform form
                                    BlockForm = ""
                                        + "<p>This content has limited access. If you have an account, please login using this form.</p>"
                                        + "<p>If you do not have an account, <a href=\"?" + core.doc.refreshQueryString + "&subform=0\">click here to register</a>.</p>"
                                        + core.addon.execute(DbBaseModel.create<AddonModel>(core.cpParent, addonGuidLoginForm), new CPUtilsBaseClass.addonExecuteContext {
                                            addonType = CPUtilsBaseClass.addonContext.ContextPage,
                                            errorContextMessage = "calling login form addon [" + addonGuidLoginPage + "] because content box is blocked for registration"
                                        });
                                } else {
                                    //
                                    // Register Form
                                    //
                                    if (!core.session.isAuthenticated && core.session.isRecognized()) {
                                        //
                                        // -- Can not take the chance, if you go to a registration page, and you are recognized but not auth -- logout first
                                        core.session.logout();
                                    }
                                    if (!core.session.isAuthenticated) {
                                        //
                                        // -- Not Authenticated
                                        core.doc.verifyRegistrationFormPage(core);
                                        BlockForm = ""
                                            + "<p>This content has limited access. If you have an account, <a href=\"?" + core.doc.refreshQueryString + "&subform=" + ContentBlockWithLogin + "\">click Here to login</a>.</p>"
                                            + "<p>To view this content, please complete this form.</p>"
                                            + getFormPage(core, "Registration Form", RegistrationGroupId) + "";
                                    } else {
                                        //
                                        // -- Authenticated
                                        core.doc.verifyRegistrationFormPage(core);
                                        string BlockCopy = ""
                                            + "<p>You are currently logged in as \"<b>" + core.session.user.name + "</b>\". If this is not you, please <a href=\"?" + core.doc.refreshQueryString + "&method=logout\" rel=\"nofollow\">click Here</a>.</p>"
                                            + "<p>This account does not have access to this content. To view this content, please complete this form.</p>"
                                            + getFormPage(core, "Registration Form", RegistrationGroupId) + "";
                                    }
                                }
                                result = "<div style=\"margin: 100px, auto, auto, auto;text-align:left;\">" + ErrorController.getUserError(core) + BlockForm + "</div>";
                                break;
                            }
                        default: {
                                //
                                // ----- Content as blocked - convert from site property to content page
                                result = getContentBlockMessage(core, core.siteProperties.useContentWatchLink);
                                break;
                            }
                    }
                    //
                    // -- encode the copy
                    if (!core.siteProperties.beta200327_BlockCCmdPostPageRender) {
                        //
                        // -- {%%} execution should be limited to legacy pagecontent.copy only so this was moved down to _contentbox_renderelements
                        result = ActiveContentController.renderHtmlForWeb(core, result, PageContentModel.tableMetadata.contentName, core.doc.pageController.page.id, core.doc.pageController.page.contactMemberId, "http://" + core.webServer.requestDomain, core.siteProperties.defaultWrapperID, CPUtilsBaseClass.addonContext.ContextPage);
                    }
                    if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(core.doc.refreshQueryString)) {
                        result = result.Replace("?method=login", "?method=Login&" + core.doc.refreshQueryString);
                    }
                    //
                    // Add in content padding required for integration with the template
                    result = getContentBoxWrapper(core, result, ContentPadding);
                }
                //
                // -- Encoding, Tracking and Triggers
                if (!ContentBlocked) {
                    //
                    // -- OnPageStart Event
                    Dictionary<string, string> instanceArguments = new Dictionary<string, string> { { "CSPage", "-1" } };
                    foreach (var addon in core.addonCache.getOnPageStartAddonList()) {
                        CPUtilsBaseClass.addonExecuteContext pageStartContext = new CPUtilsBaseClass.addonExecuteContext {
                            instanceGuid = "-1",
                            argumentKeyValuePairs = instanceArguments,
                            addonType = CPUtilsBaseClass.addonContext.ContextOnPageStart,
                            errorContextMessage = "calling start page addon [" + addon.name + "]"
                        };
                        result += core.addon.execute(addon, pageStartContext);
                    }
                    //
                    // -- Render the content for the contentbox
                    result += getHtmlBody_BodyTag_ContentBox_RenderPageFeatures(core);
                    if (core.session.isQuickEditing("")) {
                        //
                        // Quick Editor, no encoding or tracking
                        //
                    } else {
                        //
                        // Live content
                        if (!core.siteProperties.beta200327_BlockCCmdPostPageRender) {
                            //
                            // -- {%%} execution should be limited to legacy pagecontent.copy only so this was moved down to _contentbox_renderelements
                            result = ActiveContentController.renderHtmlForWeb(core, result, PageContentModel.tableMetadata.contentName, core.doc.pageController.page.id, core.doc.pageController.page.contactMemberId, "http://" + core.webServer.requestDomain, core.siteProperties.defaultWrapperID, CPUtilsBaseClass.addonContext.ContextPage);
                        }
                        if (!core.session.isEditing(PageContentModel.tableMetadata.contentName)) {
                            //
                            // -- tracking
                            core.db.executeNonQuery("update ccpagecontent set viewings=" + (core.doc.pageController.page.viewings + 1) + " where id=" + core.doc.pageController.page.id);
                            //
                            // -- Page Hit Notification
                            if ((!core.session.visit.excludeFromAnalytics) && (core.doc.pageController.page.contactMemberId != 0) && (core.webServer.requestBrowser.IndexOf("kmahttp", System.StringComparison.OrdinalIgnoreCase) == -1)) {
                                PersonModel person = DbBaseModel.create<PersonModel>(core.cpParent, core.doc.pageController.page.contactMemberId);
                                if (person != null) {
                                    if (core.doc.pageController.page.allowHitNotification) {
                                        string PageName = core.doc.pageController.page.name;
                                        if (string.IsNullOrEmpty(PageName)) {
                                            PageName = core.doc.pageController.page.menuHeadline;
                                            if (string.IsNullOrEmpty(PageName)) {
                                                PageName = core.doc.pageController.page.headline;
                                                if (string.IsNullOrEmpty(PageName)) {
                                                    PageName = "[no name]";
                                                }
                                            }
                                        }
                                        var emailBody = new StringBuilder();
                                        emailBody.Append("<h1>Page Hit Notification.</h1>");
                                        emailBody.Append("<p>This email was sent to you as a notification of the following viewing details.</p>");
                                        emailBody.Append(HtmlController.tableStart(4, 1, 1));
                                        emailBody.Append("<tr><td align=\"right\" width=\"150\" Class=\"ccPanelHeader\">Description<br><img alt=\"image\" src=\"http://" + core.webServer.requestDomain + "" + cdnPrefix + "images/spacer.gif\" width=\"150\" height=\"1\"></td><td align=\"left\" width=\"100%\" Class=\"ccPanelHeader\">Value</td></tr>");
                                        emailBody.Append(get2ColumnTableRow("Domain", core.webServer.requestDomain, true));
                                        emailBody.Append(get2ColumnTableRow("Link", core.webServer.requestUrl, false));
                                        emailBody.Append(get2ColumnTableRow("Page Name", PageName, true));
                                        emailBody.Append(get2ColumnTableRow("Member Name", core.session.user.name, false));
                                        emailBody.Append(get2ColumnTableRow("Member #", encodeText(core.session.user.id), true));
                                        emailBody.Append(get2ColumnTableRow("Visit Start Time", encodeText(core.session.visit.startTime), false));
                                        emailBody.Append(get2ColumnTableRow("Visit #", encodeText(core.session.visit.id), true));
                                        emailBody.Append(get2ColumnTableRow("Visit IP", core.webServer.requestRemoteIP, false));
                                        emailBody.Append(get2ColumnTableRow("Browser ", core.webServer.requestBrowser, true));
                                        emailBody.Append(get2ColumnTableRow("Visitor #", encodeText(core.session.visitor.id), false));
                                        emailBody.Append(get2ColumnTableRow("Visit Authenticated", encodeText(core.session.visit.visitAuthenticated), true));
                                        emailBody.Append(get2ColumnTableRow("Visit Referrer", core.session.visit.http_referer, false));
                                        emailBody.Append(kmaEndTable);
                                        string queryStringForLinkAppend = "";
                                        string emailStatus = "";
                                        EmailController.queuePersonEmail(core, person, core.siteProperties.getText("EmailFromAddress", "info@" + core.webServer.requestDomain), "Page Hit Notification", emailBody.ToString(), "", "", false, true, 0, "", false, ref emailStatus, queryStringForLinkAppend, "Page Hit Notification, page [" + core.doc.pageController.page.id + "]");
                                    }
                                }
                            }
                            //        
                            // -- Process Trigger Conditions
                            if (!core.doc.pageController.page.triggerConditionId.Equals(0)) {
                                string pageTriggerName = "page" + core.doc.pageController.page.id + "trigger";
                                if (!core.visitProperty.getBoolean(pageTriggerName, false)) {
                                    core.visitProperty.setProperty(pageTriggerName, true);
                                    switch (core.doc.pageController.page.triggerConditionId) {
                                        case 1: {
                                                //
                                                // Always
                                                //
                                                if (core.doc.pageController.page.triggerSendSystemEmailId != 0) {
                                                    EmailController.queueSystemEmail(core, MetadataController.getRecordName(core, "System Email", core.doc.pageController.page.triggerSendSystemEmailId), "", core.session.user.id);
                                                }
                                                if (core.doc.pageController.page.triggerAddGroupId != 0) {
                                                    GroupController.addUser(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerAddGroupId));
                                                }
                                                if (core.doc.pageController.page.triggerRemoveGroupId != 0) {
                                                    GroupController.removeUser(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerRemoveGroupId));
                                                }
                                                break;
                                            }
                                        case 2: {
                                                //
                                                // If in Condition Group
                                                //
                                                if (core.doc.pageController.page.triggerConditionGroupId != 0) {
                                                    if (GroupController.isMemberOfGroup(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerConditionGroupId))) {
                                                        if (core.doc.pageController.page.triggerSendSystemEmailId != 0) {
                                                            EmailController.queueSystemEmail(core, MetadataController.getRecordName(core, "System Email", core.doc.pageController.page.triggerSendSystemEmailId), "", core.session.user.id);
                                                        }
                                                        if (core.doc.pageController.page.triggerAddGroupId != 0) {
                                                            GroupController.addUser(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerAddGroupId));
                                                        }
                                                        if (core.doc.pageController.page.triggerRemoveGroupId != 0) {
                                                            GroupController.removeUser(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerRemoveGroupId));
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 3: {
                                                //
                                                // If not in Condition Group
                                                //
                                                if (core.doc.pageController.page.triggerConditionGroupId != 0) {
                                                    if (!GroupController.isMemberOfGroup(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerConditionGroupId))) {
                                                        if (core.doc.pageController.page.triggerAddGroupId != 0) {
                                                            GroupController.addUser(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerAddGroupId));
                                                        }
                                                        if (core.doc.pageController.page.triggerRemoveGroupId != 0) {
                                                            GroupController.removeUser(core, GroupController.getGroupName(core, core.doc.pageController.page.triggerRemoveGroupId));
                                                        }
                                                        if (core.doc.pageController.page.triggerSendSystemEmailId != 0) {
                                                            EmailController.queueSystemEmail(core, MetadataController.getRecordName(core, "System Email", core.doc.pageController.page.triggerSendSystemEmailId), "", core.session.user.id);
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        default: {
                                                // do nothing
                                                break;
                                            }
                                    }

                                }
                            }
                        }
                    }
                    //
                    // -- Add in ContentPadding (a table around content with the appropriate padding added)
                    result = getContentBoxWrapper(core, result, core.doc.pageController.page.contentPadding);
                    //
                    // -- OnPageEndEvent
                    core.doc.bodyContent = result;
                    foreach (AddonModel addon in core.addonCache.getOnPageEndAddonList()) {
                        CPUtilsBaseClass.addonExecuteContext pageEndContext = new CPUtilsBaseClass.addonExecuteContext {
                            instanceGuid = "-1",
                            argumentKeyValuePairs = instanceArguments,
                            addonType = CPUtilsBaseClass.addonContext.ContextOnPageEnd,
                            errorContextMessage = "calling start page addon [" + addon.name + "]"
                        };
                        core.doc.bodyContent += core.addon.execute(addon, pageEndContext);
                    }
                    result = core.doc.bodyContent;
                    //
                }
                //
                // -- edit wrapper
                bool isRootPage = (core.doc.pageController.pageToRootList.Count == 1);
                if (core.session.isAdvancedEditing()) {
                    result = AdminUIController.getRecordEditAndCutAnchorTag(core, PageContentModel.tableMetadata.contentName, core.doc.pageController.page.id, (!isRootPage), core.doc.pageController.page.name) + result;
                    result = AdminUIController.getEditWrapper(core, result);
                } else if (core.session.isEditing(PageContentModel.tableMetadata.contentName)) {
                    result = AdminUIController.getRecordEditAndCutAnchorTag(core, PageContentModel.tableMetadata.contentName, core.doc.pageController.page.id, (!isRootPage), core.doc.pageController.page.name) + result;
                    result = AdminUIController.getEditWrapper(core, result);
                }
                //
                // -- title
                core.html.addTitle(core.doc.pageController.page.name);
                //
                // -- add contentid and sectionid
                core.html.addHeadTag("<meta name=\"contentId\" content=\"" + core.doc.pageController.page.id + "\" >", "page content");
                //
                // -- display Admin Warnings with Edits for record errors
                if (core.doc.adminWarning != "") {
                    if (core.doc.adminWarningPageID != 0) {
                        core.doc.adminWarning = core.doc.adminWarning + "</p>" + AdminUIController.getRecordEditAndCutAnchorTag(core, "Page Content", core.doc.adminWarningPageID, true, "Page " + core.doc.adminWarningPageID) + "&nbsp;Edit the page<p>";
                        core.doc.adminWarningPageID = 0;
                    }
                    result = AdminUIController.getAdminHintWrapper(core, core.doc.adminWarning) + result + "";
                    core.doc.adminWarning = "";
                }
                //
                // -- wrap in quick editor
                if (core.doc.redirectLink == "" && (result.IndexOf(html_quickEdit_fpo) != -1)) {
                    int FieldRows = core.userProperty.getInteger("Page Content.copyFilename.PixelHeight", 500);
                    if (FieldRows < 50) {
                        FieldRows = 50;
                        core.userProperty.setProperty("Page Content.copyFilename.PixelHeight", 50);
                    }
                    string addonListJSON = core.html.getWysiwygAddonList(CPHtml5BaseClass.EditorContentType.contentTypeWeb);
                    string Editor = core.html.getFormInputHTML("copyFilename", core.doc.quickEditCopy, FieldRows.ToString(), "100%", false, true, addonListJSON, "", "");
                    result = result.Replace(html_quickEdit_fpo, Editor);
                }
                //
                // -- Add admin warning to the top of the content
                if (core.session.isAuthenticatedAdmin() && core.doc.adminWarning != "") {
                    if (core.doc.adminWarningPageID != 0) {
                        core.doc.adminWarning += "</p>" + AdminUIController.getRecordEditAndCutAnchorTag(core, "Page Content", core.doc.adminWarningPageID, true, "Page " + core.doc.adminWarningPageID) + "&nbsp;Edit the page<p>";
                        core.doc.adminWarningPageID = 0;
                    }
                    result = AdminUIController.getAdminHintWrapper(core, core.doc.adminWarning) + result;
                    core.doc.adminWarning = "";
                }
                //
                // -- handle redirect
                if (core.doc.redirectLink != "") {
                    return core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// render the page content
        /// </summary>
        internal static string getHtmlBody_BodyTag_ContentBox_RenderPageFeatures(CoreController core) {
            StringBuilder result = new StringBuilder();
            try {
                bool isRootPage = core.doc.pageController.pageToRootList.Count.Equals(1);
                if (core.doc.pageController.page.allowReturnLinkDisplay && (!isRootPage)) {
                    //
                    // -- add Breadcrumb
                    string BreadCrumbPrefix = core.siteProperties.getText("BreadCrumbPrefix", "Return to");
                    string breadCrumb = core.doc.pageController.getReturnBreadcrumb(core);
                    if (!string.IsNullOrEmpty(breadCrumb)) {
                        breadCrumb = "\r<p class=\"ccPageListNavigation\">" + BreadCrumbPrefix + " " + breadCrumb + "</p>";
                    }
                    result.Append(breadCrumb);
                }
                //
                // -- add Page Content
                var resultInnerContent = new StringBuilder();
                // -- Headline
                if (!string.IsNullOrWhiteSpace(core.doc.pageController.page.headline)) {
                    resultInnerContent.Append("\r<h1>").Append(HtmlController.encodeHtml(core.doc.pageController.page.headline)).Append("</h1>");
                }
                bool useLegacyPageCopy = core.siteProperties.getBoolean("PageController Render Legacy Copy", false);
                if (useLegacyPageCopy) {
                    //
                    // -- legacy deprecated mode -- display copyFilename field as contnet
                    bool userLegacyQuickEditing = core.session.isQuickEditing(PageContentModel.tableMetadata.contentName);
                    if (userLegacyQuickEditing) {
                        //
                        // -- quick editor for wysiwyg content instead of rendered content
                        resultInnerContent.Append(QuickEditController.getQuickEditing(core));
                    } else {
                        //
                        // -- Render the legacy content for the page (copyFilename field)
                        string htmlPageContent = core.doc.pageController.page.copyfilename.content;
                        if (string.IsNullOrEmpty(htmlPageContent)) {
                            //
                            // Page copy is empty if  Links Enabled put in a blank line to separate edit from add tag
                            if (core.session.isEditing(PageContentModel.tableMetadata.contentName)) {
                                htmlPageContent = "\r<p><!-- Empty Content Placeholder --></p>";
                            }
                        }
                        if (core.siteProperties.beta200327_BlockCCmdPostPageRender) {
                            //
                            // -- page content, directly from the record can be {%%} rendered.
                            // -- if the content includes {%%} executables, render them
                            // -- if those executables return content containing executables, those are block by post-processing executeaddon result to remove {%%}
                            htmlPageContent = ActiveContentController.renderHtmlForWeb(core, htmlPageContent, PageContentModel.tableMetadata.contentName, core.doc.pageController.page.id, core.doc.pageController.page.contactMemberId, "http://" + core.webServer.requestDomain, core.siteProperties.defaultWrapperID, CPUtilsBaseClass.addonContext.ContextPage);
                        }
                        resultInnerContent.Append(htmlPageContent);
                    }
                } else {
                    //
                    // -- addonList mode
                    if (core.cpParent.User.IsPageBuilderEditing) {
                        //
                        // -- drag-drop editor for addonList, then include rendered content after it
                        core.docProperties.setProperty("contentid", ContentMetadataModel.getContentId(core, PageContentModel.tableMetadata.contentName));
                        core.docProperties.setProperty("recordid", core.doc.pageController.page.id);
                        resultInnerContent.Append(core.addon.execute("{92B75A6A-E84B-4551-BBF3-849E91D084BC}", new CPUtilsBaseClass.addonExecuteContext {
                            addonType = CPUtilsBaseClass.addonContext.ContextSimple
                        }));
                    } else {
                        //
                        // -- render addonList
                        if (!string.IsNullOrWhiteSpace(core.doc.pageController.page.addonList)) {
                            try {
                                AddonModel addonListRender = DbBaseModel.create<AddonModel>(core.cpParent, addonGuidRenderAddonList);
                                if (addonListRender == null) {
                                    //
                                    // -- not installed
                                    resultInnerContent.Append("<!-- Page Builder AddonList Render not available -->");
                                } else {
                                    //
                                    // -- execute PageBuilder RenderAddonList
                                    core.docProperties.setProperty("addonList", core.doc.pageController.page.addonList);
                                    resultInnerContent.Append(core.addon.execute(addonListRender, new CPUtilsBaseClass.addonExecuteContext {
                                        addonType = CPUtilsBaseClass.addonContext.ContextPage
                                    }));
                                }
                            } catch (Exception) {
                                LogController.logWarn(core, "The addonList for page [" + core.doc.pageController.page.id + ", " + core.doc.pageController.page.name + "] was not empty, but deserialized to null, addonList '" + core.doc.pageController.page.addonList + "'");
                            }
                        }
                    }
                }
                // -- End Text Search
                result.Append("\r<!-- TextSearchStart -->" + resultInnerContent.ToString() + "\r<!-- TextSearchEnd -->");
                //
                // ----- Orphan Child pages. Pages in this list do not appear on the page. This is an admin editing tool to let admins see all pages not associated to a list.
                if (core.session.isEditing()) {
                    var editItemList = new StringBuilder();
                    string sqlCriteria = "(parentid=" + core.doc.pageController.page.id + ")";
                    var usedPageidList = core.cpParent.Doc.GetText("Current Page Child PageId List", "");
                    if (!string.IsNullOrWhiteSpace(usedPageidList)) {
                        sqlCriteria += "and(id not in (" + usedPageidList + "))";
                    }
                    foreach (var page in DbBaseModel.createList<PageContentModel>(core.cpParent, sqlCriteria,"sortorder")) {
                        ContentMetadataModel contentMetadata;
                        if (page.contentControlId == 0) {
                            contentMetadata = ContentMetadataModel.createByUniqueName(core, "page content");
                        } else {
                            contentMetadata = ContentMetadataModel.create(core, page.contentControlId);

                        }
                        string pageLink = getPageLink(core, page.id, "", true, false);
                        string pageName = (!string.IsNullOrWhiteSpace(page.name)) ? page.name : "Page " + page.id;
                        string pageMenuHeadline = (!string.IsNullOrWhiteSpace(page.menuHeadline)) ? page.menuHeadline : pageName;
                        string editAnchorTag = AdminUIController.getRecordEditAndCutAnchorTag(core, contentMetadata, page.id, true, pageName);
                        string pageAnchorTag = GenericController.getLinkedText("<a href=\"" + HtmlController.encodeHtml(pageLink) + "\">", pageMenuHeadline);
                        editItemList.Append("\r<li name=\"page" + page.id + "\"  id=\"page" + page.id + "\" class=\"ccEditWrapper ccListItem allowSort\">");
                        if (!string.IsNullOrEmpty(editAnchorTag)) {
                            editItemList.Append(HtmlController.div(iconGrip, "ccListItemDragHandle") + editAnchorTag + "&nbsp;");
                        }
                        editItemList.Append(pageAnchorTag);
                        editItemList.Append("</li>");
                    }
                    var addItemList = new StringBuilder();
                    foreach (var addItem in AdminUIController.getRecordAddAnchorTag(core, "page content", "parentid=" + core.doc.pageController.page.id, true)) {
                        if (!string.IsNullOrEmpty(addItem)) { addItemList.Append("\r<li class=\"ccEditWrapper ccListItemNoBullet\">" + addItem + "</LI>"); }
                    }
                    //
                    // -- build orphan list admin tool here
                    string RequestedListName = "";
                    string hintHtml = ""
                        + HtmlController.h4("Orphan Child Pages")
                        + HtmlController.p("All child pages not assigned to a Child Page List within this page. These pages may display in menuing but do not display on this page. To display any or all of these, add a Child Page List to the page and drag these where needed.")
                        + HtmlController.ul(editItemList.ToString() + addItemList.ToString(), "ccChildList", "childPageList_" + core.doc.pageController.page.id + "_" + RequestedListName);
                    result.Append(AdminUIController.getAdminHintWrapper(core, hintHtml));
                }
                //
                // -- Allow More Info
                if ((core.doc.pageController.page.contactMemberId != 0) && core.doc.pageController.page.allowMoreInfo) {
                    result.Append(getMoreInfoHtml(core, core.doc.pageController.page.contactMemberId));
                }
                //
                // -- Last Modified line
                if (core.doc.pageController.page.allowLastModifiedFooter) {
                    DateTime pageModifiedDate = (core.doc.pageController.page.modifiedDate != null) ? encodeDate(core.doc.pageController.page.modifiedDate) : DateTime.MinValue;
                    core.cpParent.Content.LatestContentModifiedDate.Track(pageModifiedDate);
                    pageModifiedDate = core.cpParent.Content.LatestContentModifiedDate.Get();
                    if (pageModifiedDate != DateTime.MinValue)  {
                        result.Append("\r<p>This page was last modified " + encodeDate(pageModifiedDate).ToString("G"));
                        //if (core.session.isAuthenticatedAdmin()) {
                        //    if (core.doc.pageController.page.modifiedBy == 0) {
                        //        result.Append(" (admin only: modified by unknown)");
                        //    } else {
                        //        string personName = MetadataController.getRecordName(core, "people", encodeInteger(core.doc.pageController.page.modifiedBy));
                        //        if (string.IsNullOrEmpty(personName)) {
                        //            result.Append(" (admin only: modified by person with unnamed or deleted record #" + core.doc.pageController.page.modifiedBy + ")");
                        //        } else {
                        //            result.Append(" (admin only: modified by " + personName + ")");
                        //        }
                        //    }
                        //}
                        result.Append("</p>");
                    }
                }
                //
                // -- Last Reviewed line
                if ((core.doc.pageController.page.dateReviewed != DateTime.MinValue) && core.doc.pageController.page.allowReviewedFooter) {
                    result.Append("\r<p>This page was last reviewed " + encodeDate(core.doc.pageController.page.dateReviewed).ToString(""));
                    if (core.session.isAuthenticatedAdmin()) {
                        if (core.doc.pageController.page.reviewedBy == 0) {
                            result.Append(" (by unknown)");
                        } else {
                            string personName = MetadataController.getRecordName(core, "people", core.doc.pageController.page.reviewedBy);
                            if (string.IsNullOrEmpty(personName)) {
                                result.Append(" (by person with unnamed or deleted record #" + core.doc.pageController.page.reviewedBy + ")");
                            } else {
                                result.Append(" (by " + personName + ")");
                            }
                        }
                        result.Append(".</p>");
                    }
                }
                //
                // -- Page Content Message Footer
                if (core.doc.pageController.page.allowMessageFooter) {
                    string pageContentMessageFooter = core.siteProperties.getText("PageContentMessageFooter", "");
                    if (!string.IsNullOrEmpty(pageContentMessageFooter)) {
                        result.Append("\r<p>" + pageContentMessageFooter + "</p>");
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result.ToString();
        }
        //
        //====================================================================================================
        /// <summary>
        /// Add content padding around content
        /// </summary>
        /// <param name="core"></param>
        /// <param name="content"></param>
        /// <param name="contentPadding"></param>
        /// <returns></returns>
        internal static string getContentBoxWrapper(CoreController core, string content, int contentPadding) {
            if (contentPadding < 0) {
                return "<div class=\"contentBox\">" + content + "</div>";
            } else {
                return "<div class=\"contentBox\" style=\"padding:" + contentPadding + "px\">" + content + "</div>";
            }
        }
        //
        //====================================================================================================
        //
        internal static string getContentBlockMessage(CoreController core, bool UseContentWatchLink) {
            string result = "";
            try {
                var copyRecord = DbBaseModel.createByUniqueName<CopyContentModel>(core.cpParent, ContentBlockCopyName);
                if (copyRecord != null) {
                    return copyRecord.copy;
                }
                //
                // ----- Do not allow blank message - if still nothing, create default
                result = "<p>The content on this page has restricted access. If you have a username and password for this system, <a href=\"?method=login\" rel=\"nofollow\">Click Here</a>. For more information, please contact the administrator.</p>";
                copyRecord = DbBaseModel.addDefault<CopyContentModel>(core.cpParent, ContentMetadataModel.getDefaultValueDict(core, CopyContentModel.tableMetadata.contentName));
                copyRecord.name = ContentBlockCopyName;
                copyRecord.copy = result;
                copyRecord.save(core.cpParent);
                return result;
            } catch (Exception ex) {
                LogController.logError(core, ex);
                return string.Empty;
            }
        }
        //
        //====================================================================================================
        //
        public static string getMoreInfoHtml(CoreController core, int PeopleID) {
            string result = "";
            try {
                string Copy = "";
                var person = DbBaseModel.create<PersonModel>(core.cpParent, PeopleID);
                if (person != null) {
                    if (!string.IsNullOrEmpty(person.name)) {
                        Copy += "For more information, please contact " + person.name;
                        if (string.IsNullOrWhiteSpace(person.email)) {
                            if (!string.IsNullOrWhiteSpace(person.phone)) { Copy += " by phone at " + person.phone; }
                        } else {
                            Copy += " by <A href=\"mailto:" + person.email + "\">email</A>";
                            if (!string.IsNullOrWhiteSpace(person.phone)) { Copy += " or by phone at " + person.phone; }
                        }
                    } else {
                        if (string.IsNullOrWhiteSpace(person.email)) {
                            if (!string.IsNullOrWhiteSpace(person.phone)) { Copy += "For more information, please call " + person.phone; }
                        } else {
                            Copy += "For more information, please <A href=\"mailto:" + person.email + "\">email</A>";
                            if (!string.IsNullOrWhiteSpace(person.phone)) { Copy += ", or call " + person.phone; }
                        }
                    }
                }
                result = Copy;
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        //
        internal static string get2ColumnTableRow(string caption, string result, bool evenRow) {
            string copyCaption = caption;
            if (string.IsNullOrEmpty(copyCaption)) {
                copyCaption = "&nbsp;";
            }
            string CopyResult = result;
            if (string.IsNullOrEmpty(CopyResult)) {
                CopyResult = "&nbsp;";
            }
            return "<tr>"
                + HtmlController.td("<nobr>" + copyCaption + "</nobr>", "150", 0, evenRow, "right")
                + HtmlController.td(CopyResult, "100%", 0, evenRow, "left")
                + kmaEndTableRow;
        }
        //
        //====================================================================================================
        //
        public static void loadPage(CoreController core, int requestedPageId, DomainModel domain) {
            try {
                if (domain == null) {
                    //
                    // -- domain is not valid
                    LogController.logError(core, new GenericException("Page could not be determined because the domain was not recognized."));
                    return;
                }
                //
                // -- attempt requested page
                PageContentModel requestedPage = null;
                if (!requestedPageId.Equals(0)) {
                    requestedPage = DbBaseModel.create<PageContentModel>(core.cpParent, requestedPageId);
                    if (requestedPage == null) {
                        //
                        // -- requested page not found
                        requestedPage = DbBaseModel.create<PageContentModel>(core.cpParent, getPageNotFoundPageId(core));
                    }
                }
                if (requestedPage == null) {
                    //
                    // -- use the Landing Page
                    requestedPage = getLandingPage(core, domain);
                }
                //
                // -- if page has a link override, exit with redirect now.
                if (!string.IsNullOrWhiteSpace(requestedPage.link)) {
                    core.doc.redirectLink = requestedPage.link;
                    core.doc.redirectReason = "Redirecting because page includes a link override.";
                    core.doc.redirectBecausePageNotFound = false;
                    core.webServer.redirect(core.doc.redirectLink, core.doc.redirectReason, core.doc.redirectBecausePageNotFound);
                    return;
                }
                core.doc.addRefreshQueryString(rnPageId, encodeText(requestedPage.id));
                //
                // -- build parentpageList (first = current page, last = root)
                // -- add a 0, then repeat until another 0 is found, or there is a repeat
                core.doc.pageController.pageToRootList = new List<PageContentModel>();
                List<int> usedPageIdList = new List<int>();
                int targetPageId = requestedPage.id;
                usedPageIdList.Add(0);
                while (!usedPageIdList.Contains(targetPageId)) {
                    usedPageIdList.Add(targetPageId);
                    PageContentModel targetpage = DbBaseModel.create<PageContentModel>(core.cpParent, targetPageId);
                    if (targetpage == null) {
                        break;
                    } else {
                        core.doc.pageController.pageToRootList.Add(targetpage);
                        targetPageId = targetpage.parentId;
                    }
                }
                if (core.doc.pageController.pageToRootList.Count == 0) {
                    //
                    // -- attempt failed, create default page
                    core.doc.pageController.page = PageContentModel.addDefault<PageContentModel>(core.cpParent, ContentMetadataModel.getDefaultValueDict(core, PageContentModel.tableMetadata.contentName));
                    core.doc.pageController.page.name = defaultLandingPageName + ", " + domain.name;
                    core.doc.pageController.page.copyfilename.content = landingPageDefaultHtml;
                    core.doc.pageController.page.save(core.cpParent);
                    core.doc.pageController.pageToRootList.Add(core.doc.pageController.page);
                } else {
                    core.doc.pageController.page = core.doc.pageController.pageToRootList.First();
                }
                //
                // -- get template from pages
                core.doc.pageController.template = null;
                foreach (PageContentModel page in core.doc.pageController.pageToRootList) {
                    if (page.templateId > 0) {
                        core.doc.pageController.template = DbBaseModel.create<PageTemplateModel>(core.cpParent, page.templateId);
                        if (core.doc.pageController.template == null) {
                            //
                            // -- templateId is not valid
                            page.templateId = 0;
                            page.save(core.cpParent);
                        } else {
                            if (page == core.doc.pageController.pageToRootList.First()) {
                                core.doc.pageController.templateReason = "This template was used because it is selected by the current page.";
                            } else {
                                core.doc.pageController.templateReason = "This template was used because it is selected one of this page's parents [" + page.name + "].";
                            }
                            break;
                        }
                    }
                }
                //
                if (core.doc.pageController.template == null) {
                    //
                    // -- get template from domain
                    if (domain != null) {
                        if (domain.defaultTemplateId > 0) {
                            core.doc.pageController.template = DbBaseModel.create<PageTemplateModel>(core.cpParent, domain.defaultTemplateId);
                            if (core.doc.pageController.template == null) {
                                //
                                // -- domain templateId is not valid
                                domain.defaultTemplateId = 0;
                                domain.save(core.cpParent);
                            }
                        }
                    }
                    if (core.doc.pageController.template == null) {
                        //
                        // -- get the default template site property
                        core.doc.pageController.template = DbBaseModel.create<PageTemplateModel>(core.cpParent, core.siteProperties.getInteger("Default TemplateId"));
                        if (core.doc.pageController.template == null) {
                            //
                            // -- (legacy) get template named Default
                            core.doc.pageController.template = DbBaseModel.createByUniqueName<PageTemplateModel>(core.cpParent, singleColumnTemplateName);
                            if (core.doc.pageController.template == null) {
                                //
                                // -- get most recent template
                                List<PageTemplateModel> templateList = DbBaseModel.createList<PageTemplateModel>(core.cpParent, "", "id desc");
                                if (templateList.Count > 0) {
                                    core.doc.pageController.template = templateList.First();
                                } else {
                                    //
                                    // -- nothing found, create a new template matching the guid of Full Width template from themes and put just the content box
                                    core.doc.pageController.template = DbBaseModel.addDefault<PageTemplateModel>(core.cpParent);
                                    core.doc.pageController.template.name = singleColumnTemplateName;
                                    core.doc.pageController.template.ccguid = singleColumnTemplateGuid;
                                    core.doc.pageController.template.bodyHTML = Properties.Resources.DefaultTemplateHtml;
                                    core.doc.pageController.template.save(core.cpParent);
                                }
                            }
                            //
                            // -- set site property for future looks
                            core.siteProperties.setProperty("Default TemplateId", core.doc.pageController.template.id);
                        }
                    }
                }
                //
                //---------------------------------------------------------------------------------
                // ----- Set Headers
                //---------------------------------------------------------------------------------
                //
                DateTime pageModifiedDate = (core.doc.pageController.page.modifiedDate != null) ? encodeDate(core.doc.pageController.page.modifiedDate) : DateTime.MinValue;
                core.cpParent.Content.LatestContentModifiedDate.Track(pageModifiedDate);
                pageModifiedDate = core.cpParent.Content.LatestContentModifiedDate.Get();
                if (pageModifiedDate != DateTime.MinValue) {
                    core.webServer.addResponseHeader("LAST-MODIFIED", GenericController.getRFC1123PatternDateFormat(encodeDate(pageModifiedDate)));
                }
                //
                //---------------------------------------------------------------------------------
                // ----- Store page javascript
                //---------------------------------------------------------------------------------
                // todo -- assets should all come from addons !!!
                //
                core.html.addScriptCode_onLoad(core.doc.pageController.page.jSOnLoad, "page content");
                core.html.addScriptCode(core.doc.pageController.page.jSHead, "page content");
                if (core.doc.pageController.page.jSFilename != "") {
                    core.html.addScriptLinkSrc(GenericController.getCdnFileLink(core, core.doc.pageController.page.jSFilename), "page content");
                }
                core.html.addScriptCode(core.doc.pageController.page.jSEndBody, "page content");
                //
                //---------------------------------------------------------------------------------
                // Set the Meta Content flag
                //---------------------------------------------------------------------------------
                //
                core.html.addTitle(HtmlController.encodeHtml(core.doc.pageController.page.pageTitle), "page content");
                core.html.addMetaDescription(HtmlController.encodeHtml(core.doc.pageController.page.metaDescription), "page content");
                core.html.addHeadTag(core.doc.pageController.page.otherHeadTags, "page content");
                core.html.addMetaKeywordList(core.doc.pageController.page.metaKeywordList, "page content");
                //
                //---------------------------------------------------------------------------------
                // add open graph properties
                //---------------------------------------------------------------------------------
                //
                core.docProperties.setProperty("Open Graph Site Name", core.siteProperties.getText("Open Graph Site Name", core.appConfig.name));
                core.docProperties.setProperty("Open Graph Content Type", "website");
                core.docProperties.setProperty("Open Graph URL", getPageLink(core, core.doc.pageController.page.id, ""));
                core.docProperties.setProperty("Open Graph Title", HtmlController.encodeHtml(core.doc.pageController.page.pageTitle));
                core.docProperties.setProperty("Open Graph Description", HtmlController.encodeHtml(core.doc.pageController.page.metaDescription));
                core.docProperties.setProperty("Open Graph Image", (string.IsNullOrEmpty(core.doc.pageController.page.imageFilename)) ? string.Empty : core.webServer.requestProtocol + core.appConfig.domainList.First() + core.appConfig.cdnFileUrl + core.doc.pageController.page.imageFilename);
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
        //====================================================================================================
        //
        public static int getPageNotFoundPageId(CoreController core) {
            if (core.domain.pageNotFoundPageId == 0) {
                //
                // no domain page not found, use site default
                return core.siteProperties.getInteger("PageNotFoundPageID", 0);
            }
            return core.domain.pageNotFoundPageId;
        }
        //
        //====================================================================================================
        //
        public static PageContentModel getLandingPage(CoreController core, DomainModel domain) {
            PageContentModel landingPage = null;
            try {
                if (domain == null) {
                    //
                    // -- domain not available
                    LogController.logError(core, new GenericException("Landing page could not be determined because the domain was not recognized."));
                } else {
                    //
                    // -- attempt domain landing page
                    if (!domain.rootPageId.Equals(0)) {
                        landingPage = DbBaseModel.create<PageContentModel>(core.cpParent, domain.rootPageId);
                        if (landingPage == null) {
                            domain.rootPageId = 0;
                            domain.save(core.cpParent);
                        }
                    }
                    if (landingPage == null) {
                        //
                        // -- attempt site landing page
                        int siteLandingPageId = core.siteProperties.getInteger("LandingPageID", 0);
                        if (!siteLandingPageId.Equals(0)) {
                            landingPage = DbBaseModel.create<PageContentModel>(core.cpParent, siteLandingPageId);
                            if (landingPage == null) {
                                core.siteProperties.setProperty("LandingPageID", 0);
                                domain.rootPageId = 0;
                                domain.save(core.cpParent);
                            }
                        }
                        if (landingPage == null) {
                            //
                            // -- create detault landing page
                            landingPage = DbBaseModel.addDefault<PageContentModel>(core.cpParent, ContentMetadataModel.getDefaultValueDict(core, PageContentModel.tableMetadata.contentName));
                            landingPage.name = defaultLandingPageName + ", " + domain.name;
                            landingPage.copyfilename.content = landingPageDefaultHtml;
                            landingPage.save(core.cpParent);
                            core.doc.landingPageID = landingPage.id;
                        }
                        //
                        // -- save new page to the domain
                        domain.rootPageId = landingPage.id;
                        domain.save(core.cpParent);
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return landingPage;
        }
        //
        //====================================================================================================
        // Verify a link from the template link field to be used as a Template Link
        //
        internal static string verifyTemplateLink(CoreController core, string linkSrc) {
            if (string.IsNullOrEmpty(linkSrc)) { return string.Empty; }
            if (strInstr(1, linkSrc, "://") != 0) {
                //
                // protocol provided, do not fixup
                return GenericController.encodeVirtualPath(linkSrc, core.appConfig.cdnFileUrl, appRootPath, core.webServer.requestDomain);
            }
            //
            // no protocol, convert to short link
            string result = linkSrc;
            if (result.left(1) != "/") {
                //
                // page entered without path, assume it is in root path
                result = "/" + result;
            }
            result = GenericController.convertLinkToShortLink(result, core.webServer.requestDomain, core.appConfig.cdnFileUrl);
            return GenericController.encodeVirtualPath(result, core.appConfig.cdnFileUrl, appRootPath, core.webServer.requestDomain);
        }
        //
        //====================================================================================================
        //
        internal static string getPageNotFoundRedirectLink(CoreController core, string adminMessage, string BackupPageNotFoundLink = "", string PageNotFoundLink = "", int EditPageId = 0) {
            string result = "";
            try {
                int PageNotFoundPageId = getPageNotFoundPageId(core);
                string Link = null;
                if (PageNotFoundPageId == 0) {
                    //
                    // No PageNotFound was set -- use the backup link
                    if (string.IsNullOrEmpty(BackupPageNotFoundLink)) {
                        adminMessage = adminMessage + " The Site Property 'PageNotFoundPageID' is not set so the Landing Page was used.";
                        Link = core.doc.landingLink;
                    } else {
                        adminMessage = adminMessage + " The Site Property 'PageNotFoundPageID' is not set.";
                        Link = BackupPageNotFoundLink;
                    }
                } else {
                    //
                    // Set link
                    Link = getPageLink(core, PageNotFoundPageId, "", true, false);
                    string DefaultLink = getPageLink(core, 0, "", true, false);
                    if (Link != DefaultLink) {
                    } else {
                        adminMessage = adminMessage + "</p><p>The current 'Page Not Found' could not be used. It is not valid, or it is not associated with a valid site section. To configure a valid 'Page Not Found' page, first create the page as a child page on your site and check the 'Page Not Found' checkbox on it's control tab. The Landing Page was used.";
                    }
                }
                //
                // Add the Admin Message to the link
                if (core.session.isAuthenticatedAdmin()) {
                    if (string.IsNullOrEmpty(PageNotFoundLink)) {
                        PageNotFoundLink = core.webServer.requestUrl;
                    }
                    //
                    // Add the Link to the Admin Msg
                    adminMessage = adminMessage + "<p>The URL was " + PageNotFoundLink + ".";
                    //
                    // Add the Referrer to the Admin Msg
                    if (core.webServer.requestReferer != "") {
                        String referer = core.webServer.requestReferrer;
                        int Pos = GenericController.strInstr(1, referer, "AdminWarningPageID=", 1);
                        if (Pos != 0) {
                            referer = referer.left(Pos - 2);
                        }
                        Pos = GenericController.strInstr(1, referer, "AdminWarningMsg=", 1);
                        if (Pos != 0) {
                            referer = referer.left(Pos - 2);
                        }
                        Pos = GenericController.strInstr(1, referer, "blockcontenttracking=", 1);
                        if (Pos != 0) {
                            referer = referer.left(Pos - 2);
                        }
                        adminMessage = adminMessage + " The referring page was " + referer + ".";
                    }
                    //
                    adminMessage = adminMessage + "</p>";
                    //
                    if (EditPageId != 0) {
                        Link = GenericController.modifyLinkQuery(Link, "AdminWarningPageID", EditPageId.ToString(), true);
                    }
                    //
                    Link = GenericController.modifyLinkQuery(Link, rnBlockContentTracking, "1", true);
                    Link = GenericController.modifyLinkQuery(Link, "AdminWarningMsg", "<p>" + adminMessage + "</p>", true);
                }
                //
                result = Link;
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public static string getPageLink(CoreController core, int PageID, string QueryStringSuffix, bool AllowLinkAliasIfEnabled = true, bool UseContentWatchNotDefaultPage = false) {
            string result = "";
            try {
                //
                // -- set linkPathPath to linkAlias
                string linkPathPage = null;
                if (AllowLinkAliasIfEnabled && core.siteProperties.allowLinkAlias) {
                    linkPathPage = LinkAliasController.getLinkAlias(core, PageID, QueryStringSuffix, "");
                }
                if (string.IsNullOrEmpty(linkPathPage)) {
                    //
                    // -- if not linkAlis, set default page and qs
                    linkPathPage = core.siteProperties.serverPageDefault;
                    if (string.IsNullOrEmpty(linkPathPage)) {
                        linkPathPage = "/" + getDefaultScript();
                    } else {
                        int Pos = GenericController.strInstr(1, linkPathPage, "?");
                        if (Pos != 0) {
                            linkPathPage = linkPathPage.left(Pos - 1);
                        }
                        if (linkPathPage.left(1) != "/") {
                            linkPathPage = "/" + linkPathPage;
                        }
                    }
                    //
                    // -- calc linkQS (cleared in come cases later)
                    linkPathPage += "?" + rnPageId + "=" + PageID;
                    if (!string.IsNullOrEmpty(QueryStringSuffix)) {
                        linkPathPage += "&" + QueryStringSuffix;
                    }
                }
                //
                // -- domain -- determine if the domain has any template requirements, and if so, is this template allowed
                string SqlCriteria = "(domainId=" + core.doc.domain.id + ")";
                List<TemplateDomainRuleModel> allowTemplateRuleList = DbBaseModel.createList<TemplateDomainRuleModel>(core.cpParent, SqlCriteria);
                bool templateAllowed = false;
                foreach (TemplateDomainRuleModel rule in allowTemplateRuleList) {
                    if (rule.templateId == core.doc.pageController.template.id) {
                        templateAllowed = true;
                        break;
                    }
                }
                string linkDomain = "";
                if (allowTemplateRuleList.Count == 0) {
                    //
                    // this template has no domain preference, use current domain
                    //
                    linkDomain = core.webServer.requestDomain;
                } else if (core.domain.id == 0) {
                    //
                    // the current domain is not recognized, or is default - use it
                    //
                    linkDomain = core.webServer.requestDomain;
                } else if (templateAllowed) {
                    //
                    // current domain is in the allowed domain list
                    //
                    linkDomain = core.webServer.requestDomain;
                } else {
                    //
                    // there is an allowed domain list and current domain is not on it, or use first
                    //
                    int setdomainId = allowTemplateRuleList.First().domainId;
                    linkDomain = MetadataController.getRecordName(core, "domains", setdomainId);
                    if (string.IsNullOrEmpty(linkDomain)) {
                        linkDomain = core.webServer.requestDomain;
                    }
                }
                //
                // -- protocol
                string linkprotocol = "";
                if (core.doc.pageController.page.isSecure || core.doc.pageController.template.isSecure) {
                    linkprotocol = "https://";
                } else {
                    linkprotocol = "http://";
                }
                //
                // -- assemble
                result = linkprotocol + linkDomain + linkPathPage;
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        //
        internal static string getDefaultScript() { return "default.aspx"; }
        //
        //
        //
        private static void processForm(CoreController core, int FormPageID) {
            try {
                //
                // main_Get the instructions from the record
                string Formhtml = "";
                string FormInstructions = "";
                using (var csData = new CsModel(core)) {
                    csData.openRecord("Form Pages", FormPageID);
                    if (csData.ok()) {
                        Formhtml = csData.getText("Body");
                        FormInstructions = csData.getText("Instructions");
                    }
                }
                PageFormModel pageForm;
                if (!string.IsNullOrEmpty(FormInstructions)) {
                    //
                    // Load the instructions
                    //
                    pageForm = loadFormPageInstructions(core, FormInstructions, Formhtml);
                    if (pageForm.authenticateOnFormProcess && !core.session.isAuthenticated && core.session.isRecognized()) {
                        //
                        // If this form will authenticate when done, and their is a current, non-authenticated account -- logout first
                        //
                        core.session.logout();
                    }
                    bool Success = true;
                    int Ptr = 0;
                    string PeopleFirstName = "";
                    string PeopleLastName = "";
                    string PeopleName = "";
                    Models.Domain.ContentMetadataModel peopleMeta = null;
                    foreach (var formField in pageForm.formFieldList) {
                        bool IsInGroup = false;
                        bool WasInGroup = false;
                        string FormValue = null;
                        switch (formField.type) {
                            case 1: {
                                    //
                                    // People Record
                                    //
                                    if (peopleMeta == null) { peopleMeta = Models.Domain.ContentMetadataModel.createByUniqueName(core, "people"); }
                                    var peopleFieldMeta = Models.Domain.ContentMetadataModel.getField(core, peopleMeta, formField.peopleFieldName);
                                    if (peopleFieldMeta != null) {
                                        FormValue = core.docProperties.getText(formField.peopleFieldName);
                                        if ((!string.IsNullOrEmpty(FormValue)) && peopleFieldMeta.uniqueName) {
                                            using (var csData = new CsModel(core)) {
                                                string SQL = "select count(*) from ccMembers where " + formField.peopleFieldName + "=" + DbController.encodeSQLText(FormValue);
                                                csData.openSql(SQL);
                                                if (csData.ok()) {
                                                    Success = csData.getInteger("cnt") == 0;
                                                }
                                            }
                                            if (!Success) {
                                                ErrorController.addUserError(core, "The field [" + formField.caption + "] must be unique, and the value [" + HtmlController.encodeHtml(FormValue) + "] has already been used.");
                                            }
                                        }
                                        if ((formField.required || peopleFieldMeta.required) && string.IsNullOrEmpty(FormValue)) {
                                            Success = false;
                                            ErrorController.addUserError(core, "The field [" + HtmlController.encodeHtml(formField.caption) + "] is required.");
                                        } else {
                                            using (var csData = new CsModel(core)) {
                                                if (!csData.ok()) {
                                                    csData.openRecord("people", core.session.user.id);
                                                }
                                                if (csData.ok()) {
                                                    string PeopleUsername = null;
                                                    string PeoplePassword = null;
                                                    string PeopleEmail = "";
                                                    switch (GenericController.toUCase(formField.peopleFieldName)) {
                                                        case "NAME": {
                                                                PeopleName = FormValue;
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                        case "FIRSTNAME": {
                                                                PeopleFirstName = FormValue;
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                        case "LASTNAME": {
                                                                PeopleLastName = FormValue;
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                        case "EMAIL": {
                                                                PeopleEmail = FormValue;
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                        case "USERNAME": {
                                                                PeopleUsername = FormValue;
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                        case "PASSWORD": {
                                                                PeoplePassword = FormValue;
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                        default: {
                                                                csData.set(formField.peopleFieldName, FormValue);
                                                                break;
                                                            }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 2: {
                                    //
                                    // Group main_MemberShip
                                    //
                                    IsInGroup = core.docProperties.getBoolean("Group" + formField.groupName);
                                    WasInGroup = GroupController.isMemberOfGroup(core, formField.groupName);
                                    if (WasInGroup && !IsInGroup) {
                                        GroupController.removeUser(core, formField.groupName);
                                    } else if (IsInGroup && !WasInGroup) {
                                        GroupController.addUser(core, formField.groupName);
                                    }
                                    break;
                                }
                            default: {
                                    // do nothing
                                    break;
                                }
                        }
                    }
                    //
                    // Create People Name
                    //
                    if (string.IsNullOrWhiteSpace(PeopleName) && !string.IsNullOrWhiteSpace(PeopleFirstName) && !string.IsNullOrWhiteSpace(PeopleLastName)) {
                        using (var csData = new CsModel(core)) {
                            if (csData.openRecord("people", core.session.user.id)) {
                                csData.set("name", PeopleFirstName + " " + PeopleLastName);
                            }
                        }
                    }
                    //
                    // AuthenticationOnFormProcess requires Username/Password and must be valid
                    //
                    if (Success) {
                        //
                        // Authenticate
                        //
                        if (pageForm.authenticateOnFormProcess) {
                            core.session.authenticateById(core.session.user.id, core.session);
                        }
                        //
                        // Join Group requested by page that created form
                        var groupToken = SecurityController.decodeToken(core, core.docProperties.getText("SuccessID"));
                        if (groupToken.id != 0) {
                            GroupController.addUser(core, GroupController.getGroupName(core, groupToken.id));
                        }
                        //
                        // Join Groups requested by pageform
                        if (pageForm.addGroupNameList != "") {
                            string[] Groups = (encodeText(pageForm.addGroupNameList).Trim(' ')).Split(',');
                            for (Ptr = 0; Ptr <= Groups.GetUpperBound(0); Ptr++) {
                                string GroupName = Groups[Ptr].Trim(' ');
                                if (!string.IsNullOrEmpty(GroupName)) {
                                    GroupController.addUser(core, GroupName);
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
        // Instruction Format
        //   Line 1 - Version FormBuilderVersion
        //   Line 2+ instruction lines
        //   blank line
        //   FormHTML
        //
        // Instruction Line Format
        //   Type,Caption,Required,arguments
        //
        //   Types
        //       1 = People Content field
        //           arguments = FieldName
        //       2 = Group main_MemberShip
        //           arguments = GroupName
        //
        // FormHTML
        //   All HTML with the following:
        //   ##REPEAT></REPEAT> tags -- repeated for each instruction line
        //   {{CAPTION}} tags -- main_Gets the caption for each instruction line
        //   {{FIELD}} tags -- main_Gets the form field for each instruction line
        //
        internal static PageFormModel loadFormPageInstructions(CoreController core, string FormInstructions, string Formhtml) {
            PageFormModel result = new PageFormModel();
            try {
                //
                {
                    int PtrFront = GenericController.strInstr(1, Formhtml, "{{REPEATSTART", 1);
                    if (PtrFront > 0) {
                        int PtrBack = GenericController.strInstr(PtrFront, Formhtml, "}}");
                        if (PtrBack > 0) {
                            result.preRepeat = Formhtml.left(PtrFront - 1);
                            PtrFront = GenericController.strInstr(PtrBack, Formhtml, "{{REPEATEND", 1);
                            if (PtrFront > 0) {
                                result.repeatCell = Formhtml.Substring(PtrBack + 1, PtrFront - PtrBack - 2);
                                PtrBack = GenericController.strInstr(PtrFront, Formhtml, "}}");
                                if (PtrBack > 0) {
                                    result.postRepeat = Formhtml.Substring(PtrBack + 1);
                                    //
                                    // Decode instructions and build output
                                    //
                                    string[] i = GenericController.splitNewLine(FormInstructions);
                                    if (i.GetUpperBound(0) > 0) {
                                        int IStart = 0;
                                        if (string.CompareOrdinal(i[0].Trim(' '), "1") >= 0) {
                                            //
                                            // decode Version 1 arguments, then start instructions line at line 1
                                            //
                                            result.addGroupNameList = GenericController.encodeText(i[1]);
                                            result.authenticateOnFormProcess = GenericController.encodeBoolean(i[2]);
                                            IStart = 3;
                                        }
                                        //
                                        // read in and compose the repeat lines
                                        //
                                        int IPtr = 0;
                                        for (IPtr = 0; IPtr <= i.GetUpperBound(0) - IStart; IPtr++) {
                                            var tempVar = result.formFieldList[IPtr];
                                            string[] IArgs = i[IPtr + IStart].Split(',');
                                            if (IArgs.GetUpperBound(0) >= main_IPosMax) {
                                                tempVar.caption = IArgs[main_IPosCaption];
                                                tempVar.type = GenericController.encodeInteger(IArgs[main_IPosType]);
                                                tempVar.required = GenericController.encodeBoolean(IArgs[main_IPosRequired]);
                                                switch (tempVar.type) {
                                                    case 1: {
                                                            //
                                                            // People Record
                                                            //
                                                            tempVar.peopleFieldName = IArgs[main_IPosPeopleField];
                                                            break;
                                                        }
                                                    case 2: {
                                                            //
                                                            // Group main_MemberShip
                                                            //
                                                            tempVar.groupName = IArgs[main_IPosGroupName];
                                                            break;
                                                        }
                                                    default: {
                                                            // do nothing
                                                            break;
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //
        //
        //
        internal static string getFormPage(CoreController core, string FormPageName, int GroupIDToJoinOnSuccess) {
            string result = null;
            try {
                PageFormModel pageForm;
                string Formhtml = "";
                string FormInstructions = "";
                bool IsRetry = (core.docProperties.getInteger("ContensiveFormPageID") != 0);
                int FormPageId = 0;
                using (var csData = new CsModel(core)) {
                    csData.open("Form Pages", "name=" + DbController.encodeSQLText(FormPageName));
                    if (csData.ok()) {
                        FormPageId = csData.getInteger("ID");
                        Formhtml = csData.getText("Body");
                        FormInstructions = csData.getText("Instructions");
                    }
                }
                pageForm = loadFormPageInstructions(core, FormInstructions, Formhtml);
                string RepeatBody = "";
                string Body = null;
                bool HasRequiredFields = false;
                var peopleMeta = Models.Domain.ContentMetadataModel.createByUniqueName(core, "people");
                foreach (var formField in pageForm.formFieldList) {
                    bool GroupValue = false;
                    int GroupRowPtr = 0;
                    string CaptionSpan = null;
                    string Caption = null;
                    switch (formField.type) {
                        case 1: {
                                //
                                // People Record
                                //
                                var peopleMetaField = Models.Domain.ContentMetadataModel.getField(core, peopleMeta, formField.peopleFieldName);
                                if (IsRetry && core.docProperties.getText(formField.peopleFieldName) == "") {
                                    CaptionSpan = "<span class=\"ccError\">";
                                } else {
                                    CaptionSpan = "<span>";
                                }
                                Caption = formField.caption;
                                if (formField.required || peopleMetaField.required) {
                                    Caption = "*" + Caption;
                                }
                                using (var csPeople = new CsModel(core)) {
                                    csPeople.openRecord("people", core.session.user.id);
                                    if (csPeople.ok()) {
                                        Body = pageForm.repeatCell;
                                        Body = GenericController.strReplace(Body, "{{CAPTION}}", CaptionSpan + Caption + "</span>", 1, 99, 1);
                                        Body = GenericController.strReplace(Body, "{{FIELD}}", core.html.inputCs(csPeople, "People", formField.peopleFieldName), 1, 99, 1);
                                        RepeatBody = RepeatBody + Body;
                                        HasRequiredFields = HasRequiredFields || formField.required;
                                    }
                                }

                                break;
                            }
                        case 2: {
                                //
                                // Group main_MemberShip
                                //
                                GroupValue = GroupController.isMemberOfGroup(core, formField.groupName);
                                Body = pageForm.repeatCell;
                                Body = GenericController.strReplace(Body, "{{CAPTION}}", HtmlController.checkbox("Group" + formField.groupName, GroupValue), 1, 99, 1);
                                Body = GenericController.strReplace(Body, "{{FIELD}}", formField.caption);
                                RepeatBody = RepeatBody + Body;
                                GroupRowPtr = GroupRowPtr + 1;
                                HasRequiredFields = HasRequiredFields || formField.required;
                                break;
                            }
                        default: {
                                // do nothing
                                break;
                            }
                    }
                }
                if (HasRequiredFields) {
                    Body = pageForm.repeatCell;
                    Body = GenericController.strReplace(Body, "{{CAPTION}}", "&nbsp;", 1, 99, 1);
                    Body = GenericController.strReplace(Body, "{{FIELD}}", "*&nbsp;Required Fields");
                    RepeatBody = RepeatBody + Body;
                }
                //
                string innerHtml = ""
                    + HtmlController.inputHidden("ContensiveFormPageID", FormPageId)
                    + HtmlController.inputHidden("SuccessID", SecurityController.encodeToken(core, GroupIDToJoinOnSuccess, core.doc.profileStartTime.AddMinutes(30)))
                    + pageForm.preRepeat
                    + RepeatBody
                    + pageForm.postRepeat;
                result = ""
                    + ErrorController.getUserError(core)
                    + HtmlController.formMultipart(core, innerHtml, core.doc.refreshQueryString, "", "ccForm");
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //========================================================================
        // ----- prints a link to the feedback popup form
        //
        //   Creates a sub-form that when submitted, is logged by the notes
        //   system (in MembersLib right now). When submitted, it prints a thank you
        //   message.
        //
        //========================================================================
        //
        public static string getFeedbackForm(CoreController core, string ContentName, int RecordID, int ToMemberID, string headline = "") {
            string result = "";
            try {
                const string FeedbackButtonSubmit = "Submit";
                //
                string FeedbackButton = core.docProperties.getText("fbb");
                string Panel = null;
                string Copy = null;
                string NoteCopy = "";
                string NoteFromEmail = null;
                string NoteFromName = null;
                switch (FeedbackButton) {
                    case FeedbackButtonSubmit: {
                            //
                            // ----- form was submitted, save the note, send it and say thanks
                            //
                            NoteFromName = core.docProperties.getText("NoteFromName");
                            NoteFromEmail = core.docProperties.getText("NoteFromEmail");
                            //
                            NoteCopy = NoteCopy + "Feedback Submitted" + BR;
                            NoteCopy = NoteCopy + "From " + NoteFromName + " at " + NoteFromEmail + BR;
                            NoteCopy = NoteCopy + "Replying to:" + BR;
                            if (!string.IsNullOrEmpty(headline)) {
                                NoteCopy = NoteCopy + "    Article titled [" + headline + "]" + BR;
                            }
                            NoteCopy = NoteCopy + "    Record [" + RecordID + "] in Content Definition [" + ContentName + "]" + BR;
                            NoteCopy = NoteCopy + BR;
                            NoteCopy = NoteCopy + "<b>Comments</b>" + BR;
                            //
                            Copy = core.docProperties.getText("NoteCopy");
                            if (string.IsNullOrEmpty(Copy)) {
                                NoteCopy = NoteCopy + "[no comments entered]" + BR;
                            } else {
                                NoteCopy = NoteCopy + HtmlController.convertNewLineToHtmlBreak(Copy) + BR;
                            }
                            //
                            NoteCopy = NoteCopy + BR;
                            NoteCopy = NoteCopy + "<b>Content on which the comments are based</b>" + BR;
                            //
                            using (var csData = new CsModel(core)) {
                                csData.open(ContentName, "ID=" + RecordID);
                                Copy = "[the content of this page is not available]" + BR;
                                if (csData.ok()) {
                                    Copy = (csData.getText("copyFilename"));
                                }
                                NoteCopy = NoteCopy + Copy + BR;
                            }
                            //
                            PersonModel person = DbBaseModel.create<PersonModel>(core.cpParent, ToMemberID);
                            if (person != null) {
                                string sendStatus = "";
                                string queryStringForLinkAppend = "";
                                EmailController.queuePersonEmail(core, person, NoteFromEmail, "Feedback Form Submitted", NoteCopy, "", "", false, true, 0, "", false, ref sendStatus, queryStringForLinkAppend, "Feedback Form Email");
                            }
                            //
                            // ----- Note sent, say thanks
                            //
                            result += "<p>Thank you. Your feedback was received.</p>";
                            break;
                        }
                    default: {
                            //
                            // ----- print the feedback submit form
                            //
                            Panel = "<form Action=\"" + core.webServer.serverFormActionURL + "?" + core.doc.refreshQueryString + "\" Method=\"post\">";
                            Panel = Panel + "<table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%\">";
                            Panel = Panel + "<tr>";
                            Panel = Panel + "<td colspan=\"2\"><p>Your feedback is welcome</p></td>";
                            Panel = Panel + "</tr><tr>";
                            //
                            // ----- From Name
                            //
                            Copy = core.session.user.name;
                            Panel = Panel + "<td align=\"right\" width=\"100\"><p>Your Name</p></td>";
                            Panel = Panel + "<td align=\"left\"><input type=\"text\" name=\"NoteFromName\" value=\"" + HtmlController.encodeHtml(Copy) + "\"></span></td>";
                            Panel = Panel + "</tr><tr>";
                            //
                            // ----- From Email address
                            //
                            Copy = core.session.user.email;
                            Panel = Panel + "<td align=\"right\" width=\"100\"><p>Your Email</p></td>";
                            Panel = Panel + "<td align=\"left\"><input type=\"text\" name=\"NoteFromEmail\" value=\"" + HtmlController.encodeHtml(Copy) + "\"></span></td>";
                            Panel = Panel + "</tr><tr>";
                            //
                            // ----- Message
                            //
                            Copy = "";
                            Panel = Panel + "<td align=\"right\" width=\"100\" valign=\"top\"><p>Feedback</p></td>";
                            Panel = Panel + "<td>" + HtmlController.inputText_Legacy(core, "NoteCopy", Copy, 4, 40, "TextArea", false) + "</td>";
                            Panel = Panel + "</tr><tr>";
                            //
                            // ----- submit button
                            //
                            Panel = Panel + "<td>&nbsp;</td>";
                            Panel = Panel + "<td>" + HtmlController.inputSubmit(FeedbackButtonSubmit, "fbb") + "</td>";
                            Panel = Panel + "</tr></table>";
                            Panel = Panel + "</form>";
                            //
                            result = Panel;
                            break;
                        }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public static string getWatchList(CoreController core, string ListName, string SortField, bool SortReverse) {
            string result = "";
            try {
                using (var csData = new CsModel(core)) {
                    if (SortReverse && (!string.IsNullOrEmpty(SortField))) {
                        csData.openContentWatchList(ListName, SortField + " Desc", true);
                    } else {
                        csData.openContentWatchList(ListName, SortField, true);
                    }
                    //
                    if (csData.ok()) {
                        int ContentId = Models.Domain.ContentMetadataModel.getContentId(core, "Content Watch");
                        while (csData.ok()) {
                            string Link = csData.getText("link");
                            string LinkLabel = csData.getText("LinkLabel");
                            int RecordId = csData.getInteger("ID");
                            if (!string.IsNullOrEmpty(LinkLabel)) {
                                result += "\r<li id=\"main_ContentWatch" + RecordId + "\" class=\"ccListItem\">";
                                if (!string.IsNullOrEmpty(Link)) {
                                    result += "<a href=\"http://" + core.webServer.requestDomain + "/" + core.webServer.requestPage + "?rc=" + ContentId + "&ri=" + RecordId + "\">" + LinkLabel + "</a>";
                                } else {
                                    result += LinkLabel;
                                }
                                result += "</li>";
                            }
                        }
                        if (!string.IsNullOrEmpty(result)) {
                            result = core.html.getContentCopy("Watch List Caption: " + ListName, ListName, core.session.user.id, true, core.session.isAuthenticated) + "\r<ul class=\"ccWatchList\">" + nop(result) + "\r</ul>";
                        }
                    }
                }
                //
                if (core.visitProperty.getBoolean("AllowAdvancedEditor")) {
                    result = AdminUIController.getEditWrapper(core, result);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //========================================================================
        //
        public static void processFormQuickEditing(CoreController core) {
            //
            int recordId = (core.docProperties.getInteger("ID"));
            string button = core.docProperties.getText("Button");
            if ((!string.IsNullOrEmpty(button)) && (recordId != 0) && (core.session.isAuthenticatedContentManager(PageContentModel.tableMetadata.contentName))) {
                var pageCdef = Models.Domain.ContentMetadataModel.createByUniqueName(core, "page content");
                var pageTable = TableModel.createByContentName(core.cpParent, pageCdef.name);
                WorkflowController.editLockClass editLock = WorkflowController.getEditLock(core, pageTable.id, recordId);
                WorkflowController.clearEditLock(core, pageTable.id, recordId);
                //
                // tough case, in Quick mode, lets mark the record reviewed, no matter what button they push, except cancel
                if (button != ButtonCancel) {
                    PageContentModel.markReviewed(core.cpParent, recordId);
                }
                bool allowSave = false;
                //
                // Determine is the record should be saved
                //
                if (core.session.isAuthenticatedAdmin()) {
                    //
                    // cases that admin can save
                    allowSave = (button == ButtonAddChildPage) || (button == ButtonAddSiblingPage) || (button == ButtonSave) || (button == ButtonOK);
                } else {
                    //
                    // cases that CM can save
                    allowSave = (button == ButtonAddChildPage) || (button == ButtonAddSiblingPage) || (button == ButtonSave) || (button == ButtonOK);
                }
                if (allowSave) {
                    //
                    // ----- Save Changes
                    bool SaveButNoChanges = true;
                    PageContentModel page = DbBaseModel.create<PageContentModel>(core.cpParent, recordId);
                    if (page != null) {
                        string Copy = core.docProperties.getText("copyFilename");
                        Copy = ActiveContentController.processWysiwygResponseForSave(core, Copy);
                        if (Copy != page.copyfilename.content) {
                            page.copyfilename.content = Copy;
                            SaveButNoChanges = false;
                        }
                        string RecordName = core.docProperties.getText("name");
                        if (RecordName != page.name) {
                            page.name = RecordName;
                            LinkAliasController.addLinkAlias(core, RecordName, recordId, "");
                            SaveButNoChanges = false;
                        }
                        page.save(core.cpParent);
                        WorkflowController.setEditLock(core, pageTable.id, page.id);
                        if (!SaveButNoChanges) {
                            ContentController.processAfterSave(core, false, pageCdef.name, page.id, page.name, page.parentId, false);
                            DbBaseModel.invalidateCacheOfRecord<PageContentModel>(core.cpParent, page.id);
                        }
                    }
                }
                string Link = null;
                if (button == ButtonAddChildPage) {
                    using (var csData = new CsModel(core)) {
                        csData.insert(pageCdef.name);
                        if (csData.ok()) {
                            csData.set("active", true);
                            csData.set("ParentID", recordId);
                            csData.set("contactmemberid", core.session.user.id);
                            csData.set("name", "New Page added " + core.doc.profileStartTime + " by " + core.session.user.name);
                            csData.set("copyFilename", "");
                            csData.save();
                            recordId = csData.getInteger("ID");
                            Link = PageContentController.getPageLink(core, recordId, "", true, false);
                            core.webServer.redirect(Link, "Redirecting because a new page has been added with the quick editor.", false, false);
                        }
                    }
                    DbBaseModel.invalidateCacheOfRecord<PageContentModel>(core.cpParent, recordId);
                }
                int ParentId = 0;
                if (button == ButtonAddSiblingPage) {
                    //
                    //
                    //
                    using (var csData = new CsModel(core)) {
                        csData.openRecord(pageCdef.name, recordId, "ParentID");
                        if (csData.ok()) {
                            ParentId = csData.getInteger("ParentID");
                        }
                        if (ParentId != 0) {
                            csData.insert(pageCdef.name);
                            if (csData.ok()) {
                                csData.set("active", true);
                                csData.set("ParentID", ParentId);
                                csData.set("contactmemberid", core.session.user.id);
                                csData.set("name", "New Page added " + core.doc.profileStartTime + " by " + core.session.user.name);
                                csData.set("copyFilename", "");
                                csData.save();
                                recordId = csData.getInteger("ID");
                                //
                                Link = PageContentController.getPageLink(core, recordId, "", true, false);
                                core.webServer.redirect(Link, "Redirecting because a new page has been added with the quick editor.", false, false);
                            }
                        }
                    }
                }
                if (button == ButtonDelete) {
                    using (var csData = new CsModel(core)) {
                        csData.openRecord(pageCdef.name, recordId);
                        if (csData.ok()) {
                            ParentId = csData.getInteger("parentid");
                        }
                    }
                    //
                    MetadataController.deleteContentRecord(core, pageCdef.name, recordId);
                    DbBaseModel.invalidateCacheOfRecord<PageContentModel>(core.cpParent, recordId);
                    //
                    if (!false) {
                        Link = PageContentController.getPageLink(core, ParentId, "", true, false);
                        Link = GenericController.modifyLinkQuery(Link, "AdminWarningMsg", "The page has been deleted, and you have been redirected to the parent of the deleted page.", true);
                        core.webServer.redirect(Link, "Redirecting to the parent page because the page was deleted with the quick editor.", core.doc.redirectBecausePageNotFound, false);
                        return;
                    }
                }
                if ((!(!core.doc.userErrorList.Count.Equals(0))) && ((button == ButtonOK) || (button == ButtonCancel))) {
                    //
                    // ----- Turn off Quick Editor if not save or add child
                    //
                    core.visitProperty.setProperty("AllowQuickEditor", false);
                }
            }
        }
        //
        //========================================================================
        // Print Whats New
        //   Prints a linked list of new content
        //========================================================================
        //
        public static string getWhatsNewList(CoreController core, string SortFieldList = "") {
            string result = "";
            try {
                int ContentId = 0;
                int RecordId = 0;
                string LinkLabel = null;
                string Link = null;
                //
                using (var csData = new CsModel(core)) {
                    csData.openWhatsNew(SortFieldList);
                    //
                    if (csData.ok()) {
                        ContentId = Models.Domain.ContentMetadataModel.getContentId(core, "Content Watch");
                        while (csData.ok()) {
                            Link = csData.getText("link");
                            LinkLabel = csData.getText("LinkLabel");
                            RecordId = csData.getInteger("ID");
                            if (!string.IsNullOrEmpty(LinkLabel)) {
                                result += "\r<li class=\"ccListItem\">";
                                if (!string.IsNullOrEmpty(Link)) {
                                    result += GenericController.getLinkedText("<a href=\"" + HtmlController.encodeHtml(core.webServer.requestPage + "?rc=" + ContentId + "&ri=" + RecordId) + "\">", LinkLabel);
                                } else {
                                    result += LinkLabel;
                                }
                                result += "</li>";
                            }
                            csData.goNext();
                        }
                        result = "\r<ul class=\"ccWatchList\">" + nop(result) + "\r</ul>";
                    }
                    csData.close();
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
        //=============================================================================
        //
        internal string getReturnBreadcrumb(CoreController core) {
            string returnHtml = "";
            //
            foreach (PageContentModel testpage in pageToRootList) {
                string pageCaption = testpage.menuHeadline;
                if (string.IsNullOrEmpty(pageCaption)) {
                    pageCaption = GenericController.encodeText(testpage.name);
                }
                if (string.IsNullOrEmpty(returnHtml)) {
                    returnHtml = pageCaption;
                } else {
                    string BreadCrumbDelimiter = core.siteProperties.getText("BreadCrumbDelimiter", " &gt; ");
                    returnHtml = "<a href=\"" + HtmlController.encodeHtml(PageContentController.getPageLink(core, testpage.id, "", true, false)) + "\">" + pageCaption + "</a>" + BreadCrumbDelimiter + returnHtml;
                }
            }
            return returnHtml;
        }
        //       
        //=============================================================================
        /// <summary>
        /// Is This member allowed through the content block
        /// </summary>
        /// <param name="core"></param>
        /// <param name="ContentID"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static bool allowThroughPageBlock(CoreController core, int pageId) {
            bool result = false;
            try {
                if (core.session.isAuthenticatedAdmin()) { return true; }
                string sql = "SELECT ccMemberRules.memberId"
                    + " FROM (ccPageContentBlockRules LEFT JOIN ccgroups ON ccPageContentBlockRules.GroupId = ccgroups.ID) LEFT JOIN ccMemberRules ON ccgroups.Id = ccMemberRules.GroupID"
                    + " WHERE (((ccPageContentBlockRules.RecordID)=" + pageId + ")"
                    + " AND ((ccPageContentBlockRules.Active)<>0)"
                    + " AND ((ccgroups.Active)<>0)"
                    + " AND ((ccMemberRules.Active)<>0)"
                    + " AND ((ccMemberRules.DateExpires) Is Null Or (ccMemberRules.DateExpires)>" + DbController.encodeSQLDate(core.doc.profileStartTime) + ")"
                    + " AND ((ccMemberRules.memberId)=" + core.session.user.id + "));";
                int recordsReturned = 0;
                DataTable dt = core.db.executeQuery(sql, 0, 1, ref recordsReturned);
                return !recordsReturned.Equals(0);
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //       
        //=============================================================================
        /// <summary>
        /// Attempt to perform a child-record paste into a child page list
        /// </summary>
        /// <param name="core"></param>
        /// <param name="pasteParentContentId"></param>
        /// <param name="pasteParentRecordId"></param>
        private static void attemptClipboardPaste(CoreController core, int pasteParentContentId, int pasteParentRecordId) {
            if ((pasteParentContentId == 0) || (pasteParentRecordId == 0)) { return; }
            //
            var pasteParentContentMetadata = Models.Domain.ContentMetadataModel.create(core, pasteParentContentId);
            if (pasteParentContentMetadata == null) { return; }
            //
            // -- get current clipboard values to be pasted here (and clear the clipboard)
            string cutClip = core.visitProperty.getText("Clipboard", "");
            if (string.IsNullOrWhiteSpace(cutClip)) { return; }
            core.visitProperty.setProperty("Clipboard", "");
            //
            // -- clear the paste from refresh click
            GenericController.modifyQueryString(core.doc.refreshQueryString, RequestNamePasteParentContentId, "");
            GenericController.modifyQueryString(core.doc.refreshQueryString, RequestNamePasteParentRecordId, "");

            {
                if (!core.session.isAuthenticatedContentManager(pasteParentContentMetadata)) {
                    ErrorController.addUserError(core, "The paste operation failed because you are not a content manager of the Clip Parent");
                } else {
                    //
                    // -- get the cut contentid and recordId from the clip
                    if (cutClip.IndexOf('.') < 0) { return; }
                    string[] clipBoardArray = cutClip.Split('.');
                    int cutClipContentId = GenericController.encodeInteger(clipBoardArray[0]);
                    int cutClipRecordId = GenericController.encodeInteger(clipBoardArray[1]);
                    if (!pasteParentContentMetadata.isParentOf(core, cutClipContentId)) { return; }
                    var clipChildContentMetadata = Models.Domain.ContentMetadataModel.create(core, cutClipContentId);
                    if (DbBaseModel.isChildOf<PageContentController>(core.cpParent, pasteParentRecordId, cutClipRecordId, new List<int>())) {
                        ErrorController.addUserError(core, "The paste operation failed because the destination location is a child of the clipboard data record.");
                    } else {
                        //
                        // the parent record is not a child of the child record (circular check)
                        //
                        string ClipChildRecordName = "record " + cutClipRecordId;
                        using (var csData = new CsModel(core)) {
                            csData.openRecord(clipChildContentMetadata.name, cutClipRecordId);
                            if (!csData.ok()) {
                                ErrorController.addUserError(core, "The paste operation failed because the data record referenced by the clipboard could not found.");
                            } else {
                                //
                                // Paste the edit record record
                                //
                                ClipChildRecordName = csData.getText("name");
                                string ClipParentFieldList = core.docProperties.getText(RequestNamePasteFieldList);
                                if (string.IsNullOrEmpty(ClipParentFieldList)) {
                                    //
                                    // Legacy paste - go right to the parent id
                                    //
                                    if (!csData.isFieldSupported("ParentID")) {
                                        ErrorController.addUserError(core, "The paste operation failed because the record you are pasting does not   support the necessary parenting feature.");
                                    } else {
                                        csData.set("ParentID", pasteParentRecordId);
                                    }
                                } else {
                                    //
                                    // Fill in the Field List name values
                                    //
                                    string[] Fields = ClipParentFieldList.Split(',');
                                    int FieldCount = Fields.GetUpperBound(0) + 1;
                                    for (var FieldPointer = 0; FieldPointer < FieldCount; FieldPointer++) {
                                        string Pair = Fields[FieldPointer];
                                        if (Pair.left(1) == "(" && Pair.Substring(Pair.Length - 1, 1) == ")") {
                                            Pair = Pair.Substring(1, Pair.Length - 2);
                                        }
                                        string[] NameValues = Pair.Split('=');
                                        if (NameValues.GetUpperBound(0) == 0) {
                                            ErrorController.addUserError(core, "The paste operation failed because the clipboard data Field List is not configured correctly.");
                                        } else {
                                            if (!csData.isFieldSupported(encodeText(NameValues[0]))) {
                                                ErrorController.addUserError(core, "The paste operation failed because the clipboard data Field [" + encodeText(NameValues[0]) + "] is not supported by the location data.");
                                            } else {
                                                csData.set(encodeText(NameValues[0]), encodeText(NameValues[1]));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //
                        // Set Child Pages Found and clear caches
                        //
                        using (var csData = new CsModel(core)) {
                            csData.openRecord(pasteParentContentMetadata.name, pasteParentRecordId, "ChildPagesFound");
                            if (csData.ok()) {
                                csData.set("ChildPagesFound", true);
                            }
                        }
                        //
                        // clear cache
                        DbBaseModel.invalidateCacheOfRecord<PageContentModel>(core.cpParent, pasteParentRecordId);
                        DbBaseModel.invalidateCacheOfRecord<PageContentModel>(core.cpParent, cutClipRecordId);
                    }
                }
            }
        }
        //
        //====================================================================================================
        #region  IDisposable Support 
        //
        // this class must implement System.IDisposable
        // never throw an exception in dispose
        // Do not change or add Overridable to these methods.
        // Put cleanup code in Dispose(ByVal disposing As Boolean).
        //====================================================================================================
        //
        protected bool disposed;
        //
        public void Dispose() {
            // do not add code  here. Use the Dispose(disposing) overload
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //
        ~PageContentController() {
            // do not add code here. Use the Dispose(disposing) overload
            Dispose(false);


        }
        //
        //====================================================================================================
        /// <summary>
        /// dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                this.disposed = true;
                if (disposing) {
                }
                //
                // cleanup non-managed objects
                //
            }
        }
        #endregion
    }
    //
}