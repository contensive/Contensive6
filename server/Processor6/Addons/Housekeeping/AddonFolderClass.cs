//
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Constants;
using System.Xml;
using System.Collections.Generic;
using static Contensive.BaseClasses.CPFileSystemBaseClass;
using System.Globalization;
//
namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class AddonFolderClass {
        //====================================================================================================
        //
        public static void housekeep(CoreController core) {
            try {
                //
                LogController.logInfo(core, "HousekeepDaily, addon folder");
                //
                bool loadOK = true;
                XmlDocument Doc = new XmlDocument();
                string hint = "";
                try {
                    string collectionFileFilename = AddonController.getPrivateFilesAddonPath() + "Collections.xml";
                    string collectionFileContent = core.privateFiles.readFileText(collectionFileFilename);
                    Doc.LoadXml(collectionFileContent);
                } catch (Exception ex) {
                    LogController.logInfo(core, "RegisterAddonFolder, Hint=[" + hint + "], Error loading Collections.xml file, ex [" + ex + "]");
                    loadOK = false;
                }
                if (loadOK) {
                    //
                    LogController.logInfo(core, "Collection.xml loaded ok");
                    //
                    if (GenericController.toLCase(Doc.DocumentElement.Name) != GenericController.toLCase(CollectionListRootNode)) {
                        LogController.logInfo(core, "RegisterAddonFolder, Hint=[" + hint + "], The Collections.xml file has an invalid root node, [" + Doc.DocumentElement.Name + "] was received and [" + CollectionListRootNode + "] was expected.");
                    } else {
                        //
                        LogController.logInfo(core, "Collection.xml root name ok");
                        //
                        {
                            int NodeCnt = 0;
                            foreach (XmlNode LocalListNode in Doc.DocumentElement.ChildNodes) {
                                //
                                // Get the collection path
                                //
                                string collectionPath = "";
                                string localGuid = "";
                                string localName = "no name found";
                                DateTime lastChangeDate = default;
                                if (LocalListNode.Name.ToLower(CultureInfo.InvariantCulture).Equals("collection")) {
                                    localGuid = "";
                                    foreach (XmlNode CollectionNode in LocalListNode.ChildNodes) {
                                        switch (CollectionNode.Name.ToLower(CultureInfo.InvariantCulture)) {
                                            case "name":
                                                //
                                                localName = CollectionNode.InnerText.ToLower(CultureInfo.InvariantCulture);
                                                break;
                                            case "guid":
                                                //
                                                localGuid = CollectionNode.InnerText.ToLower(CultureInfo.InvariantCulture);
                                                break;
                                            case "path":
                                                //
                                                collectionPath = CollectionNode.InnerText.ToLower(CultureInfo.InvariantCulture);
                                                break;
                                            case "lastchangedate":
                                                lastChangeDate = GenericController.encodeDate(CollectionNode.InnerText);
                                                break;
                                            default:
                                                LogController.logWarn(core, "Collection node contains unrecognized child [" + CollectionNode.Name.ToLower(CultureInfo.InvariantCulture) + "]");
                                                break;
                                        }
                                    }
                                }
                                //
                                LogController.logInfo(core, "Node[" + NodeCnt + "], LocalName=[" + localName + "], LastChangeDate=[" + lastChangeDate + "], CollectionPath=[" + collectionPath + "], LocalGuid=[" + localGuid + "]");
                                //
                                // Go through all subpaths of the collection path, register the version match, unregister all others
                                //
                                if (string.IsNullOrEmpty(collectionPath)) {
                                    //
                                    LogController.logInfo(core, "no collection path, skipping");
                                    //
                                } else {
                                    collectionPath = GenericController.toLCase(collectionPath);
                                    string CollectionRootPath = collectionPath;
                                    int Pos = CollectionRootPath.LastIndexOf("\\") + 1;
                                    if (Pos <= 0) {
                                        //
                                        LogController.logInfo(core, "CollectionPath has no '\\', skipping");
                                        //
                                    } else {
                                        CollectionRootPath = CollectionRootPath.left(Pos - 1);
                                        string Path = AddonController.getPrivateFilesAddonPath() + CollectionRootPath + "\\";
                                        List<FolderDetail> folderList = new List<FolderDetail>();
                                        if (core.privateFiles.pathExists(Path)) {
                                            folderList = core.privateFiles.getFolderList(Path);
                                        }
                                        if (folderList.Count == 0) {
                                            //
                                            LogController.logInfo(core, "no subfolders found in physical path [" + Path + "], skipping");
                                            //
                                        } else {
                                            int folderPtr = -1;
                                            foreach (FolderDetail dir in folderList) {
                                                folderPtr += 1;
                                                //
                                                // -- check for empty foler name
                                                if (string.IsNullOrEmpty(dir.Name)) {
                                                    //
                                                    LogController.logInfo(core, "....empty folder skipped [" + dir.Name + "]");
                                                    continue;
                                                }
                                                //
                                                // -- preserve folder in use
                                                if (CollectionRootPath + "\\" + dir.Name == collectionPath) {
                                                    LogController.logInfo(core, "....active folder preserved [" + dir.Name + "]");
                                                    continue;
                                                }
                                                //
                                                // preserve last three folders
                                                if (folderPtr >= (folderList.Count - 3)) {
                                                    LogController.logInfo(core, "....last 3 folders reserved [" + dir.Name + "]");
                                                    continue;
                                                }
                                                //
                                                LogController.logInfo(core, "....Deleting unused folder [" + Path + dir.Name + "]");
                                                core.privateFiles.deleteFolder(Path + dir.Name);
                                            }
                                        }
                                    }
                                }
                                NodeCnt += 1;
                            }
                        }
                    }
                }
                //
                LogController.logInfo(core, "Exiting RegisterAddonFolder");
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
    }
}