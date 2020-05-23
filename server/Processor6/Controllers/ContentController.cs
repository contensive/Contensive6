﻿
using Contensive.BaseClasses;
using Contensive.Models.Db;
using Contensive.Processor.Models.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// static class controller
    /// </summary>
    public class ContentController : IDisposable {
        //
        public static string pluralToSingular( string pluralContentName ) {
            if (pluralContentName.Equals("quizzes",StringComparison.InvariantCultureIgnoreCase)) { return pluralContentName.Substring(0, 4); }
            var pluralization = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            return pluralization.Singularize(pluralContentName);
        }
        //
        //====================================================================================================
        //
        public static void processAfterSave_AddonCollection(CoreController core, bool isDelete, string contentName, int recordID, string recordName, int recordParentID, bool useContentWatchLink) {
            //
            // -- if this is an add or delete, manage the collection folders
            if (isDelete) {
                //
                // todo - if a collection is deleted, consider deleting the collection folder (or saving as archive)
            } else {
                //
                // -- add or modify collection, verify collection /addon folder
                var addonCollection = AddonCollectionModel.create<AddonCollectionModel>(core.cpParent, recordID);
                if (addonCollection != null) {
                    string CollectionVersionFolderName = CollectionFolderController.verifyCollectionVersionFolderName(core, addonCollection.ccguid, addonCollection.name);
                    if (string.IsNullOrEmpty(CollectionVersionFolderName)) {
                        //
                        // -- new collection
                        string CollectionVersionFolder =  AddonController.getPrivateFilesAddonPath() + CollectionVersionFolderName;
                        core.privateFiles.createPath(CollectionVersionFolder);
                        CollectionFolderController.updateCollectionFolderConfig(core, addonCollection.name, addonCollection.ccguid, core.dateTimeNowMockable, CollectionVersionFolderName);
                    }
                }
            }
        }
        //
        //====================================================================================================
        //
        public static void processAfterSave_LibraryFiles(CoreController core, bool isDelete, string contentName, int recordID, string recordName, int recordParentID, bool useContentWatchLink) {
            //
            // if a AltSizeList is blank, make large,medium,small and thumbnails
            //
            if (core.siteProperties.getBoolean("ImageAllowSFResize", true) && (!isDelete)) {
                using (var csData = new CsModel(core)) {
                    if (csData.openRecord("library files", recordID)) {
                        string Filename = csData.getText("filename");
                        int Pos = Filename.LastIndexOf("/") + 1;
                        string FilePath = "";
                        if (Pos > 0) {
                            FilePath = Filename.left(Pos);
                            Filename = Filename.Substring(Pos);
                        }
                        csData.set("filesize", core.wwwFiles.getFileSize(FilePath + Filename));
                        Pos = Filename.LastIndexOf(".") + 1;
                        if (Pos > 0) {
                            string FilenameExt = Filename.Substring(Pos);
                            string FilenameNoExt = Filename.left(Pos - 1);
                            if (GenericController.strInstr(1, "jpg,gif,png", FilenameExt, 1) != 0) {
                                ImageEditController sf = new ImageEditController();
                                if (sf.load(FilePath + Filename, core.wwwFiles)) {
                                    //
                                    //
                                    //
                                    csData.set("height", sf.height);
                                    csData.set("width", sf.width);
                                    string AltSizeList = csData.getText("AltSizeList");
                                    bool RebuildSizes = (string.IsNullOrEmpty(AltSizeList));
                                    if (RebuildSizes) {
                                        AltSizeList = "";
                                        //
                                        // Attempt to make 640x
                                        //
                                        if (sf.width >= 640) {
                                            sf.height = GenericController.encodeInteger(sf.height * (640 / sf.width));
                                            sf.width = 640;
                                            sf.save(FilePath + FilenameNoExt + "-640x" + sf.height + "." + FilenameExt, core.wwwFiles);
                                            AltSizeList = AltSizeList + Environment.NewLine + "640x" + sf.height;
                                        }
                                        //
                                        // Attempt to make 320x
                                        //
                                        if (sf.width >= 320) {
                                            sf.height = GenericController.encodeInteger(sf.height * (320 / sf.width));
                                            sf.width = 320;
                                            sf.save(FilePath + FilenameNoExt + "-320x" + sf.height + "." + FilenameExt, core.wwwFiles);

                                            AltSizeList = AltSizeList + Environment.NewLine + "320x" + sf.height;
                                        }
                                        //
                                        // Attempt to make 160x
                                        //
                                        if (sf.width >= 160) {
                                            sf.height = GenericController.encodeInteger(sf.height * (160 / sf.width));
                                            sf.width = 160;
                                            sf.save(FilePath + FilenameNoExt + "-160x" + sf.height + "." + FilenameExt, core.wwwFiles);
                                            AltSizeList = AltSizeList + Environment.NewLine + "160x" + sf.height;
                                        }
                                        //
                                        // Attempt to make 80x
                                        //
                                        if (sf.width >= 80) {
                                            sf.height = GenericController.encodeInteger(sf.height * (80 / sf.width));
                                            sf.width = 80;
                                            sf.save(FilePath + FilenameNoExt + "-180x" + sf.height + "." + FilenameExt, core.wwwFiles);
                                            AltSizeList = AltSizeList + Environment.NewLine + "80x" + sf.height;
                                        }
                                        csData.set("AltSizeList", AltSizeList);
                                    }
                                    sf.Dispose();
                                    sf = null;
                                }
                            }
                        }
                    }
                }
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Process manual changes needed for special cases
        /// </summary>
        /// <param name="isDelete"></param>
        /// <param name="contentName"></param>
        /// <param name="recordId"></param>
        /// <param name="recordName"></param>
        /// <param name="recordParentID"></param>
        /// <param name="useContentWatchLink"></param>
        public static void processAfterSave(CoreController core, bool isDelete, string contentName, int recordId, string recordName, int recordParentID, bool useContentWatchLink) {
            try {
                PageContentModel.markReviewed(core.cpParent, recordId);
                string tableName = MetadataController.getContentTablename(core, contentName);
                //
                // -- invalidate the specific cache for this record
                core.cache.invalidateDbRecord(recordId, tableName);
                //
                string tableNameLower = tableName.ToLower(CultureInfo.InvariantCulture);
                if (tableNameLower == AddonCollectionModel.tableMetadata.tableNameLower) {
                    //
                    // -- addon collection
                    processAfterSave_AddonCollection(core, isDelete, contentName, recordId, recordName, recordParentID, useContentWatchLink);
                } else if (tableNameLower == LinkForwardModel.tableMetadata.tableNameLower) {
                    //
                    // -- link forward
                    core.routeMapCacheClear();
                } else if (tableNameLower == LinkAliasModel.tableMetadata.tableNameLower) {
                    //
                    // -- link alias
                    core.routeMapCacheClear();
                } else if (tableNameLower == AddonModel.tableMetadata.tableNameLower) {
                    //
                    // -- addon
                    core.routeMapCacheClear();
                } else if (tableNameLower == PersonModel.tableMetadata.tableNameLower) {
                    //
                    // -- PersonModel
                    var person = PersonModel.create<PersonModel>(core.cpParent, recordId);
                    if(person != null ) {
                        if (isDelete) {
                            LogController.addSiteActivity(core, "deleting user #" + recordId + " (" + recordName + ")", recordId, person.organizationId);
                        } else {
                            LogController.addSiteActivity(core, "saving changes to user #" + recordId + " (" + recordName + ")", recordId, person.organizationId);
                        }
                    }
                } else if (tableNameLower == OrganizationModel.tableMetadata.tableNameLower) {
                    //
                    // -- Log Activity for changes to people and organizattions
                    if (isDelete) {
                        LogController.addSiteActivity(core, "deleting organization #" + recordId + " (" + recordName + ")", 0, recordId);
                    } else {
                        LogController.addSiteActivity(core, "saving changes to organization #" + recordId + " (" + recordName + ")", 0, recordId);
                    }
                } else if (tableNameLower == SitePropertyModel.tableMetadata.tableNameLower) {
                    //
                    // -- Site Properties
                    switch (GenericController.toLCase(recordName)) {
                        case "allowlinkalias":
                            PageContentModel.invalidateCacheOfTable<PageContentModel>(core.cpParent);
                            break;
                        case "sectionlandinglink":
                            PageContentModel.invalidateCacheOfTable<PageContentModel>(core.cpParent);
                            break;
                        case Constants._siteproperty_serverPageDefault_name:
                            PageContentModel.invalidateCacheOfTable<PageContentModel>(core.cpParent);
                            break;
                    }
                } else if (tableNameLower == PageContentModel.tableMetadata.tableNameLower) {
                    //
                    // -- set ChildPagesFound true for parent page
                    if (recordParentID > 0) {
                        if (!isDelete) {
                            core.db.executeNonQuery("update ccpagecontent set ChildPagesfound=1 where ID=" + recordParentID);
                        }
                    }
                    if (isDelete) {
                        //
                        // Clear the Landing page and page not found site properties
                        if (recordId == GenericController.encodeInteger(core.siteProperties.getText("PageNotFoundPageID", "0"))) {
                            core.siteProperties.setProperty("PageNotFoundPageID", "0");
                        }
                        if (recordId == core.siteProperties.landingPageID) {
                            core.siteProperties.setProperty("landingPageId", "0");
                        }
                        //
                        // Delete Link Alias entries with this PageID
                        core.db.executeNonQuery("delete from cclinkAliases where PageID=" + recordId);
                    }
                    DbBaseModel.invalidateCacheOfRecord<PageContentModel>(core.cpParent, recordId);
                } else if (tableNameLower == LibraryFilesModel.tableMetadata.tableNameLower) {
                    //
                    // -- 
                    processAfterSave_LibraryFiles(core, isDelete, contentName, recordId, recordName, recordParentID, useContentWatchLink);
                }
                //
                // Process Addons marked to trigger a process call on content change
                //
                Dictionary<string, string> instanceArguments;
                bool onChangeAddonsAsync = core.siteProperties.getBoolean("execute oncontentchange addons async", false);
                using (var csData = new CsModel(core)) {
                    int contentId = ContentMetadataModel.getContentId(core, contentName);
                    csData.open("Add-on Content Trigger Rules", "ContentID=" + contentId, "", false, 0, "addonid");
                    string Option_String = null;
                    if (isDelete) {
                        instanceArguments = new Dictionary<string, string> {
                            {"action","contentdelete"},
                            {"contentid",contentId.ToString()},
                            {"recordid",recordId.ToString()}
                        };
                        Option_String = ""
                            + Environment.NewLine + "action=contentdelete"
                            + Environment.NewLine + "contentid=" + contentId
                            + Environment.NewLine + "recordid=" + recordId + "";
                    } else {
                        instanceArguments = new Dictionary<string, string> {
                            {"action","contentchange"},
                            {"contentid",contentId.ToString()},
                            {"recordid",recordId.ToString()}
                        };
                        Option_String = ""
                            + Environment.NewLine + "action=contentchange"
                            + Environment.NewLine + "contentid=" + contentId
                            + Environment.NewLine + "recordid=" + recordId + "";
                    }
                    while (csData.ok()) {
                        var addon = DbBaseModel.create<AddonModel>(core.cpParent, csData.getInteger("Addonid"));
                        if (addon != null) {
                            if (onChangeAddonsAsync) {
                                //
                                // -- execute addon async
                                core.addon.executeAsync(addon, instanceArguments);
                            } else {
                                //
                                // -- execute addon
                                core.addon.execute(addon, new CPUtilsBaseClass.addonExecuteContext {
                                    addonType = CPUtilsBaseClass.addonContext.ContextOnContentChange,
                                    backgroundProcess = false,
                                    errorContextMessage = "",
                                    argumentKeyValuePairs = instanceArguments
                                });
                            }
                        }
                        csData.goNext();
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
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
        ~ContentController()  {
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