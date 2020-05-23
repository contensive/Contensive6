
using System;
using Contensive.BaseClasses;
using Contensive.Processor.Controllers;

namespace Contensive.Processor {
    //
    // todo implement or deprecate. might be nice to have this convenient api, but a model does the same, costs one query but will
    // always have the model at the save version as the addon code - this cp interface will match the database, but not the addon.
    // not sure which is better
    public class CPLogClass : CPLogBaseClass, IDisposable {
        //
        // ====================================================================================================
        /// <summary>
        /// dependencies
        /// </summary>
        private readonly CPClass cp;
        //
        // ====================================================================================================
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cp"></param>
        public CPLogClass(CPClass cp) 
            => this.cp = cp;
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the trace level (trace, debug, info, warn, error, fatal)
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Trace(string logMessage) {
            LogController.log(cp.core, logMessage, LogLevel.Trace);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the debug level (trace, debug, info, warn, error, fatal)
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Debug(string logMessage) {
            LogController.log(cp.core, logMessage, LogLevel.Debug);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the info level (trace, debug, info, warn, error, fatal)
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Info(string logMessage) {
            LogController.log(cp.core, logMessage, LogLevel.Info);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the warn level (trace, debug, info, warn, error, fatal)
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Warn(string logMessage) {
            LogController.log(cp.core, logMessage, LogLevel.Warn);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the error level (trace, debug, info, warn, error, fatal)
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Error(string logMessage) {
            LogController.log(cp.core, logMessage, LogLevel.Error);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the fatal level (trace, debug, info, warn, error, fatal)
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Fatal(string logMessage) {
            LogController.log(cp.core, logMessage, LogLevel.Fatal);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message at the debug level
        /// </summary>
        /// <param name="logMessage"></param>
        public override void Add(string logMessage) {
            LogController.logDebug(cp.core, logMessage);
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a log message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="logMessage"></param>
        public override void Add(LogLevel level, string logMessage) {
            LogController.log(cp.core, logMessage, level);
        }
        //
        #region  IDisposable Support 
        // Do not change or add Overridable to these methods.
        // Put cleanup code in Dispose(ByVal disposing As Boolean).
        //
        // ====================================================================================================
        /// <summary>
        /// must call to dispose
        /// </summary>
        protected virtual void Dispose(bool disposing_log) {
            if (!this.disposed_log) {
                if (disposing_log) {
                    //
                    // call .dispose for managed objects
                    //
                }
                //
                // Add code here to release the unmanaged resource.
                //
            }
            this.disposed_log = true;
        }
        //
        protected bool disposed_log;
        public override void Dispose()  {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //
        ~CPLogClass()  {
            Dispose(false);
        }
        #endregion
    }
}