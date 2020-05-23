
using System;
using System.Linq;
using System.Collections.Generic;
using Contensive.Processor.Controllers;

namespace Contensive.Processor {
    public class CPCacheClass : BaseClasses.CPCacheBaseClass {
        //
        private readonly CPClass cp;
        //
        //====================================================================================================
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cp"></param>
        /// <remarks></remarks>
        public CPCacheClass(CPClass cp) {
            this.cp = cp;
        }
        //
        //====================================================================================================
        //
        public override void Clear(List<string> keyList) => cp.core.cache.invalidate(keyList);
        //
        //====================================================================================================
        //
        public override object GetObject(string key) => cp.core.cache.getObject<object>(key);
        //
        //====================================================================================================
        //
        public override string GetText(string key) => cp.core.cache.getText(key);
        //
        //====================================================================================================
        //
        public override int GetInteger(string key) => cp.core.cache.getInteger(key);
        //
        //====================================================================================================
        //
        public override double GetNumber(string key) => cp.core.cache.getNumber(key);
        //
        //====================================================================================================
        //
        public override DateTime GetDate(string key) => cp.core.cache.getDate(key);
        //
        //====================================================================================================
        //
        public override bool GetBoolean(string key) => cp.core.cache.getBoolean(key);
        //
        //====================================================================================================
        //
        public override void Invalidate(string key) => cp.core.cache.invalidate(key);
        //
        //====================================================================================================
        //
        public override string CreateDependencyKeyInvalidateOnChange(string tableName) => CacheController.createCacheKey_TableObjectsInvalidationDate(tableName);
        //
        public override string CreateDependencyKeyInvalidateOnChange(string tableName, string dataSourceName) => CacheController.createCacheKey_TableObjectsInvalidationDate(tableName, dataSourceName);
        //
        public override void UpdateLastModified(string tableName) => cp.core.cache.store_LastRecordModifiedDate(tableName);
        //
        //====================================================================================================
        //
        public override string CreateKeyForDbRecord(int recordId, string tableName, string dataSourceName) => CacheController.createCacheKey_forDbRecord(recordId, tableName, dataSourceName);
        //
        public override string CreateKeyForDbRecord(int recordId, string tableName) => CacheController.createCacheKey_forDbRecord(recordId, tableName);
        //
        //====================================================================================================
        //
        public override string CreateKey(string objectName) => CacheController.createCacheKey_forObject(objectName);
        //
        public override string CreateKey(string objectName, string objectUniqueIdentifier = "") => CacheController.createCacheKey_forObject(objectName, objectUniqueIdentifier);
        //
        //====================================================================================================
        //
        public override string CreatePtrKeyforDbRecordGuid(string guid, string tableName, string dataSourceName) => CacheController.createCachePtr_forDbRecord_guid(guid, tableName, dataSourceName);
        //
        public override string CreatePtrKeyforDbRecordGuid(string guid, string tableName) => CacheController.createCachePtr_forDbRecord_guid(guid, tableName);
        //
        //====================================================================================================
        //
        public override string CreatePtrKeyforDbRecordUniqueName(string name, string tableName, string dataSourceName) => CacheController.createCachePtr_forDbRecord_uniqueName(name, tableName, dataSourceName);

        public override string CreatePtrKeyforDbRecordUniqueName(string name, string tableName) => CacheController.createCachePtr_forDbRecord_uniqueName(name, tableName);
        //
        //====================================================================================================
        //
        public override T GetObject<T>(string key) => cp.core.cache.getObject<T>(key);
        //
        //====================================================================================================
        //
        public override void Store(string key, object value) => cp.core.cache.storeObject(key, value);
        //
        //====================================================================================================
        //
        public override void Store(string key, object value, DateTime invalidationDate) => cp.core.cache.storeObject(key, value, invalidationDate);
        //
        //====================================================================================================
        //
        public override void Store(string key, object value, List<string> dependentKeyList) => cp.core.cache.storeObject(key, value, dependentKeyList);
        //
        //====================================================================================================
        //
        public override void Store(string key, object value, DateTime invalidationDate, List<string> dependentKeyList) => cp.core.cache.storeObject(key, value, invalidationDate, dependentKeyList);
        //
        //====================================================================================================
        //
        public override void Store(string key, object value, string dependentKey)
            => cp.core.cache.storeObject(key, value, dependentKey.Split(',').ToList());
        //
        //====================================================================================================
        //
        public override void Store(string key, object value, DateTime invalidationDate, string dependentKey) => cp.core.cache.storeObject(key, value, invalidationDate, dependentKey.Split(',').ToList());
        //
        //====================================================================================================
        //
        public override void StorePtr(string keyPtr, string key) {
            cp.core.cache.storePtr(keyPtr, key);
        }
        //
        //====================================================================================================
        /// <summary>
        /// Clear all cache values
        /// </summary>
        /// <remarks></remarks>
        public override void ClearAll() => cp.core.cache.invalidateAll();
        //
        //====================================================================================================
        /// <summary>
        /// clear all cache entries related to all tables used by a comma delimited list of content.
        /// </summary>
        /// <param name="ContentNameList"></param>
        /// <remarks></remarks>
        ///
        [Obsolete("Use Clear(dependentKeyList)", false)]
        public override void Clear(string ContentNameList) {
            if (!string.IsNullOrEmpty(ContentNameList)) {
                List<string> tableNameList = new List<string>();
                foreach (var contentName in new List<string>(ContentNameList.ToLowerInvariant().Split(','))) {
                    string tableName = MetadataController.getContentTablename(cp.core, contentName).ToLowerInvariant();
                    if (!tableNameList.Contains(tableName)) {
                        tableNameList.Add(tableName);
                        cp.core.cache.invalidateTableObjects(tableName);
                    }
                }
            }
        }
        //
        //====================================================================================================
        //
        public override void InvalidateAll()  {
            cp.core.cache.invalidateAll();
        }
        //
        //====================================================================================================
        //
        public override void InvalidateTagList(List<string> tagList) {
            cp.core.cache.invalidate(tagList);
        }
        //
        //====================================================================================================
        //
        public override void InvalidateContentRecord(string contentName, int recordId) {
            cp.core.cache.invalidateDbRecord(recordId, MetadataController.getContentTablename(cp.core, contentName));
        }
        //
        //====================================================================================================
        //
        public override void InvalidateTableRecord(string tableName, int recordId) {
            cp.core.cache.invalidateDbRecord(recordId, tableName);
        }
        //
        //====================================================================================================
        //
        public override void InvalidateTable(string tableName) {
            cp.core.cache.invalidateTableObjects(tableName);
        }
        //
        //====================================================================================================
        // deprecated
        //
        //
        //====================================================================================================
        //
        [Obsolete("deprecated",true)]
        public override void InvalidateTag(string tag) {
            cp.core.cache.invalidate(tag);
        }
        //
        [Obsolete("deprecated",true)]
        public override void SetKey(string key, object value) {
            cp.core.cache.storeObject(key, value);
        }
        //
        [Obsolete("deprecated",true)]
        public override void SetKey(string key, object value, DateTime invalidationDate) {
            cp.core.cache.storeObject(key, value, invalidationDate, new List<string> { });
        }
        //
        [Obsolete("deprecated",true)]
        public override void SetKey(string key, object value, List<string> tagList) {
            cp.core.cache.storeObject(key, value, tagList);
        }
        //
        [Obsolete("deprecated",true)]
        public override void SetKey(string key, object value, DateTime invalidationDate, List<string> tagList) {
            cp.core.cache.storeObject(key, value, invalidationDate, tagList);
        }
        //
        [Obsolete("deprecated",true)]
        public override void SetKey(string key, object value, string tag) {
            cp.core.cache.storeObject(key, value, tag);
        }
        //
        [Obsolete("deprecated",true)]
        public override void SetKey(string key, object Value, DateTime invalidationDate, string tag) {
            List<string> depKeyList = (string.IsNullOrWhiteSpace(tag) ? new List<string> { } : tag.Split(',').ToList());
            cp.core.cache.storeObject(key, Value, invalidationDate, depKeyList);
        }
        //
        [Obsolete("Use GetText()", false)]
        public override string Read(string Name) => cp.core.cache.getText(Name);
        /// 
        [Obsolete("Use StoreObject()", false)]
        public override void Save(string key, string Value) => Store(key, Value);
        //
        [Obsolete("Use StoreObject()", false)]
        public override void Save(string key, string Value, string invalidationTagCommaList) => Save(key, Value, invalidationTagCommaList, DateTime.MinValue);
        //
        [Obsolete("Use StoreObject()", false)]
        public override void Save(string key, string Value, string invalidationTagCommaList, DateTime invalidationDate) {
            try {
                List<string> invalidationTagList = new List<string>();
                if (!string.IsNullOrEmpty(invalidationTagCommaList.Trim())) {
                    invalidationTagList.AddRange(invalidationTagCommaList.Split(','));
                }
                if (invalidationDate.isOld()) {
                    cp.core.cache.storeObject(key, Value, invalidationTagList);
                } else {
                    cp.core.cache.storeObject(key, Value, invalidationDate, invalidationTagList);
                }
            } catch (Exception ex) {
                LogController.logError(cp.core, ex);
                throw;
            }
        }
   }
}