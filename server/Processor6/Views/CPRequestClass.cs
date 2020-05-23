
using System;
using System.Collections.Generic;
using Contensive.Models.Db;
using Contensive.Processor.Controllers;

namespace Contensive.Processor {
    //
    // comVisible to be activeScript compatible
    //
    public class CPRequestClass : BaseClasses.CPRequestBaseClass, IDisposable {
        /// <summary>
        /// dependencies
        /// </summary>
        private readonly CPClass cp;
        //
        //====================================================================================================
        // Constructor
        public CPRequestClass(CPClass cp) {
            this.cp = cp;
        }
        //
        //====================================================================================================
        public override string Browser {
            get {
                return cp.core.webServer.requestBrowser;
            }
        }
        //
        //====================================================================================================
        public override bool BrowserIsMobile {
            get {
                return cp.core.session.visit.mobile;
            }
        }
        //
        //====================================================================================================
        public override string Cookie(string CookieName) {
            return cp.core.webServer.getRequestCookie(CookieName);
        }
        //
        //====================================================================================================
        /// <summary>
        /// return a string that includes the simple name value pairs for all request cookies
        /// </summary>
        /// <returns></returns>
        public override string CookieString {
            get {
                string returnCookies = "";
                foreach (KeyValuePair<string, WebServerController.CookieClass> kvp in cp.core.webServer.requestCookies) {
                    returnCookies += "&" + kvp.Key + "=" + kvp.Value.value;
                }
                if (returnCookies.Length > 0) {
                    returnCookies = returnCookies.Substring(1);
                }
                return returnCookies;
            }
        }
        //
        //====================================================================================================
        public override string Form {
            get {
                return Controllers.GenericController.convertNameValueDictToREquestString(cp.core.webServer.requestForm);
            }
        }
        //
        //====================================================================================================
        public override string FormAction {
            get {
                return cp.core.webServer.serverFormActionURL;
            }
        }
        //
        //====================================================================================================
        public override bool GetBoolean(string RequestName) {
            return cp.core.docProperties.getBoolean(RequestName);
        }
        //
        //====================================================================================================
        public override DateTime GetDate(string RequestName) {
            return cp.core.docProperties.getDate(RequestName);
        }
        //
        //====================================================================================================
        public override int GetInteger(string RequestName) {
            return cp.core.docProperties.getInteger(RequestName);
        }
        //
        //====================================================================================================
        public override double GetNumber(string RequestName) {
            return cp.core.docProperties.getNumber(RequestName);
        }
        //
        //====================================================================================================
        public override string GetText(string RequestName) {
            return cp.core.docProperties.getText(RequestName);
        }
        //
        //====================================================================================================
        public override string Host {
            get {
                return cp.core.webServer.requestDomain;
            }
        }
        //
        //====================================================================================================
        public override string HTTPAccept {
            get {
                return (cp.core.webServer.serverEnvironment.ContainsKey("HTTP_ACCEPT")) ? cp.core.webServer.serverEnvironment["HTTP_ACCEPT"] : ""; 
            }
        }
        //
        //====================================================================================================
        public override string HTTPAcceptCharset {
            get {
                return (cp.core.webServer.serverEnvironment.ContainsKey("HTTP_ACCEPT_CHARSET")) ? cp.core.webServer.serverEnvironment["HTTP_ACCEPT_CHARSET"] : "";
            }
        }
        //
        //====================================================================================================
        public override string HTTPProfile {
            get {
                return (cp.core.webServer.serverEnvironment.ContainsKey("HTTP_PROFILE")) ? cp.core.webServer.serverEnvironment["HTTP_PROFILE"] : "";
            }
        }
        //
        //====================================================================================================
        public override string HTTPXWapProfile {
            get {
                return (cp.core.webServer.serverEnvironment.ContainsKey("HTTP_X_WAP_PROFILE")) ? cp.core.webServer.serverEnvironment["HTTP_X_WAP_PROFILE"] : "";
            }
        }
        //
        //====================================================================================================
        public override string Language {
            get {
                if (cp.core.session.userLanguage == null) {
                    return "";
                }
                LanguageModel userLanguage = DbBaseModel.create<LanguageModel>(cp, cp.core.session.user.languageId);
                if (userLanguage != null) {
                    return userLanguage.name;
                }
                return "English";
            }
        }
        //
        //====================================================================================================
        public override string Link {
            get {
                return cp.core.webServer.requestUrl;
            }
        }
        //
        //====================================================================================================
        public override string LinkForwardSource {
            get {
                return cp.core.webServer.linkForwardSource;
            }
        }
        //
        //====================================================================================================
        public override string LinkSource {
            get {
                return cp.core.webServer.requestUrlSource;
            }
        }
        //
        //====================================================================================================
        public override string Page {
            get {
                return cp.core.webServer.requestPage;
            }
        }
        //
        //====================================================================================================
        public override string Path {
            get {
                return cp.core.webServer.requestPath;
            }
        }
        //
        //====================================================================================================
        public override string PathPage {
            get {
                return cp.core.webServer.requestPathPage;
            }
        }
        //
        //====================================================================================================
        public override string Protocol {
            get {
                return cp.core.webServer.requestProtocol;
            }
        }
        //
        //====================================================================================================
        public override string QueryString {
            get {
                return cp.core.webServer.requestQueryString;
            }
        }
        //
        //====================================================================================================
        public override string Referer {
            get {
                return cp.core.webServer.requestReferer;
            }
        }
        //
        //====================================================================================================
        public override string RemoteIP {
            get {
                return cp.core.webServer.requestRemoteIP;
            }
        }
        //
        //====================================================================================================
        public override bool Secure {
            get {
                return cp.core.webServer.requestSecure;
            }
        }
        //
        //====================================================================================================
        public override string Body {
            get {
                return cp.core.webServer.requestBody;
            }
        }
        //
        //====================================================================================================
        public override string ContentType {
            get {
                return cp.core.webServer.requestContentType;
            }
        }
        //
        //====================================================================================================
        public override bool OK(string RequestName) {
            return cp.core.docProperties.containsKey(RequestName);
        }
        //
        // deprecated
        //====================================================================================================
        //
        [Obsolete]
        public override bool BrowserIsIE { get { return false; } }
        //
        [Obsolete]
        public override bool BrowserIsMac { get { return false; } }
        //
        [Obsolete]
        public override bool BrowserIsWindows {
            get {
                return false;
            }
        }
        //
        [Obsolete]
        public override string BrowserVersion {
            get {
                return "";
            }
        }
        //
        #region  IDisposable Support 
        //
        //====================================================================================================
        // dispose
        protected virtual void Dispose(bool disposing_req) {
            if (!this.disposed_req) {
                if (disposing_req) {
                    //
                    // call .dispose for managed objects
                    //
                }
                //
                // Add code here to release the unmanaged resource.
                //
            }
            this.disposed_req = true;
        }
        protected bool disposed_req;
        // Do not change or add Overridable to these methods.
        // Put cleanup code in Dispose(ByVal disposing As Boolean).
        public void Dispose()  {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CPRequestClass()  {
            Dispose(false);
            
            
        }
        #endregion
    }
}