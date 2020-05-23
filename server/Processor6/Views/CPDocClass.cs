
using System;
using System.Collections.Generic;
using Contensive.Processor.Controllers;
using Contensive.Processor.Models.Domain;

namespace Contensive.Processor {
    public class CPDocClass : BaseClasses.CPDocBaseClass, IDisposable {
        /// <summary>
        /// dependencies
        /// </summary>
        private readonly CPClass cp;
        //
        //====================================================================================================
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cpParent"></param>
        public CPDocClass(CPClass cpParent) {
            cp = cpParent;
        }
        //
        public override List<HtmlAssetClass> HtmlAssetList {

            get{
                return cp.core.doc.htmlAssetList;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Returns to the current value of NoFollow, set by addon execution
        /// </summary>
        /// <returns></returns>
        public override bool NoFollow {
            get {
                return cp.core.webServer.response_NoFollow;
            }
            set {
                cp.core.webServer.response_NoFollow = value;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the pageId
        /// </summary>
        /// <returns></returns>
        public override int PageId {
            get {
                if (cp.core.doc == null) { return 0; }
                if (cp.core.doc.pageController == null) { return 0; }
                if (cp.core.doc.pageController.page == null) { return 0; }
                return cp.core.doc.pageController.page.id;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the page name, set by the pagemenager addon
        /// </summary>
        /// <returns></returns>
        public override string PageName {
            get {
                if (cp.core.doc == null) { return string.Empty; }
                if (cp.core.doc.pageController == null) { return string.Empty; }
                if (cp.core.doc.pageController.page == null) {return string.Empty; }
                return cp.core.doc.pageController.page.name;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the current value of refreshquerystring 
        /// </summary>
        /// <returns></returns>
        public override string RefreshQueryString {
            get {
                if (cp.core.doc == null) { return String.Empty; }
                return cp.core.doc.refreshQueryString;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// the time and date when this document was started 
        /// </summary>
        /// <returns></returns>
        public override DateTime StartTime {
            get {
                if (cp.core.doc == null) { return default( DateTime ); }
                return cp.core.doc.profileStartTime;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the id of the template, as set by the page manager
        /// </summary>
        /// <returns></returns>
        public override int TemplateId {
            get {
                if (cp.core.doc == null) { return 0; }
                if (cp.core.doc.pageController == null) { return 0; }
                if (cp.core.doc.pageController.template == null) { return 0; }
                return cp.core.doc.pageController.template.id;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the docType, set by the page manager settings 
        /// </summary>
        /// <returns></returns>
        public override string Type {
            get {
                if (cp.core.siteProperties == null) { return Constants.DTDDefault; }
                return cp.core.siteProperties.docTypeDeclaration;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// adds javascript code to the head of the document
        /// </summary>
        /// <param name="code"></param>
        public override void AddHeadJavascript(string code) {
            cp.core.html.addScriptCode(code, "api", true);
        }
        //
        //====================================================================================================
        /// <summary>
        /// adds a link to javascript code to the head of the document
        /// </summary>
        /// <param name="code"></param>
        public override void AddHeadJavascriptLink(string codeLink) {
            cp.core.html.addScriptLinkSrc( codeLink, "api", true);
        }
        //
        //====================================================================================================
        /// <summary>
        /// adds javascript code to the head of the document
        /// </summary>
        /// <param name="code"></param>
        public override void AddBodyJavascript(string code) {
            cp.core.html.addScriptCode(code, "api", false);
        }
        //
        //====================================================================================================
        /// <summary>
        /// adds a link to javascript code to the head of the document
        /// </summary>
        /// <param name="code"></param>
        public override void AddBodyJavascriptLink(string codeLink) {
            cp.core.html.addScriptLinkSrc(codeLink, "api", false);
        }
        //
        //====================================================================================================
        /// <summary>
        /// adds a javascript tag to the head of the document
        /// </summary>
        /// <param name="htmlTag"></param>
        public override void AddHeadTag(string htmlTag) {
            cp.core.html.addHeadTag(htmlTag);
        }
        //
        //====================================================================================================
        //
        public override void AddMetaDescription(string metaDescription) {
            cp.core.html.addMetaDescription(metaDescription);
        }
        //
        //====================================================================================================
        //
        public override void AddMetaKeywordList(string metaKeywordList) {
            cp.core.html.addMetaKeywordList(metaKeywordList);
        }
        //
        //====================================================================================================
        //
        public override void AddOnLoadJavascript(string code) {
            cp.core.html.addScriptCode_onLoad(code, "");
        }
        //
        //====================================================================================================
        //
        public override void AddTitle(string pageTitle) {
            cp.core.html.addTitle(pageTitle);
        }
        //
        //====================================================================================================
        //
        public override void AddRefreshQueryString(string key, string Value) => cp.core.doc.addRefreshQueryString(key, Value);
        public override void AddRefreshQueryString(string key, int Value) => cp.core.doc.addRefreshQueryString(key, GenericController.encodeText(Value));
        public override void AddRefreshQueryString(string key, double Value) => cp.core.doc.addRefreshQueryString(key, GenericController.encodeText(Value));
        public override void AddRefreshQueryString(string key, bool Value) => cp.core.doc.addRefreshQueryString(key, GenericController.encodeText(Value));
        public override void AddRefreshQueryString(string key, DateTime Value) => cp.core.doc.addRefreshQueryString(key, GenericController.encodeText(Value));
        //
        //====================================================================================================
        //
        public override void AddHeadStyle(string styleSheet) {
            cp.core.html.addHeadTag(HtmlController.style(styleSheet));
        }
        //
        //====================================================================================================
        //
        public override void AddHeadStyleLink(string styleSheetLink) {
            cp.core.html.addStyleLink(styleSheetLink, "");
        }
        //
        //====================================================================================================
        //
        public override void AddBodyEnd(string html) {
            cp.core.doc.htmlForEndOfBody += html;
        }
        //
        //====================================================================================================
        //
        public override string Body {
            get {
                return cp.core.doc.body;
            }
            set {
                cp.core.doc.body = value;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Decodes an argument parsed from an AddonOptionString for all non-allowed characters
        /// </summary>
        /// <param name="encodedArgument"></param>
        /// <returns></returns>
        public string decodeLegacyOptionStringArgument(string encodedArgument) {
            return GenericController.decodeNvaArgument(encodedArgument);
        }
        //
        //=======================================================================================================
        //
        public override string GetProperty(string key, string defaultValue) {
            if (cp.core.docProperties.containsKey(key)) { return cp.core.docProperties.getText(key); }
            return defaultValue;
        }
        public override string GetProperty(string key) => GetProperty(key, string.Empty);
        //
        //=======================================================================================================
        //
        public override bool GetBoolean(string key, bool defaultValue) {
            return GenericController.encodeBoolean(GetProperty(key, GenericController.encodeText( defaultValue)));
        }
        public override bool GetBoolean(string key) => GetBoolean(key, false);
        //
        //=======================================================================================================
        //
        public override DateTime GetDate(string key, DateTime defaultValue) {
            return GenericController.encodeDate(GetProperty(key, GenericController.encodeText( defaultValue)));
        }
        public override DateTime GetDate(string key) => GetDate(key, DateTime.MinValue);
        //
        //=======================================================================================================
        //
        public override int GetInteger(string key, int defaultValue) {
            return cp.Utils.EncodeInteger(GetProperty(key, GenericController.encodeText( defaultValue )));
        }
        public override int GetInteger(string key) => GetInteger(key, 0);
        //
        //=======================================================================================================
        //
        public override double GetNumber(string key, double defaultValue) {
            return cp.Utils.EncodeNumber(GetProperty(key, GenericController.encodeText( defaultValue )));
        }
        public override double GetNumber(string key) => GetNumber(key, 0);
        //
        //=======================================================================================================
        //
        public override string GetText(string key, string defaultValue) {
            return GetProperty(key, defaultValue);
        }
        //
        public override string GetText(string key) {
            return GetProperty(key, string.Empty);
        }
        //
        //=======================================================================================================
        //
        public override bool IsProperty(string key) {
            return cp.core.docProperties.containsKey(key);
        }
        //
        //=======================================================================================================
        //
        public override void SetProperty(string key, string value) {
            cp.core.docProperties.setProperty(key, value, DocPropertyModel.DocPropertyTypesEnum.userDefined);
        }
        //
        //=======================================================================================================
        //
        public override void SetProperty(string key, bool value) {
            cp.core.docProperties.setProperty(key, value, DocPropertyModel.DocPropertyTypesEnum.userDefined);
        }
        //
        //=======================================================================================================
        //
        public override void SetProperty(string key, int value) {
            cp.core.docProperties.setProperty(key, value, DocPropertyModel.DocPropertyTypesEnum.userDefined);
        }
        //
        //=======================================================================================================
        //
        public override void SetProperty(string key, DateTime value) {
            cp.core.docProperties.setProperty(key, value, DocPropertyModel.DocPropertyTypesEnum.userDefined);
        }
        //
        //=======================================================================================================
        //
        public override void SetProperty(string key, double value) {
            cp.core.docProperties.setProperty(key, value, DocPropertyModel.DocPropertyTypesEnum.userDefined);
        }
        //
        //=======================================================================================================
        //
        public override bool IsAdminSite {
            get {
                return !cp.Request.PathPage.IndexOf(cp.Site.GetText("adminUrl"), System.StringComparison.OrdinalIgnoreCase).Equals(-1);
            }
        }
        //
        //=======================================================================================================
        // Deprecated
        //
        [Obsolete("Filter addons are deprecated", false)]
        public override string Content {
            get {
                return cp.core.doc.bodyContent;
            }
            set {
                cp.core.doc.bodyContent = value;
            }
        }
        //
        [Obsolete("Use addon navigation.", false)]
        public override string NavigationStructure {
            get {
                return string.Empty;
            }
        }
        //
        [Obsolete("Section is no longer supported", false)]
        public override int SectionId {
            get {
                return 0;
            }
        }
        //
        [Obsolete("Site styles are no longer supported. Include styles and javascript in addons.", false)]
        public override string SiteStylesheet {
            get {
                return "";
            }
        }
        //
        [Obsolete("var is deprecated.", false)]
        public override string get_GlobalVar(string Index) {
            return get_Var(Index);
        }
        //
        [Obsolete("var is deprecated.", false)]
        public override bool get_IsGlobalVar(string Index) {
            return get_IsVar(Index);
        }
        //
        [Obsolete("var is deprecated.", false)]
        public override bool get_IsVar(string Index) {
            return cp.core.docProperties.containsKey(Index);
        }
        //
        [Obsolete("var is deprecated.", false)]
        public override string get_Var(string Index) {
            return cp.core.docProperties.getText(Index);
        }
        //
        [Obsolete("var is deprecated.", false)]
        public override void set_Var(string Index, string Value) {
            cp.core.docProperties.setProperty(Index, Value);
        }
        //
        [Obsolete("var is deprecated.", false)]
        public override void set_GlobalVar(string Index, string Value) {
            cp.core.docProperties.setProperty(Index, Value);
        }
        //
        [Obsolete("Use GetBoolean(string,bool).", false)]
        public override bool GetBoolean(string key, string defaultValue) => GetBoolean(key, GenericController.encodeBoolean(defaultValue));
        //
        [Obsolete("Use GetDate(string,DateTime).", false)]
        public override DateTime GetDate(string key, string defaultValue) => GetDate(key, GenericController.encodeDate(defaultValue));
        //
        [Obsolete("Use GetInteger(string,int).", false)]
        public override int GetInteger(string key, string defaultValue) => GetInteger(key, GenericController.encodeInteger(defaultValue));
        //
        [Obsolete("Use GetNumber(string,double).", false)]
        public override double GetNumber(string key, string defaultValue) => GetNumber(key, GenericController.encodeNumber(defaultValue));
        //
        //=======================================================================================================
        // IDisposable support
        //
        #region  IDisposable Support 
        // Do not change or add Overridable to these methods.
        // Put cleanup code in Dispose(ByVal disposing As Boolean).
        public void Dispose()  {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CPDocClass()  {
            Dispose(false);
        }
        protected bool disposed_doc;
        //
        //====================================================================================================
        /// <summary>
        /// destructor
        /// </summary>
        /// <param name="disposing_doc"></param>
        protected virtual void Dispose(bool disposing_doc) {
            if (!this.disposed_doc) {
                if (disposing_doc) {
                    //
                    // call .dispose for managed objects
                    //
                }
                //
                // Add code here to release the unmanaged resource.
                //
            }
            this.disposed_doc = true;
        }
        #endregion
    }
}