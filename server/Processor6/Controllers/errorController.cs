
using System;
using static Contensive.Processor.Constants;

namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// static class controller
    /// </summary>
    public class ErrorController : IDisposable {
        //
        //====================================================================================================
        //
        public static void addUserError(CoreController core, string Message) {
            if(!core.doc.userErrorList.Contains(Message)) { core.doc.userErrorList.Add(Message); }
        }
        //
        //====================================================================================================
        //
        public static string getUserError(CoreController core) {
            if ( core.doc.userErrorList.Count.Equals(0)) { return string.Empty; }
            string result = "";
            foreach ( var userError in core.doc.userErrorList ) {
                result += "\r<li class=\"ccExceptionListRow\">" + GenericController.encodeText(userError) + "</LI>";
            }
            core.doc.userErrorList.Clear();
            return "<ul class=\"ccExceptionList\">" + result + "\r</ul>";
        }
        //
        //====================================================================================================
        /// <summary>
        /// return an html ul list of each eception produced during this document.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string getDocExceptionHtmlList(CoreController core) {
            string returnHtmlList = "";
            try {
                if (core.doc.errorList != null) {
                    if (core.doc.errorList.Count > 0) {
                        foreach (string exMsg in core.doc.errorList) {
                            returnHtmlList += cr2 + "<li class=\"ccExceptionListRow\">" + cr3 + HtmlController.convertTextToHtml(exMsg) + cr2 + "</li>";
                        }
                        returnHtmlList = "\r<ul class=\"ccExceptionList\">" + returnHtmlList + "\r</ul>";
                    }
                }
            } catch (Exception ex) {
                throw (ex);
            }
            return returnHtmlList;
        }
        //
        //====================================================================================================
        //
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
        public void Dispose()  {
            // do not add code here. Use the Dispose(disposing) overload
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //
        ~ErrorController()  {
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