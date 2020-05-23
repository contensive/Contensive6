﻿
using System;
using System.Collections.Generic;
using Contensive.Processor.Controllers;
using Contensive.BaseClasses;
using Contensive.Processor.Models.Domain;
using Contensive.Processor.Exceptions;
using Contensive.Models.Db;
using Contensive.CPBase.BaseModels;

namespace Contensive.Processor {
    public class CPContentClass : CPContentBaseClass {
        //
        private readonly CPClass cp;

        public override LastestDateTrackerBaseModel LatestContentModifiedDate {
            get {
                return _LatestContentModifiedDate;
            }
            set { }
        }
        private readonly LastestDateTracker _LatestContentModifiedDate = new LastestDateTracker();
        //
        //====================================================================================================
        //
        public CPContentClass(CPClass cp) {
            this.cp = cp;
        }
        //
        //====================================================================================================
        //
        public override int GetTableID(string tableName) {
            var table = DbBaseModel.createByUniqueName<TableModel>(cp, tableName);
            if (table == null) { return 0; }
            return table.id;
        }
        //
        //====================================================================================================
        //
        public override string GetCopy(string copyName, string DefaultContent) {
            return cp.core.html.getContentCopy(copyName, DefaultContent, cp.core.session.user.id, true, cp.core.session.isAuthenticated);
        }
        //
        public override string GetCopy(string copyName) {
            return cp.core.html.getContentCopy(copyName, "", cp.core.session.user.id, true, cp.core.session.isAuthenticated);
        }
        //
        //====================================================================================================
        //
        public override string GetCopy(string copyName, string defaultContent, int personalizationPeopleId) {
            return cp.core.html.getContentCopy(copyName, defaultContent, personalizationPeopleId, true, cp.core.session.isAuthenticated);
        }
        //
        //====================================================================================================
        //
        public override void SetCopy(string copyName, string content) {
            cp.core.html.setContentCopy(copyName, content);
        }
        //
        //====================================================================================================
        //
        public override string GetAddLink(string contentName, string presetNameValueList, bool allowPaste, bool isEditing) {
            string result = "";
            foreach (var link in AdminUIController.getRecordAddAnchorTag(cp.core, contentName, presetNameValueList, allowPaste, isEditing)) {
                result += link;
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public override string GetAddLink(string contentName, string presetNameValueList) {
            string result = "";
            foreach (var link in AdminUIController.getRecordAddAnchorTag(cp.core, contentName, presetNameValueList, false, true)) {
                result += link;
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public override string GetAddLink(string contentName) {
            string result = "";
            foreach (var link in AdminUIController.getRecordAddAnchorTag(cp.core, contentName, "", false, true)) {
                result += link;
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public override string GetAddLink(int contentId, string PresetNameValueList) {
            throw new NotImplementedException();
        }
        //
        //====================================================================================================
        //
        public override string GetAddLink(int contentId) {
            throw new NotImplementedException();
        }
        //
        //====================================================================================================
        //
        public override string GetContentControlCriteria(string contentName) {
            var meta = ContentMetadataModel.createByUniqueName(cp.core, contentName);
            if (meta != null) { return meta.legacyContentControlCriteria; }
            return string.Empty;
        }
        //
        //====================================================================================================
        //
        [Obsolete("deprecated, instead create contentMeta, lookup field and use property of field", false)]
        public override string GetFieldProperty(string contentName, string fieldName, string propertyName) {
            throw new GenericException("getContentFieldProperty deprecated, instead create contentMeta, lookup field and use property of field");
        }
        //
        //====================================================================================================
        //
        public override string GetDataSource(string contentName) {
            var meta = ContentMetadataModel.createByUniqueName(cp.core, contentName);
            if (meta != null) { return meta.dataSourceName; }
            return string.Empty;
        }
        //
        //====================================================================================================
        //
        public override string GetEditLink(string contentName, string recordID, bool allowCut, string recordName, bool isEditing) {
            return AdminUIController.getRecordEditAndCutAnchorTag(cp.core, contentName, GenericController.encodeInteger(recordID), allowCut, recordName);
        }
        //
        //====================================================================================================
        //
        public override string GetEditLink(string contentName, int recordId) {
            return AdminUIController.getRecordEditAndCutAnchorTag(cp.core, contentName, recordId, false, "");
        }
        //
        //====================================================================================================
        //
        public override string GetEditLink(string contentName, string recordGuid) {
            var contentMetadata = ContentMetadataModel.createByUniqueName(cp.core, contentName);
            if (contentMetadata == null) { throw new GenericException("ContentName [" + contentName + "], but no content metadata found with this name."); }
            return AdminUIController.getRecordEditAnchorTag(cp.core, contentMetadata, recordGuid);
        }
        //
        //====================================================================================================
        //
        public override string GetEditLink(int contentId, int recordId) {
            var contentMetadata = ContentMetadataModel.create(cp.core, contentId);
            if (contentMetadata == null) { throw new GenericException("contentId [" + contentId + "], but no content metadata found."); }
            return AdminUIController.getRecordEditAnchorTag(cp.core, contentMetadata, recordId);
        }
        //
        //====================================================================================================
        //
        public override string GetEditLink(int contentId, string recordGuid) {
            var contentMetadata = ContentMetadataModel.create(cp.core, contentId);
            if (contentMetadata == null) { throw new GenericException("contentId [" + contentId + "], but no content metadata found."); }
            return AdminUIController.getRecordEditAnchorTag(cp.core, contentMetadata, recordGuid);
        }
        //
        //====================================================================================================
        //
        public override string GetLinkAliasByPageID(int pageID, string queryStringSuffix, string defaultLink) {
            return LinkAliasController.getLinkAlias(cp.core, pageID, queryStringSuffix, defaultLink);
        }
        //
        //====================================================================================================
        //
        public override string GetEditWrapper(string innerHtml) {
            return AdminUIController.getEditWrapper(cp.core, innerHtml);
        }
        //
        //====================================================================================================
        //
        public override string GetEditWrapper(string innerHtml, string contentName, int recordId) {
            return AdminUIController.getEditWrapper(cp.core, innerHtml, contentName, recordId);
        }
        //
        //====================================================================================================
        //
        public override string GetEditWrapper(string innerHtml, string contentName, string recordGuid) {
            return AdminUIController.getEditWrapper(cp.core, innerHtml, contentName, recordGuid);
        }
        //
        //====================================================================================================
        //
        public override string GetEditWrapper(string innerHtml, int contentId, int recordId) {
            return AdminUIController.getEditWrapper(cp.core, innerHtml, contentId, recordId);
        }
        //
        //====================================================================================================
        //
        public override string GetEditWrapper(string innerHtml, int contentId, string recordGuid) {
            return AdminUIController.getEditWrapper(cp.core, innerHtml, contentId, recordGuid);
        }
        //
        //====================================================================================================
        //
        public override string GetPageLink(int pageID, string queryStringSuffix, bool allowLinkAlias) {
            return PageContentController.getPageLink(cp.core, pageID, queryStringSuffix, allowLinkAlias, false);
        }
        //
        public override string GetPageLink(int pageID, string queryStringSuffix) {
            return PageContentController.getPageLink(cp.core, pageID, queryStringSuffix, true, false);
        }
        //
        public override string GetPageLink(int pageID) {
            return PageContentController.getPageLink(cp.core, pageID, "", true, false);
        }
        //
        //====================================================================================================
        /// <summary>
        /// get a record id from its unique name. If a duplicate exists, the first ordered by id is returned
        /// </summary>
        /// <param name="contentName"></param>
        /// <param name="recordName"></param>
        /// <returns></returns>
        public override int GetRecordID(string contentName, string recordName) {
            return MetadataController.getRecordIdByUniqueName(cp.core, contentName, recordName);
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the matching record name if a match is found, otherwise blank. Does NOT validate the record.
        /// </summary>
        /// <param name="contentName"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public override string GetRecordName(string contentName, int recordID) {
            var meta = ContentMetadataModel.createByUniqueName(cp.core, contentName);
            if (meta == null) { return string.Empty; }
            return meta.getRecordName(cp.core, recordID);
        }
        //
        //====================================================================================================
        //
        public override string GetTable(string contentName) {
            var meta = ContentMetadataModel.createByUniqueName(cp.core, contentName);
            if (meta == null) { return string.Empty; }
            return meta.tableName;
        }
        //
        //====================================================================================================
        //
        public override bool IsField(string contentName, string fieldName) {
            var contentMetadata = Models.Domain.ContentMetadataModel.createByUniqueName(cp.core, contentName);
            return (contentMetadata == null) ? false : contentMetadata.containsField(cp.core, fieldName);
        }
        //
        //====================================================================================================
        //
        public override bool IsLocked(string contentName, string recordId) {
            var contentTable = TableModel.createByContentName(cp, contentName);
            if (contentTable != null) return WorkflowController.isRecordLocked(cp.core, contentTable.id, GenericController.encodeInteger(recordId));
            return false;
        }
        //
        //====================================================================================================
        //
        public override bool IsChildContent(string childContentID, string parentContentID) {
            var parentMetadata = ContentMetadataModel.create(cp.core, parentContentID);
            return (parentMetadata == null) ? false : parentMetadata.isParentOf(cp.core, GenericController.encodeInteger(childContentID));
        }
        //
        //====================================================================================================
        //
        public override string getLayout(string layoutName) {
            try {
                if (string.IsNullOrWhiteSpace(layoutName)) { return string.Empty; }
                using (var cs = new CsModel(cp.core)) {
                    cs.open("layouts", "name=" + DbController.encodeSQLText(layoutName), "id", false, cp.core.session.user.id, "layout");
                    if (cs.ok()) { return cs.getText("layout"); }
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
            return string.Empty;
        }
        //
        //====================================================================================================
        //
        public override string GetLayout(int layoutid) {
            try {
                using (var cs = new CsModel(cp.core)) {
                    string sql = "select layout from ccLayouts where id=" + layoutid;
                    cs.openSql(sql);
                    if (cs.ok()) { return cs.getText("layout"); }
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
            }
            return string.Empty;
        }
        //
        //====================================================================================================
        //
        public override int AddRecord(string contentName, string recordName) {
            int recordId = 0;
            try {
                CsModel cs = new CsModel(cp.core);
                if (cs.insert(contentName)) {
                    cs.set("name", recordName);
                    recordId = cs.getInteger("id");
                }
                cs.close();
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
                throw;
            }
            return recordId;
        }
        //
        //====================================================================================================
        //
        public override int AddRecord(string contentName) {
            int result = 0;
            try {
                CsModel cs = new CsModel(cp.core);
                if (cs.insert(contentName)) {
                    result = cs.getInteger("id");
                }
                cs.close();
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        //
        public override void Delete(string contentName, string sqlCriteria) {
            MetadataController.deleteContentRecords(cp.core, contentName, sqlCriteria);
        }
        //
        //====================================================================================================
        //
        public override void DeleteContent(string contentName) {
            DbBaseModel.delete<ContentModel>(cp, ContentMetadataModel.getContentId(cp.core, contentName));
        }
        //
        //====================================================================================================
        //
        public override int AddContentField(string contentName, string fieldName, CPContentBaseClass.FieldTypeIdEnum fieldType) {
            var contentMetadata = ContentMetadataModel.createByUniqueName(cp.core, contentName);
            var fieldMeta = ContentFieldMetadataModel.createDefault(cp.core, fieldName, fieldType);
            contentMetadata.verifyContentField(cp.core, fieldMeta, false, "Api CPContent.AddContentField [" + contentName + "." + fieldName + "]");
            return fieldMeta.id;
        }
        //
        public override int AddContentField(string contentName, string fieldName, int fieldTypeId)
            => AddContentField(contentName, fieldName, (CPContentBaseClass.FieldTypeIdEnum)fieldTypeId);
        //
        //====================================================================================================
        //
        public override int AddContent(string contentName) {
            return AddContent(contentName, contentName.Replace(' '.ToString(), "").Replace(' '.ToString(), ""), "default");
        }
        //
        //====================================================================================================
        //
        public override int AddContent(string contentName, string sqlTableName) {
            return AddContent(contentName, sqlTableName, "default");
        }
        //
        //====================================================================================================
        //
        public override int AddContent(string contentName, string sqlTableName, string dataSourceName) {
            var tmpList = new List<string> { };
            DataSourceModel dataSource = DataSourceModel.createByUniqueName(cp, dataSourceName, ref tmpList);
            return ContentMetadataModel.verifyContent_returnId(cp.core, new Models.Domain.ContentMetadataModel {
                dataSourceName = dataSource.name,
                tableName = sqlTableName,
                name = contentName
            }, "Adding content [" + contentName + "], sqlTableName [" + sqlTableName + "]");
        }
        //
        //====================================================================================================
        //
        public override string GetListLink(string contentName) {
            return AdminUIController.getRecordEditAnchorTag(cp.core, Models.Domain.ContentMetadataModel.createByUniqueName(cp.core, contentName));
        }
        //
        //====================================================================================================
        //
        public override int GetID(string ContentName) {
            var content = DbBaseModel.createByUniqueName<ContentModel>(cp, ContentName);
            if (content != null) return content.id;
            return 0;
        }
        //
        //====================================================================================================
        //
        public override string GetName(int contentId) {
            var content = DbBaseModel.create<ContentModel>(cp, contentId);
            if (content != null) return content.name;
            return string.Empty;
        }
        //
        //====================================================================================================
        // deprecated
        //
        [Obsolete("Use AddRecord( string contentName ) ", false)]
        public override int AddRecord(object ContentName) {
            return AddRecord(cp.Utils.EncodeText(ContentName));
        }
        //
        [Obsolete("workflow editing is deprecated", false)]
        public override bool IsWorkflow(string ContentName) {
            //
            // -- workflow no longer supported (but may come back)
            return false;
        }
        //
        [Obsolete("workflow editing is deprecated", false)]
        public override void PublishEdit(string ContentName, int RecordID) {
            // 
        }
        //
        [Obsolete("workflow editing is deprecated", false)]
        public override void SubmitEdit(string ContentName, int RecordID) {
            //
        }
        //
        [Obsolete("workflow editing is deprecated", false)]
        public override void AbortEdit(string ContentName, int RecordId) {
            // 
        }
        //
        [Obsolete("workflow editing is deprecated", false)]
        public override void ApproveEdit(string ContentName, int RecordId) {
            //
        }
        //
        [Obsolete("Deprecated, template link is not supported", false)]
        public override string GetTemplateLink(int TemplateID) {
            return "";
        }
        //
        [Obsolete("Deprecated, access model properties instead", false)]
        public override string GetProperty(string ContentName, string PropertyName) {
            var contentMetadata = ContentMetadataModel.createByUniqueName(cp.core, ContentName);
            if (contentMetadata == null) { return string.Empty; }
            return contentMetadata.getContentProperty(cp.core, PropertyName);
        }
        //
        [Obsolete("Deprecated, use methods tih FieldTypeIdEnum instead", false)]
        public override int AddContentField(string ContentName, string FieldName, fileTypeIdEnum fileTypeEnum) {
            AddContentField(ContentName, FieldName, (FieldTypeIdEnum)fileTypeEnum);
            throw new NotImplementedException();
        }
    }
}
