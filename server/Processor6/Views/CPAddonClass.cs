﻿
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using Contensive.Processor.Controllers;

namespace Contensive.Processor {
    //
    // todo implement or deprecate. might be nice to have this convenient api, but a model does the same, costs one query but will
    // always have the model at the save version as the addon code - this cp interface will match the database, but not the addon.
    // not sure which is better
    public class CPAddonClass : CPAddonBaseClass {
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
        public CPAddonClass(CPClass cp) => this.cp = cp;
        //
        //====================================================================================================
        /// <summary>
        /// The id of the addon currently executing
        /// </summary>
        public override int ID => cp.core.doc.addonModelStack.Peek().id;
        //
        //====================================================================================================
        /// <summary>
        /// The guid of the addon currently executing
        /// </summary>
        public override string ccGuid => cp.core.doc.addonModelStack.Peek().ccguid;
        //
        //====================================================================================================
        //
        public override string Execute(string addonGuid) {
            return cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, addonGuid), new BaseClasses.CPUtilsBaseClass.addonExecuteContext());
        }
        //
        //====================================================================================================
        //
        public override string Execute(string addonGuid, Dictionary<string, string> argumentKeyValuePairs) {
            return cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, addonGuid), new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                argumentKeyValuePairs = argumentKeyValuePairs
            });
        }
        //
        //====================================================================================================
        //
        public override string Execute(string addonGuid, CPUtilsBaseClass.addonExecuteContext executeContext) {
            return cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, addonGuid), executeContext);
        }
        //
        //====================================================================================================
        //
        public override string Execute(int addonId) {
            return cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, addonId), new BaseClasses.CPUtilsBaseClass.addonExecuteContext());
        }
        //
        //====================================================================================================
        //
        public override string Execute(int addonId, Dictionary<string, string> argumentKeyValuePairs) {
            return cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, addonId), new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                argumentKeyValuePairs = argumentKeyValuePairs
            });
        }
        //
        //====================================================================================================
        //
        public override string Execute(int addonId, CPUtilsBaseClass.addonExecuteContext executeContext) {
            return cp.core.addon.execute(DbBaseModel.create<AddonModel>(cp, addonId), executeContext);
        }
        //
        //====================================================================================================
        //
        public override string ExecuteByUniqueName(string addonName) {
            return cp.core.addon.execute(AddonModel.createByUniqueName(cp, addonName), new BaseClasses.CPUtilsBaseClass.addonExecuteContext());
        }
        //
        //====================================================================================================
        //
        public override string ExecuteByUniqueName(string addonName, Dictionary<string, string> argumentKeyValuePairs) {
            return cp.core.addon.execute(AddonModel.createByUniqueName(cp, addonName), new BaseClasses.CPUtilsBaseClass.addonExecuteContext {
                argumentKeyValuePairs = argumentKeyValuePairs
            });
        }
        //
        //====================================================================================================
        //
        public override string ExecuteByUniqueName(string addonName, CPUtilsBaseClass.addonExecuteContext executeContext) {
            return cp.core.addon.execute(AddonModel.createByUniqueName(cp, addonName), executeContext);
        }
        //
        //====================================================================================================
        /// <summary>
        /// execute an addon asyncronously. The session environment will include the same user, visit, doc. Include argument keyValuePairs available to the addon through cp.doc.get
        /// </summary>
        /// <param name="Addonid"></param>
        /// <param name="keyValuePairs"></param>
        public override void ExecuteAsync(int Addonid, Dictionary<string, string> keyValuePairs) {
            if (Addonid <= 0) { throw new ArgumentException("ExecuteAsync called with invalid AddonId [" + Addonid + "]"); }
            var addon = DbBaseModel.create<AddonModel>(cp, Addonid);
            if (addon == null) { throw new ArgumentException("ExecuteAsync cannot find AddonId [" + Addonid + "]"); }
            cp.core.addon.executeAsync(addon, keyValuePairs);
        }
        //
        //====================================================================================================
        //
        public override void ExecuteAsync(int Addonid) => ExecuteAsync(Addonid, new Dictionary<string, string>());
        //
        //====================================================================================================
        //
        public override void ExecuteAsync(string guid, Dictionary<string, string> keyValuePairs) {
            if (string.IsNullOrEmpty(guid)) { throw new ArgumentException("ExecuteAsync called with invalid guid [" + guid + "]"); }
            var addon = DbBaseModel.create<AddonModel>(cp, guid);
            if (addon == null) { throw new ArgumentException("ExecuteAsync cannot find Addon for guid [" + guid + "]"); }
            cp.core.addon.executeAsync(addon, keyValuePairs);
        }
        //
        //====================================================================================================
        //
        public override void ExecuteAsync(string guid) => ExecuteAsync(guid, new Dictionary<string, string>());
        //
        //====================================================================================================
        //
        public override void ExecuteAsyncByUniqueName(string name, Dictionary<string, string> keyValuePairs) {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException("ExecuteAsyncByUniqueName called with invalid name [" + name + "]"); }
            var addon = AddonModel.createByUniqueName(cp, name);
            if (addon == null) { throw new ArgumentException("ExecuteAsyncByUniqueName cannot find Addon for name [" + name + "]"); }
            cp.core.addon.executeAsync(addon, keyValuePairs);
        }
        //
        //====================================================================================================
        //
        public override void ExecuteAsyncByUniqueName(string name) => ExecuteAsyncByUniqueName(name, new Dictionary<string, string>());
        //
        //==========================================================================================
        /// <summary>
        /// Install an uploaded collection file from a private folder. Return true if successful, else the issue is in the returnUserError
        /// </summary>
        /// <param name="tempPathFilename"></param>
        /// <param name="returnUserError"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool InstallCollectionFile(string tempPathFilename, ref string returnUserError) {
            bool returnOk = false;
            try {
                bool deleteTempFileWhenDone = false;
                if (!cp.TempFiles.FileExists(tempPathFilename) && cp.PrivateFiles.FileExists(tempPathFilename)) {
                    //
                    // -- if caller uploaded to privateFiles, copy to tempFiles
                    cp.PrivateFiles.Copy(tempPathFilename, tempPathFilename, cp.TempFiles);
                    deleteTempFileWhenDone = true;
                }
                string ignoreReturnedCollectionGuid = "";
                var tmpList = new List<string> { };
                string logPrefix = "CPSiteClass.installCollectionFile";
                var installedCollections = new List<string>();
                var context = new Stack<string>();
                context.Push("Api call cp.addon.InstallCollectionFile [" + tempPathFilename + "]");
                returnOk = Controllers.CollectionInstallController.installCollectionFromTempFile(cp.core, false, context, tempPathFilename, ref returnUserError, ref ignoreReturnedCollectionGuid, false, true, ref tmpList, logPrefix, ref installedCollections);
                if (deleteTempFileWhenDone) { cp.TempFiles.DeleteFolder(tempPathFilename); }
            } catch (Exception ex) {
                Controllers.LogController.logError(cp.core, ex);
                if (!cp.core.siteProperties.trapErrors) {
                    throw;
                }
            }
            return returnOk;
        }
        //
        public override int InstallCollectionFileAsync(string tempPathFilename) {
            throw new NotImplementedException("InstallCollectionFileAsync, async methods are not yet implemented.");
        }
        //
        //====================================================================================================
        /// <summary>
        /// Install all addon collections in a folder asynchonously. Optionally delete the folder. The task is queued and the taskId is returned. Use cp.tasks.getTaskStatus to determine status
        /// </summary>
        /// <param name="tempPath"></param>
        /// <param name="deleteFolderWhenDone"></param>
        /// <returns></returns>
        public override bool InstallCollectionsFromFolder(string tempPath, bool deleteFolderWhenDone, ref string returnUserError) {
            bool deleteTempFolderWhenDone = false;
            if (!cp.TempFiles.FolderExists(tempPath) && cp.PrivateFiles.FolderExists(tempPath)) {
                //
                // -- if caller uploaded to privateFiles, copy to tempFiles
                cp.PrivateFiles.CopyPath(tempPath, tempPath, cp.TempFiles);
                deleteTempFolderWhenDone = true;
            }
            string ignoreUserMessage = "";
            List<string> ignoreList1 = new List<string>();
            List<string> ignoreList2 = new List<string>();
            string logPrefix = "CPUtilsClass.installCollectionsFromFolder";
            var collectionsInstalledList = new List<string>();
            var collectionsDownloaded = new List<string>();
            var context = new Stack<string>();
            context.Push("Api call cp.addon.InstallCollectionFromFolder [" + tempPath + "]");
            bool isDependency = false;
            bool result = CollectionInstallController.installCollectionsFromTempFolder(cp.core, isDependency, context, tempPath, ref ignoreUserMessage, ref collectionsInstalledList, false, false, ref ignoreList2, logPrefix, true, ref collectionsDownloaded);
            if (deleteTempFolderWhenDone) { 
                //
                // -- caller used private folder. delete the temp path created, but do not delete anything from private folder. They need to do it right.
                cp.TempFiles.DeleteFolder(tempPath); 
            } else {
                //
                // -- if they used the temp folder and asked folder to be deleted, delete.
                if (deleteFolderWhenDone) { cp.TempFiles.DeleteFolder(tempPath); }
            }
            return result;
        }
        //
        public override int InstallCollectionsFromFolderAsync(string tempFolder, bool deleteFolderWhenDone) {
            throw new NotImplementedException("InstallCollectionsFromFolderAsync, async methods are not yet implemented");
        }
        //
        //====================================================================================================
        /// <summary>
        /// Install an addon collections from the collection library asynchonously. The task is queued and the taskId is returned. Use cp.tasks.getTaskStatus to determine status
        /// </summary>
        public override bool InstallCollectionFromLibrary(string collectionGuid, ref string returnUserError) {
            string ignoreUserMessage = "";
            var installedCollections = new List<string>();
            string logPrefix = "installCollectionFromLibrary";
            var nonCriticalErrorList = new List<string>();
            var context = new Stack<string>();
            context.Push("Api call cp.addon.InstallCollectionFromLibrary [" + collectionGuid + "]");
            return CollectionLibraryController.installCollectionFromLibrary(cp.core, false, context, collectionGuid, ref ignoreUserMessage, false, false, ref nonCriticalErrorList, logPrefix, ref installedCollections);
        }
        //
        public override int InstallCollectionFromLibraryAsync(string collectionGuid) {
            throw new NotImplementedException("InstallCollectionFromLibraryAsync, async methods are not yet implemented");
        }
        //
        //====================================================================================================
        //
        public override bool InstallCollectionFromLink(string link, ref string returnUserError) {
            throw new NotImplementedException("InstallCollectionFromLink, methods are not yet implemented");
        }
        //
        //====================================================================================================
        //
        public override int InstallCollectionFromLinkAsync(string link) {
            throw new NotImplementedException("InstallCollectionFromLink, methods are not yet implemented");
        }
        //
        //====================================================================================================
        //
        public override bool ExportCollection(int collectionId, ref string collectionZipPathFilename, ref string returnUserError) {
            try {
                AddonCollectionModel collection = DbBaseModel.create<AddonCollectionModel>(cp, collectionId);
                collectionZipPathFilename = ExportController.createCollectionZip_returnCdnPathFilename(cp, collection);
                if (!cp.UserError.OK()) {
                    returnUserError = string.Join("", cp.UserError.GetList());
                }
                return (cp.UserError.OK());
            } catch (Exception) {
                throw;
            }
        }
        //
        //====================================================================================================
        //
        public override bool ExportCollection(string collectionGuid, ref string collectionZipPathFilename, ref string returnUserError) {
            try {
                AddonCollectionModel collection = DbBaseModel.create<AddonCollectionModel>(cp, collectionGuid);
                collectionZipPathFilename = ExportController.createCollectionZip_returnCdnPathFilename(cp, collection);
                if (!cp.UserError.OK()) {
                    returnUserError = string.Join("", cp.UserError.GetList());
                }
                return (cp.UserError.OK());
            } catch (Exception) {
                throw;
            }
        }
        //
        //====================================================================================================
        // Deprecated methods
        //
        [Obsolete("Deprecated", false)]
        public override bool Admin => cp.core.doc.addonModelStack.Peek().admin;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string ArgumentList => cp.core.doc.addonModelStack.Peek().argumentList;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool AsAjax => cp.core.doc.addonModelStack.Peek().admin;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string BlockDefaultStyles => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override int CollectionID => cp.core.doc.addonModelStack.Peek().collectionId;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool Content => cp.core.doc.addonModelStack.Peek().content;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string Copy => cp.core.doc.addonModelStack.Peek().copy;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string CopyText => cp.core.doc.addonModelStack.Peek().copyText;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string CustomStyles => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string DefaultStyles => cp.core.doc.addonModelStack.Peek().stylesFilename.content;
        //
        //====================================================================================================
        // 
        [Obsolete("Deprecated", false)]
        public override string Description => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string DotNetClass => cp.core.doc.addonModelStack.Peek().dotNetClass;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string FormXML => cp.core.doc.addonModelStack.Peek().formXML;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string Help => cp.core.doc.addonModelStack.Peek().help;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string HelpLink => cp.core.doc.addonModelStack.Peek().helpLink;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string IconFilename => cp.core.doc.addonModelStack.Peek().iconFilename;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override int IconHeight => GenericController.encodeInteger(cp.core.doc.addonModelStack.Peek().iconHeight);
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override int IconSprites => GenericController.encodeInteger(cp.core.doc.addonModelStack.Peek().iconSprites);
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override int IconWidth => GenericController.encodeInteger(cp.core.doc.addonModelStack.Peek().iconWidth);
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool InFrame => cp.core.doc.addonModelStack.Peek().inFrame;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool IsInline => cp.core.doc.addonModelStack.Peek().isInline;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string JavaScriptBodyEnd => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string JavascriptInHead => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string JavaScriptOnLoad => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string Link => cp.core.doc.addonModelStack.Peek().link;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string MetaDescription => cp.core.doc.addonModelStack.Peek().metaDescription;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string MetaKeywordList => cp.core.doc.addonModelStack.Peek().metaKeywordList;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string Name => cp.core.doc.addonModelStack.Peek().name;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string NavIconType => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string ObjectProgramID => cp.core.doc.addonModelStack.Peek().objectProgramId;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool OnBodyEnd => cp.core.doc.addonModelStack.Peek().onBodyEnd;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool OnBodyStart => cp.core.doc.addonModelStack.Peek().onBodyStart;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool OnContentEnd => cp.core.doc.addonModelStack.Peek().onPageEndEvent;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool OnContentStart => cp.core.doc.addonModelStack.Peek().onPageStartEvent;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool Open(int AddonId) => false;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool Open(string AddonNameOrGuid) => false;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string OtherHeadTags => cp.core.doc.addonModelStack.Peek().otherHeadTags;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string PageTitle => cp.core.doc.addonModelStack.Peek().pageTitle;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string ProcessInterval => cp.core.doc.addonModelStack.Peek().processInterval.ToString();
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override DateTime ProcessNextRun => GenericController.encodeDate(cp.core.doc.addonModelStack.Peek().processNextRun);
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool ProcessRunOnce => cp.core.doc.addonModelStack.Peek().processRunOnce;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string RemoteAssetLink => cp.core.doc.addonModelStack.Peek().remoteAssetLink;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool RemoteMethod => cp.core.doc.addonModelStack.Peek().remoteMethod;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string RobotsTxt => cp.core.doc.addonModelStack.Peek().robotsTxt;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string ScriptCode => cp.core.doc.addonModelStack.Peek().scriptingCode;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string ScriptEntryPoint => cp.core.doc.addonModelStack.Peek().scriptingEntryPoint;
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string ScriptLanguage => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override string SharedStyles => "";
        //
        //====================================================================================================
        //
        [Obsolete("Deprecated", false)]
        public override bool Template => cp.core.doc.addonModelStack.Peek().template;
    }
}