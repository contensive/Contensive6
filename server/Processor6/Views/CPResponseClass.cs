
using Contensive.Processor.Controllers;
using System;

namespace Contensive.Processor {
    //
    // comVisible to be activeScript compatible
    //
    public class CPResponseClass : BaseClasses.CPResponseBaseClass, IDisposable {
        //
        //====================================================================================================
        /// <summary>
        /// dependencies
        /// </summary>
        private readonly CPClass cp;
        //
        //====================================================================================================
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cp"></param>
        public CPResponseClass(CPClass cp) {
            this.cp = cp;
        }
        //
        //====================================================================================================
        //
        public override string ContentType {
            get {
                return cp.core.webServer.bufferContentType;
            }
            set {
                cp.core.webServer.setResponseContentType(value);
            }
        }
        //
        //====================================================================================================
        //
        public override string Cookies {
            get {
                return cp.core.webServer.bufferCookies;
            }
        }
        //
        //====================================================================================================
        //
        public override string Header {
            get {
                return cp.core.webServer.bufferResponseHeader;
            }
        }
        //
        //====================================================================================================
        //
        public override void Clear()  {
            cp.core.webServer.clearResponseBuffer();
        }
        //
        //====================================================================================================
        //
        public override void Close()  {
            cp.core.doc.continueProcessing = false;
        }
        //
        //====================================================================================================
        //
        public override void AddHeader(string name, string value) {
            cp.core.webServer.addResponseHeader(name, value);
        }
        //
        //====================================================================================================
        //
        public override void Flush()  {
            cp.core.webServer.flushStream();
        }
        //
        //====================================================================================================
        //
        public override void Redirect(string link) {
            //
            // -- determine the message by getting the call stack

            // Get call stack
            string callStack = "CP.Redirect call from " + GenericController.getCallStack();
            cp.core.webServer.redirect(link, callStack, false, false);
        }
        //
        //====================================================================================================
        //
        public override void SetBuffer(bool bufferOn) {
            cp.core.html.enableOutputBuffer(bufferOn);
        }
        //
        //====================================================================================================
        //
        /// <summary>
        ///
        /// </summary>
        /// <param name="status"></param>
        public override void SetStatus(string status) {
            cp.core.webServer.setResponseStatus(status);
        }
        //
        //====================================================================================================
        //
        public override void SetTimeout(string timeoutSeconds) {
        }
        //
        //====================================================================================================
        //
        public override void SetType(string contentType) {
            cp.core.webServer.setResponseContentType(contentType);
        }
        //
        //====================================================================================================
        //
        public override void SetCookie(string name, string value) {
            cp.core.webServer.addResponseCookie(name, value, DateTime.MinValue, "", "", false);
        }
        //
        //====================================================================================================
        //
        public override void SetCookie(string name, string value, DateTime dateExpires) {
            cp.core.webServer.addResponseCookie(name, value, dateExpires, "", "", false);
        }
        //
        //====================================================================================================
        //
        public override void SetCookie(string CookieName, string CookieValue, DateTime DateExpires, string Domain) {
            cp.core.webServer.addResponseCookie(CookieName, CookieValue, DateExpires, Domain, "", false);
        }
        //
        //====================================================================================================
        //
        public override void SetCookie(string CookieName, string CookieValue, DateTime DateExpires, string Domain, string Path) {
            cp.core.webServer.addResponseCookie(CookieName, CookieValue, DateExpires, Domain, Path, false);
        }
        //
        //====================================================================================================
        //
        public override bool isOpen {
            get {
                return cp.core.doc.continueProcessing;
            }
        }
        //
        //====================================================================================================
        //
        public override void SetCookie(string CookieName, string CookieValue, DateTime DateExpires, string Domain, string Path, bool Secure) {
            cp.core.webServer.addResponseCookie(CookieName, CookieValue, DateExpires, Domain, Path, Secure);
        }
        //
        //====================================================================================================
        // deprecated
        //
        [Obsolete("The write buffer is deprecated")]
        public override void Write(string message) {}

        #region  IDisposable Support 
        //
        // dispose
        //
        protected virtual void Dispose(bool disposing_res) {
            if (!this.disposed_res) {
                if (disposing_res) {
                    //
                    // call .dispose for managed objects
                    //
                }
                //
                // Add code here to release the unmanaged resource.
                //
            }
            this.disposed_res = true;
        }
        protected bool disposed_res;
        // Do not change or add Overridable to these methods.
        // Put cleanup code in Dispose(ByVal disposing As Boolean).
        public void Dispose()  {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CPResponseClass()  {
            Dispose(false);
            
            
        }
        #endregion
    }
}