
using System;

namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// static class controller
    /// </summary>
    public class _BlankController : IDisposable {
        //
        // ----- constants
        //
        //
        // ----- objects constructed that must be disposed
        //
        //
        // ----- private instance storage
        //
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
        public void Dispose()  {
            // do not add code here. Use the Dispose(disposing) overload
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //
        ~_BlankController()  {
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