﻿
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Constants;
using Contensive.Models.Db;

namespace Contensive.Processor {
    public class CPSiteClass : BaseClasses.CPSiteBaseClass, IDisposable {
        //
        private readonly CPClass cp;
        //
        //====================================================================================================
        //
        public CPSiteClass(CPClass cp) {
            this.cp = cp;
        }
        //
        //=======================================================================================================
        /// <summary>
        /// Clear a property
        /// </summary>
        /// <param name="key"></param>
        public override void ClearProperty(string key) {
            cp.core.siteProperties.clearProperty(key);
        }
        //
        //====================================================================================================
        //
        public override string Name {
            get {
                return cp.core.appConfig.name;
            }
        }
        //
        //====================================================================================================
        //
        public override void AddLinkAlias(string linkAlias, int pageId, string queryStringSuffix) => LinkAliasController.addLinkAlias(cp.core, linkAlias, pageId, queryStringSuffix);
        //
        public override void AddLinkAlias(string linkAlias, int pageId) => LinkAliasController.addLinkAlias(cp.core, linkAlias, pageId, "");
        //
        //====================================================================================================
        //
        public override void SetProperty(string key, string value) {
            cp.core.siteProperties.setProperty(key, value);
        }
        //
        //====================================================================================================
        //
        public override void SetProperty(string key, bool value) {
            cp.core.siteProperties.setProperty(key, value);
        }
        //
        //====================================================================================================
        //
        public override void SetProperty(string key, DateTime value) {
            cp.core.siteProperties.setProperty(key, value);
        }
        //
        //====================================================================================================
        //
        public override void SetProperty(string key, int value) {
            cp.core.siteProperties.setProperty(key, value);
        }
        //
        //====================================================================================================
        //
        public override void SetProperty(string key, double value) {
            cp.core.siteProperties.setProperty(key, value);
        }
        //
        //====================================================================================================
        //
        public override bool GetBoolean(string key, bool defaultValue) {
            return cp.core.siteProperties.getBoolean(key, defaultValue);
        }
        public override bool GetBoolean(string key) {
            return cp.core.siteProperties.getBoolean(key, default(bool));
        }
        //
        //====================================================================================================
        //
        public override DateTime GetDate(string key, DateTime DefaultValue) {
            return cp.core.siteProperties.getDate(key, DefaultValue);
        }
        public override DateTime GetDate(string key) {
            return cp.core.siteProperties.getDate(key, default(DateTime));
        }
        //
        //====================================================================================================
        //
        public override int GetInteger(string key, int DefaultValue) {
            return cp.core.siteProperties.getInteger(key, DefaultValue);
        }
        public override int GetInteger(string key) {
            return cp.core.siteProperties.getInteger(key, default(int));
        }
        //
        //====================================================================================================
        //
        public override double GetNumber(string key, double DefaultValue) {
            return cp.core.siteProperties.getNumber(key, DefaultValue);
        }
        public override double GetNumber(string key) {
            return cp.core.siteProperties.getNumber(key, default(double));
        }
        //
        //====================================================================================================
        //
        public override string GetText(string key, string DefaultValue) {
            return cp.core.siteProperties.getText(key, DefaultValue);
        }
        public override string GetText(string key) {
            return cp.core.siteProperties.getText(key, default(string));
        }
        //
        //====================================================================================================
        //
        public override string DomainPrimary {
            get {
                string tempDomainPrimary = null;
                tempDomainPrimary = "";
                if (cp.core.appConfig.domainList.Count > 0) {
                    tempDomainPrimary = cp.core.appConfig.domainList[0];
                }
                return tempDomainPrimary;
            }
        }
        //
        //====================================================================================================
        //
        public override string Domain {
            get {
                return cp.core.webServer.requestDomain;
            }
        }
        //
        //====================================================================================================
        //
        public override string DomainList {
            get {
                return string.Join(",", cp.core.appConfig.domainList);
            }
        }
        //
        //====================================================================================================
        //
        public override string PageDefault {
            get {
                return cp.core.siteProperties.serverPageDefault;
            }
        }
        //
        //====================================================================================================
        //
        public override void LogActivity(string message, int userID, int organizationId) {
            LogController.addSiteActivity(cp.core, message, 0, userID, organizationId);
        }
        //
        //====================================================================================================
        //
        public override void LogWarning(string name, string description, string typeOfWarningKey, string instanceKey) {
            LogController.addSiteWarning(cp.core, name, description, "", 0, description, typeOfWarningKey, instanceKey);
        }
        //
        //====================================================================================================
        //
        public override void LogAlarm(string cause) {
            LogController.logAlarm(cp.core, cause);
        }
        //
        //====================================================================================================
        //
        public override void ErrorReport(string message) {
            LogController.log(cp.core, message , BaseClasses.CPLogBaseClass.LogLevel.Error);
        }
        //
        public override void ErrorReport(System.Exception ex, string message) {
            LogController.log(cp.core, message + ", exception [" + ex + "]", BaseClasses.CPLogBaseClass.LogLevel.Error);
        }
        //
        public override void ErrorReport(System.Exception ex) {
            LogController.log(cp.core, "exception [" + ex + "]", BaseClasses.CPLogBaseClass.LogLevel.Error);
        }
        //
        //====================================================================================================
        //
        public override void TestPoint(string message) => LogController.logDebug(cp.core, message);
        //
        //====================================================================================================
        //
        public override string ThrowEvent(string eventNameIdOrGuid) => cp.core.addon.throwEvent(eventNameIdOrGuid);
        //
        //====================================================================================================
        // Deprecated
        //
        [Obsolete("Deprecated.", false)]
        public override bool MultiDomainMode { get { return false; } }
        //
        [Obsolete("Use GetText()", false)]
        public override string GetProperty(string propertyName, string DefaultValue) {
            return cp.core.siteProperties.getText(propertyName, DefaultValue);
        }
        //
        [Obsolete("Use GetText()", false)]
        public override string GetProperty(string propertyName) {
            return cp.core.siteProperties.getText(propertyName, "");
        }
        //
        [Obsolete("Use methods with matching types", false)]
        public override bool GetBoolean(string propertyName, string DefaultValue) {
            return cp.core.siteProperties.getBoolean(propertyName, GenericController.encodeBoolean(DefaultValue));
        }
        //
        [Obsolete("Use methods with matching types", false)]
        public override DateTime GetDate(string propertyName, string DefaultValue) {
            return cp.core.siteProperties.getDate(propertyName, GenericController.encodeDate(DefaultValue));
        }
        //
        [Obsolete("Use methods with matching types", false)]
        public override int GetInteger(string propertyName, string DefaultValue) {
            return cp.core.siteProperties.getInteger(propertyName, GenericController.encodeInteger(DefaultValue));
        }
        //
        [Obsolete("Deprecated.", false)]
        public override double GetNumber(string propertyName, string DefaultValue) {
            return cp.core.siteProperties.getNumber(propertyName, GenericController.encodeNumber(DefaultValue));
        }
        //
        [Obsolete("Deprecated, please use cp.cdnFiles, cp.privateFiles, cp.WwwFiles, or cp.TempFiles instead.", false)]
        public override string PhysicalFilePath {
            get {
                return cp.core.cdnFiles.localAbsRootPath;
            }
        }
        //
        [Obsolete("Deprecated, please use cp.cdnFiles, cp.privateFiles, cp.WwwFiles, or cp.TempFiles instead.", false)]
        public override string PhysicalInstallPath {
            get {
                return cp.core.privateFiles.localAbsRootPath;
            }
        }
        //
        [Obsolete("Deprecated, please use cp.cdnFiles, cp.privateFiles, cp.WwwFiles, or cp.TempFiles instead.", false)]
        public override string PhysicalWWWPath {
            get {
                return cp.core.wwwFiles.localAbsRootPath;
            }
        }
        //
        [Obsolete("Deprecated.", false)]
        public override bool TrapErrors {
            get {
                return GenericController.encodeBoolean(GetProperty("TrapErrors", "1"));
            }
        }
        //
        [Obsolete("Deprecated.", false)]
        public override string AppPath {
            get {
                return AppRootPath;
            }
        }
        //
        [Obsolete("Deprecated.", false)]
        public override string AppRootPath {
            get {
                return appRootPath;
            }
        }
        //
        [Obsolete("Deprecated.", false)]
        public override string VirtualPath {
            get {
                return "/" + cp.core.appConfig.name;
            }
        }
        //
        [Obsolete("Deprecated.", false)]
        public override bool IsTesting()  { return false; }
        //
        [Obsolete("Use GetInteger(LandingPageID)", false)]
        public override int LandingPageId()  {
            return GetInteger("LandingPageID", 0);
        }
        //
        [Obsolete("Use GetInteger(LandingPageID)", false)]
        public override int LandingPageId(string domainName) {
            if (string.IsNullOrWhiteSpace(domainName)) return GetInteger("LandingPageID", 0);
            var domain = DbBaseModel.createByUniqueName<DomainModel>(cp, domainName);
            if (domain == null)  return GetInteger("LandingPageID", 0);
            return domain.rootPageId;
        }
        //
        [Obsolete("Use CP.Utils.ExportCsv()", false)]
        public override void RequestTask(string command, string sql, string exportName, string filename) {
            cp.Utils.ExportCsv(sql, exportName, filename);
        }
        //
        [Obsolete("Use CP.Addon.InstallCollectionFile()", false)]
        public override bool installCollectionFile(string privatePathFilename, ref string returnUserError) 
            => cp.Addon.InstallCollectionFile(privatePathFilename, ref returnUserError);
        //
        [Obsolete("Use CP.Utils.InstallCollectionFromLibrary()", false)]
        public override bool installCollectionFromLibrary(string collectionGuid, ref string returnUserError)
            => cp.Addon.InstallCollectionFromLibrary(collectionGuid, ref returnUserError);
        //
        [Obsolete("Use CP.Utils.EncodeAppRootPath()", false)]
        public override string EncodeAppRootPath(string link) 
            => cp.Utils.EncodeAppRootPath(link);
        //
        //====================================================================================================
        //
        [Obsolete("Use CP.Http.CdnFilePathPrefix or CP.Http.CdnFilePathPrefixAbsolute", false)]
        public override string FilePath {
            get {
                return cp.core.appConfig.cdnFileUrl;
            }
        }
        //
        //
        #region  IDisposable Support 
        //
        //====================================================================================================
        //
        protected virtual void Dispose(bool disposing_site) {
            if (!this.disposed_site) {
                if (disposing_site) {
                    //
                    // call .dispose for managed objects
                    //
                }
                //
                // Add code here to release the unmanaged resource.
            }
            this.disposed_site = true;
        }
        protected bool disposed_site;
        // Do not change or add Overridable to these methods.
        // Put cleanup code in Dispose(ByVal disposing As Boolean).
        public void Dispose()  {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CPSiteClass()  {
            Dispose(false);
        }
        #endregion
    }
}