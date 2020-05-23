
using System;
using Contensive.Processor.Controllers;
using Contensive.BaseClasses;
using System.Collections.Generic;

namespace Contensive.Processor.Addons.Housekeeping {
    //
    public static class MetadataClass {
        public static void housekeep(CoreController core, HouseKeepEnvironmentModel env) {
            try {
                //
                LogController.logInfo(core, "Housekeep, metadata");
                //
                //
                // block duplicate redirect fields (match contentid+fieldtype+caption)
                //
                LogController.logInfo(core, "Inactivate duplicate redirect fields");
                int FieldContentId = 0;
                string FieldLast = null;
                string FieldNew = null;
                int FieldRecordId = 0;
                using (var csData = new CsModel(core)) {
                    csData.openSql("Select ID, ContentID, Type, Caption from ccFields where (active<>0)and(Type=" + (int)CPContentBaseClass.FieldTypeIdEnum.Redirect + ") Order By ContentID, Caption, ID");
                    FieldLast = "";
                    while (csData.ok()) {
                        FieldContentId = csData.getInteger("Contentid");
                        string FieldCaption = csData.getText("Caption");
                        FieldNew = FieldContentId + FieldCaption;
                        if (FieldNew == FieldLast) {
                            FieldRecordId = csData.getInteger("ID");
                            core.db.executeNonQuery("Update ccFields set active=0 where ID=" + FieldRecordId + ";");
                        }
                        FieldLast = FieldNew;
                        csData.goNext();
                    }
                }
                //
                // convert FieldTypeLongText + htmlContent to FieldTypeHTML
                LogController.logInfo(core, "convert FieldTypeLongText + htmlContent to FieldTypeHTML.");
                string sql = "update ccfields set type=" + (int)CPContentBaseClass.FieldTypeIdEnum.HTML + " where type=" + (int)CPContentBaseClass.FieldTypeIdEnum.LongText + " and ( htmlcontent<>0 )";
                core.db.executeNonQuery(sql);
                //
                // Content TextFile types with no controlling record
                //
                if (GenericController.encodeBoolean(core.siteProperties.getText("ArchiveAllowFileClean", "false"))) {
                    //
                    int DSType = core.db.getDataSourceType();
                    LogController.logInfo(core, "Content TextFile types with no controlling record.");
                    using (var csData = new CsModel(core)) {
                        sql = "SELECT DISTINCT ccTables.Name as TableName, ccFields.Name as FieldName"
                            + " FROM (ccFields LEFT JOIN ccContent ON ccFields.ContentId = ccContent.ID) LEFT JOIN ccTables ON ccContent.ContentTableId = ccTables.ID"
                            + " Where (((ccFields.Type) = 10))"
                            + " ORDER BY ccTables.Name";
                        csData.openSql(sql);
                        while (csData.ok()) {
                            //
                            // Get all the files in this path, and check that the record exists with this in its field
                            //
                            string FieldName = csData.getText("FieldName");
                            string TableName = csData.getText("TableName");
                            string PathName = TableName + "\\" + FieldName;
                            List<CPFileSystemBaseClass.FileDetail> FileList = core.cdnFiles.getFileList(PathName);
                            if (FileList.Count > 0) {
                                core.db.executeNonQuery("CREATE INDEX temp" + FieldName + " ON " + TableName + " (" + FieldName + ")");
                                foreach (CPFileSystemBaseClass.FileDetail file in FileList) {
                                    string Filename = file.Name;
                                    string VirtualFileName = PathName + "\\" + Filename;
                                    string VirtualLink = GenericController.strReplace(VirtualFileName, "\\", "/");
                                    long FileSize = file.Size;
                                    if (FileSize == 0) {
                                        sql = "update " + TableName + " set " + FieldName + "=null where (" + FieldName + "=" + DbController.encodeSQLText(VirtualFileName) + ")or(" + FieldName + "=" + DbController.encodeSQLText(VirtualLink) + ")";
                                        core.db.executeNonQuery(sql);
                                        core.cdnFiles.deleteFile(VirtualFileName);
                                    } else {
                                        using (var csTest = new CsModel(core)) {
                                            sql = "SELECT ID FROM " + TableName + " WHERE (" + FieldName + "=" + DbController.encodeSQLText(VirtualFileName) + ")or(" + FieldName + "=" + DbController.encodeSQLText(VirtualLink) + ")";
                                            if (!csTest.openSql(sql)) {
                                                core.cdnFiles.deleteFile(VirtualFileName);
                                            }
                                        }
                                    }
                                }
                                if (DSType == 1) {
                                    // access
                                    sql = "Drop INDEX temp" + FieldName + " ON " + TableName;
                                } else if (DSType == 2) {
                                    // sql server
                                    sql = "DROP INDEX " + TableName + ".temp" + FieldName;
                                } else {
                                    // mysql
                                    sql = "ALTER TABLE " + TableName + " DROP INDEX temp" + FieldName;
                                }
                                core.db.executeNonQuery(sql);
                            }
                            csData.goNext();
                        }
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}