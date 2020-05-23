
using System;

namespace Contensive.BaseClasses {
    //
    //====================================================================================================
    /// <summary>
    /// Application settings and methods
    /// </summary>
    public abstract class CPSiteBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// The application name
        /// </summary>
        public abstract string Name { get; }
        //
        //====================================================================================================
        /// <summary>
        /// remove the property
        /// </summary>
        /// <param name="key"></param>
        public abstract void ClearProperty(string key);
        //
        //====================================================================================================
        /// <summary>
        /// set a site-wide property. Read back with cp.site.GetText(), .getBoolean(), etc
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void SetProperty(string key, string value);
        //
        public abstract void SetProperty(string key, bool value);
        //
        public abstract void SetProperty(string key, DateTime value);
        //
        public abstract void SetProperty(string key, int value);
        //
        public abstract void SetProperty(string key, double value);
        //
        //====================================================================================================
        /// <summary>
        /// Read a site property as a string. If the key is not set, sets and returns the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public abstract string GetText(string key, string defaultValue);
        /// <summary>
        /// Read a site property as a string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract string GetText(string key);
        //
        //====================================================================================================
        /// <summary>
        /// Read a site property as a boolean. If the key is not set, sets and returns the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public abstract bool GetBoolean(string key, bool defaultValue);
        /// <summary>
        /// Read a site property as a boolean.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool GetBoolean(string key);
        //
        //====================================================================================================
        /// <summary>
        /// Read a site property as a date. If the key is not set, sets and returns the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public abstract DateTime GetDate(string key, DateTime defaultValue);
        /// <summary>
        /// Read a site property as a date.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract DateTime GetDate(string key);
        //
        //====================================================================================================
        /// <summary>
        /// Read a site property as an integer. If the key is not set, sets and returns the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public abstract int GetInteger(string key, int defaultValue);
        /// <summary>
        /// Read a site property as an integer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract int GetInteger(string key);
        //
        //====================================================================================================
        /// <summary>
        /// Read a site property as a double. If the key is not set, sets and returns the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract double GetNumber(string key);
        /// <summary>
        /// Read a site property as a double.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public abstract double GetNumber(string key, double defaultValue);
        //
        //====================================================================================================
        /// <summary>
        /// The primary domain name for the application. Used for email links, monitoring, etc.
        /// </summary>
        public abstract string DomainPrimary { get; }
        //
        //====================================================================================================
        /// <summary>
        /// For a webpage hit, this is the current domain used, otherwise it is the primary domain.
        /// </summary>
        public abstract string Domain { get; }
        //
        //====================================================================================================
        /// <summary>
        /// A complete list of all domains supported.
        /// </summary>
        public abstract string DomainList { get; }
        //
        //====================================================================================================
        /// <summary>
        /// For websites, the default script page.
        /// </summary>
        public abstract string PageDefault { get; }
        //
        //====================================================================================================
        /// <summary>
        /// Log a user activity to the activity log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="organizationId"></param>
        public abstract void LogActivity(string message, int userID, int organizationId);
        //
        //====================================================================================================
        /// <summary>
        /// Report an error, logging it and sending notifications as the app is confirgured. Does not rethrow the error.
        /// </summary>
        /// <param name="message"></param>
        public abstract void ErrorReport(string message);
        /// <summary>
        /// Report an error, logging it and sending notifications as the app is confirgured. Does not rethrow the error.
        /// </summary>
        /// <param name="Ex"></param>
        /// <param name="message"></param>
        public abstract void ErrorReport(System.Exception Ex);
        /// <summary>
        /// Report an error, logging it and sending notifications as the app is confirgured. Does not rethrow the error.
        /// </summary>
        /// <param name="Ex"></param>
        /// <param name="message"></param>
        public abstract void ErrorReport(System.Exception Ex, string message);
        //
        //====================================================================================================
        /// <summary>
        /// When debugging is true, add this message and timestamp to the debug trace.
        /// </summary>
        /// <param name="message"></param>
        public abstract void TestPoint(string message);
        //
        //====================================================================================================
        //
        public abstract void LogWarning(string name, string description, string typeOfWarningKey, string instanceKey);
        //
        //====================================================================================================
        //
        public abstract void LogAlarm(string cause);
        //
        //====================================================================================================
        //
        public abstract void AddLinkAlias(string linkAlias, int pageId, string queryStringSuffix);
        //
        public abstract void AddLinkAlias(string linkAlias, int pageId);
        //
        //====================================================================================================
        //
        public abstract string ThrowEvent(string eventNameIdOrGuid);
        //
        //====================================================================================================
        // deprecated
        //
        /// <summary>
        /// deprecated. Use CP.Http.CdnFilePathPrefix or CP.Http.CdnFilepathPrefixAbsolute
        /// </summary>
        [Obsolete("Use CP.Http.CdnFilePathPrefix or CP.Http.CdnFilepathPrefixAbsolute", false)]
        public abstract string FilePath { get; }
        //
        /// <summary>
        /// deprecated. Use CP.Addon.InstallCollectionFile()
        /// </summary>
        /// <param name="privatePathFilename"></param>
        /// <param name="returnUserError"></param>
        /// <returns></returns>
        [Obsolete("Use CP.Addon.InstallCollectionFile()", false)]
        public abstract bool installCollectionFile(string privatePathFilename, ref string returnUserError);
        //
        /// <summary>
        /// Use CP.Addon.InstallCollectionFromLibrary()
        /// </summary>
        /// <param name="collectionGuid"></param>
        /// <param name="returnUserError"></param>
        /// <returns></returns>
        [Obsolete("Use CP.Addon.InstallCollectionFromLibrary()", false)]
        public abstract bool installCollectionFromLibrary(string collectionGuid, ref string returnUserError);
        //
        //
        [Obsolete("Use correct defaultValue type",true)]
        public abstract bool GetBoolean(string key, string defaultValue);
        //
        //
        [Obsolete("Use correct defaultValue type", false)]
        public abstract DateTime GetDate(string key, string defaultValue);
        //
        //
        [Obsolete("Use correct defaultValue type", false)]
        public abstract int GetInteger(string key, string defaultValue);
        //
        //
        [Obsolete("Use correct defaultValue type", false)]
        public abstract double GetNumber(string key, string defaultValue);
        //
        //
        [Obsolete("Use GetText()", false)]
        public abstract string GetProperty(string key, string value);
        //
        //
        [Obsolete("Use GetText()", false)]
        public abstract string GetProperty(string key);
        //
        //
        [Obsolete("Deprecated", false)]
        public abstract bool MultiDomainMode { get; }
        //
        //
        [Obsolete("Deprecated, please use cp.cdnFiles, cp.privateFiles, cp.WwwFiles, or cp.TempFiles instead.", false)]
        public abstract string PhysicalFilePath { get; }
        //
        //
        [Obsolete("Deprecated, please use cp.cdnFiles, cp.privateFiles, cp.WwwFiles, or cp.TempFiles instead.", false)]
        public abstract string PhysicalInstallPath { get; }
        //
        //
        [Obsolete("Deprecated, please use cp.cdnFiles, cp.privateFiles, cp.WwwFiles, or cp.TempFiles instead.", false)]
        public abstract string PhysicalWWWPath { get; }
        //
        //
        // 20151121 - not needed, removed to resolve compile issue with com compatibility
        //Public MustOverride Sub ErrorReport(ByVal Err As Microsoft.VisualBasic.ErrObject, Optional ByVal Message As String = "")
        //
        //
        [Obsolete("Deprecated", false)]
        public abstract bool TrapErrors { get; }
        //
        //
        [Obsolete("Deprecated. This was the url path to the application for virtually hosted sites. Should be a blank.", false)]
        public abstract string AppPath { get; }
        //
        //
        [Obsolete("Deprecated. This was the url path to the application for virtually hosted sites. Should be a blank.", false)]
        public abstract string AppRootPath { get; }
        //
        //
        [Obsolete("Deprecated. This was a slash followed by the application name.", false)]
        public abstract string VirtualPath { get; }
        //
        //
        [Obsolete("Deprecated.", false)]
        public abstract bool IsTesting();
        //
        // removed because VB doesnt see upper/lowecase. 2 replacement methods cover both cases, and case will be corrected in C#
        //[Obsolete("Use uppercase method", false)]
        //public abstract void addLinkAlias(string linkAlias, int pageId, string queryStringSuffix = "");
        //
        [Obsolete("Use CP.Utils.ExportCsv()", false)]
        public abstract void RequestTask(string command, string SQL, string exportName, string filename);
        //
        //
        [Obsolete("Deprecated.", false)]
        public abstract int LandingPageId(string domainName);
        //
        //
        [Obsolete("Deprecated.", false)]
        public abstract int LandingPageId();
        //
        //
        [Obsolete("Use CP.Utils.EncodeAppRootPath()", false)]
        public abstract string EncodeAppRootPath(string link);
    }
}
