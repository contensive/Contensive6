
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
using System.Runtime.Caching;
using Contensive.Processor.Exceptions;
using System.Reflection;
using Enyim.Caching;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Contensive.Processor.Models.Domain;
using static Newtonsoft.Json.JsonConvert;
using Contensive.Models.Db;
//
namespace Contensive.Processor.Controllers {
    //
    //====================================================================================================
    /// <summary>
    /// Interface to cache systems. Cache objects are saved to dotnet cache, remotecache, filecache. 
    /// 
    /// 3 types of cache methods
    /// -- 1) primary key -- cache key holding the content.
    /// -- 2) dependent key -- a dependent key holds content that the primary cache depends on. If the dependent cache is invalid, the primary cache is invalid
    /// -------- if an org cache includes a primary content person, then the org (org/id/10) is saved with a dependent key (ccmembers/id/99). The org content id valid only if both the primary and dependent keys are valid
    /// -- 3) pointer key -- a cache entry that holds a primary key (or another pointer key). When read, the cache returns the primary value. (primary="ccmembers/id/10", pointer="ccmembers/ccguid/{1234}"
    /// ------- pointer keys are read-only, as the content is saved in the primary key
    /// 
    /// 2  types of cache data:
    ///  -- record cache -- cache of an Db model (one record in a db table)
    ///     .. saveCache, reachCache in the entity model
    ///     .. invalidateCache in the entity model, includes invalication for complex objects that include the entity
    ///      
    ///  -- complex cache -- cache of a domain model, an object with mixed data based on the entity data model (an org object that contains a person object)
    ///    .. has an id for the topmost object
    ///    .. these objects are in .. ? (another model folder?)
    ///    
    /// 3 using dependent keys to track groups of record
    /// -- "table-objects-invalidate-date" key
    ///      use only as a dependency in other cache
    ///         - any cache with this as the dependency will be invalidated if it's save-date is after this table-invalidation-date
    ///      use to trigger a clear of all record-cache from one table
    ///      this key is updated when non-specific changes are made to the table, like 'delete all records matching an odd criteria'
    ///      updating this key invalidates all cache objects with data from that table saved before this date
    ///      when you run a query that updates misc records, invalidate this key
    ///      when you save an object that includes a table record, make it dependent on this key so it will be cleared if it's save date is before this date
    ///      
    /// -- "last-record-modified-date" key
    ///      this cache object contains the modified date of the record last modified for this table
    ///      to update it, use the DbModel method DbModel.storeCacheLastRecordModifiedDate()
    ///      it is updated automatically on every Db record update through models
    ///      if you update a Db record outside models (update/insert/delete query, etc.) also update this cache object
    ///      ex: the admin navigator. the object contains many records and we want it to clear if any add-on or navigator entry record is modified/added/deleted
    ///      for record-cache, only udpate the cache key on save. Do not make any record-cache dependent on this key
    ///      when you save a complex object and you want it invalidated if any record in a table is updated, make it dependent on this key
    ///      invalidate this key everytime any a record is updated.
    ///      
    /// -- if you add/modify/delete a database record
    ///      => if you update using DbModels this is handled internally and nothing more needs to be done
    ///      => otherwise, use the DbModel method setCacheRecordLastModified
    /// 
    /// -- save a record that is dependent on many untrackable records 
    /// 
    /// -- update any record is a table
    /// 
    /// -- update one or more untracked records (an update or delete command with where clause unrelated to id or guid)
    ///    
    /// When a record is saved in admin, an invalidation call is made for tableName/id/#
    /// 
    /// If code edits a db record, you should call invalidate for tablename/id/#
    /// 
    /// one-to-many lists: if the list is cached, it has to include dependent keys for all its included members
    /// 
    /// many-to-many lists: should have a model for the rule table, and lists should come from methods there. the delete method must invalidate the rule model cache
    /// 
    /// A cacheobject has:
    ///   key - if the object is a model of a database record, and there is only one model for that db record, use tableName+id as the cacheName. If it contains other data,
    ///     use the modelName+id as the cacheName, and include all records in the dependentObjectList.
    ///   primaryObjectKey -- a secondary cacheObject does not hold data, just a pointer to a primary object. For example, if you use both id and guid to reference objects,
    ///     save the data in the primary cacheObject (tableName+id) and save a secondary cacheObject (tablename+guid). Sve both when you update the cacheObject. requesting either
    ///     or invalidating either will effect the primary cache
    ///   dependentObjectList -- if a cacheobject contains data from multiple sources, each source should be included in the dependencyList. This includes both cached models and Db records.
    ///   
    /// Special cache entries
    ///     dbTablename - cachename = the name of the table
    ///         - when any record is saved to the table, this dbTablename cache is updated
    ///         - objects like addonList depend on it, and are flushed if ANY record in that table is updated
    ///         
    /// </summary>
    [Serializable]
    public class CacheController : IDisposable {
        //
        // ====================================================================================================
        // ----- objects passed in constructor, do not dispose
        //
        private readonly CoreController core;
        //
        // ====================================================================================================
        // ----- objects constructed that must be disposed
        //
        private Enyim.Caching.MemcachedClient cacheClient;
        //
        // ====================================================================================================
        // ----- private instance storage
        //
        private readonly bool remoteCacheInitialized;
        //
        //========================================================================
        /// <summary>
        /// get an object of type TData from cache. If the cache misses or is invalidated, null object is returned
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TData getObject<TData>(string key) {
            try {
                key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                if (string.IsNullOrEmpty(key)) { return default(TData); }
                //
                // -- read cacheDocument (the object that holds the data object plus control fields)
                CacheDocumentClass cacheDocument = getCacheDocument(key);
                if (cacheDocument == null) { return default(TData); }
                //
                // -- test for global invalidation
                int dateCompare = globalInvalidationDate.CompareTo(cacheDocument.saveDate);
                if (dateCompare >= 0) {
                    //
                    // -- global invalidation
                    LogController.logTrace(core, "key [" + key + "], invalidated because cacheObject saveDate [" + cacheDocument.saveDate + "] is before the globalInvalidationDate [" + globalInvalidationDate + "]");
                    return default(TData);
                }
                //
                // -- test all dependent objects for invalidation (if they have changed since this object changed, it is invalid)
                bool cacheMiss = false;
                foreach (string dependentKey in cacheDocument.dependentKeyList) {
                    CacheDocumentClass dependantCacheDocument = getCacheDocument(dependentKey);
                    if (dependantCacheDocument == null) {
                        // create dummy cache to validate future cache requests, fake saveDate as last globalinvalidationdate
                        storeCacheDocument(dependentKey, new CacheDocumentClass(core.dateTimeNowMockable) {
                            keyPtr = null,
                            content = "",
                            saveDate = globalInvalidationDate
                        });
                    } else {
                        dateCompare = dependantCacheDocument.saveDate.CompareTo(cacheDocument.saveDate);
                        if (dateCompare >= 0) {
                            //
                            // -- invalidate because a dependent document was changed after the cacheDocument was saved
                            cacheMiss = true;
                            LogController.logTrace(core, "[" + key + "], invalidated because the dependantKey [" + dependentKey + "] was modified [" + dependantCacheDocument.saveDate + "] after the cacheDocument's saveDate [" + cacheDocument.saveDate + "]");
                            break;
                        }
                    }
                }
                TData result = default(TData);
                if (!cacheMiss) {
                    if (!string.IsNullOrEmpty(cacheDocument.keyPtr)) {
                        //
                        // -- this is a pointer key, load the primary
                        result = getObject<TData>(cacheDocument.keyPtr);
                    } else if (cacheDocument.content is Newtonsoft.Json.Linq.JObject dataJObject) {
                        //
                        // -- newtonsoft types
                        result = dataJObject.ToObject<TData>();
                    } else if (cacheDocument.content is Newtonsoft.Json.Linq.JArray dataJArray) {
                        //
                        // -- newtonsoft types
                        result = dataJArray.ToObject<TData>();
                    } else if (cacheDocument.content == null) {
                        //
                        // -- if cache data was left as a string (might be empty), and return object is not string, there was an error
                        result = default(TData);
                    } else {
                        //
                        // -- all worked, but if the class is unavailable let it return default like a miss
                        try {
                            result = (TData)cacheDocument.content;
                        } catch (Exception ex) {
                            //
                            // -- object value did not match. return as miss
                            LogController.logWarn(core, "cache getObject failed to cast value as type, key [" + key + "], type requested [" +  typeof(TData).FullName + "], ex [" + ex + "]");
                            result = default(TData);
                        }
                    }
                }
                return result;
            } catch (Exception ex) {
                LogController.logError(core, ex);
                return default(TData);
            }
        }
        //
        //========================================================================
        /// <summary>
        /// get an object from cache. If the cache misses or is invalidated, return ""
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getText(string key) => getObject<string>(key) ?? "";
        //
        //========================================================================
        /// <summary>
        /// get an object from cache. If the cache misses or is invalidated, return 0
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int getInteger(string key) => getObject<int>(key);
        //
        //========================================================================
        /// <summary>
        /// get an object from cache. If the cache misses or is invalidated, return 0.0
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double getNumber(string key) => getObject<double>(key);
        //
        //========================================================================
        /// <summary>
        /// get an object from cache. If the cache misses or is invalidated, return minDate
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DateTime getDate(string key) => getObject<DateTime>(key);
        //
        //========================================================================
        /// <summary>
        /// get an object from cache. If the cache misses or is invalidated, return false
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool getBoolean(string key) => getObject<bool>(key);
        //
        //====================================================================================================
        /// <summary>
        /// get a cache object from the cache. returns the cacheObject that wraps the object
        /// </summary>
        /// <typeparam name="returnType"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private CacheDocumentClass getCacheDocument(string key) {
            CacheDocumentClass result = null;
            try {
                // - verified in createServerKey() -- key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                if (string.IsNullOrEmpty(key)) {
                    throw new ArgumentException("cache key cannot be blank");
                }
                string serverKey = createServerKey(key);
                string typeMessage = "";
                if (remoteCacheInitialized) {
                    //
                    // -- use remote cache
                    typeMessage = "remote";
                    try {
                        result = cacheClient.Get<CacheDocumentClass>(serverKey);
                    } catch (Exception ex) {
                        //
                        // --client does not throw its own errors, so try to differentiate by message
                        if (ex.Message.ToLowerInvariant().IndexOf("unable to load type") >= 0) {
                            //
                            // -- trying to deserialize an object and this code does not have a matching class, clear cache and return empty
                            LogController.logWarn(core, ex);
                            cacheClient.Remove(serverKey);
                            result = null;
                        } else {
                            //
                            // -- some other error
                            LogController.logError(core, ex);
                            throw;
                        }
                    }
                }
                if ((result == null) && core.serverConfig.enableLocalMemoryCache) {
                    //
                    // -- local memory cache
                    typeMessage = "local-memory";
                    result = (CacheDocumentClass)MemoryCache.Default[serverKey];
                }
                if ((result == null) && core.serverConfig.enableLocalFileCache) {
                    //
                    // -- local file cache
                    typeMessage = "local-file";
                    string serializedDataObject = null;
                    using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, serverKey)) {
                        mutex.WaitOne();
                        serializedDataObject = core.privateFiles.readFileText("appCache\\" + FileController.encodeDosFilename(serverKey + ".txt"));
                        mutex.ReleaseMutex();
                    }
                    if (!string.IsNullOrEmpty(serializedDataObject)) {
                        result = DeserializeObject<CacheDocumentClass>(serializedDataObject);
                        storeCacheDocument_MemoryCache(serverKey, result);
                    }
                }
                string returnContentSegment = SerializeObject(result);
                returnContentSegment = (returnContentSegment.Length > 50) ? returnContentSegment.Substring(0, 50) : returnContentSegment;
                //
                // -- log result
                if (result == null) {
                    LogController.logTrace(core, "miss, cacheType [" + typeMessage + "], key [" + key + "]");
                } else {
                    if (result.content == null) {
                        LogController.logTrace(core, "hit, cacheType [" + typeMessage + "], key [" + key + "], saveDate [" + result.saveDate + "], content [null]");
                    } else {
                        string content = result.content.ToString();
                        content = (content.Length > 50) ? (content.left(50) + "...") : content;
                        LogController.logTrace(core, "hit, cacheType [" + typeMessage + "], key [" + key + "], saveDate [" + result.saveDate + "], content [" + content + "]");
                    }
                }
                //
                // if dependentKeyList is null, return an empty list, not null
                if (result != null) {
                    //
                    // -- empty objects return nothing, empty lists return count=0
                    if (result.dependentKeyList == null) {
                        result.dependentKeyList = new List<string>();
                    }
                }

            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// save an object to cache, with invalidation date and dependentKeyList
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="invalidationDate"></param>
        /// <param name="dependentKeyList">Each tag should represent the source of data, and should be invalidated when that source changes.</param>
        /// <remarks></remarks>
        public void storeObject(string key, object content, DateTime invalidationDate, List<string> dependentKeyList) {
            try {
                key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                var cacheDocument = new CacheDocumentClass(core.dateTimeNowMockable) {
                    content = content,
                    saveDate = core.dateTimeNowMockable,
                    invalidationDate = invalidationDate,
                    dependentKeyList = dependentKeyList
                };
                storeCacheDocument(key, cacheDocument);
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// save a cache value, compatible with legacy method signature.
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="dependentKeyList">List of dependent keys.</param>
        /// <remarks>If a dependent key is invalidated, it's parent key is also invalid. 
        /// ex - org/id/10 has primary contact person/id/99. if org/id/10 object includes person/id/99 object, then org/id/10 depends on person/id/99,
        /// and "person/id/99" is a dependent key for "org/id/10". When "org/id/10" is read, it checks all its dependent keys (person/id/99) and
        /// invalidates if any dependent key is invalid.</remarks>
        public void storeObject(string key, object content, List<string> dependentKeyList) {
            storeObject(key, content, core.dateTimeNowMockable.AddDays(invalidationDaysDefault), dependentKeyList);
        }
        //
        //====================================================================================================
        /// <summary>
        /// save a cache value, compatible with legacy method signature.
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="dependantKey"></param>
        /// <remarks></remarks>
        public void storeObject(string key, object content, string dependantKey) {
            storeObject(key, content, core.dateTimeNowMockable.AddDays(invalidationDaysDefault), new List<string> { dependantKey });
        }
        //
        //====================================================================================================
        /// <summary>
        /// save an object to cache, with invalidation date
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="invalidationDate"></param>
        /// <remarks></remarks>
        public void storeObject(string key, object content, DateTime invalidationDate) {
            storeObject(key, content, invalidationDate, new List<string> { });
        }
        //
        //====================================================================================================
        /// <summary>
        /// save an object to cache, for compatibility with existing site. Always use a key generated from createKey methods
        /// </summary>
        /// <param name="key">key generated from createKey methods</param>
        /// <param name="content"></param>
        public void storeObject(string key, object content) {
            storeObject(key, content, core.dateTimeNowMockable.AddDays(invalidationDaysDefault), new List<string> { });
        }
        //
        //====================================================================================================
        /// <summary>
        /// save a Db Model cache.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="recordId"></param>
        /// <param name="tableName"></param>
        /// <param name="datasourceName"></param>
        /// <param name="modelContent"></param>
        public void storeDbModel(string guid, int recordId, string tableName, string datasourceName, object modelContent) {
            string key = createCacheKey_forDbRecord(recordId, tableName, datasourceName);
            storeObject(key, modelContent);
            string keyPtr = createCachePtr_forDbRecord_guid(guid, tableName, datasourceName);
            storePtr(keyPtr, key);
        }
        //
        //====================================================================================================
        /// <summary>
        /// future method. To support a cpbase implementation, but wait until BaseModel is exposed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid"></param>
        /// <param name="recordId"></param>
        /// <param name="content"></param>
        public void storeDbModel<T>(string guid, int recordId, object content) where T : DbBaseModel {
            Type derivedType = this.GetType();
            FieldInfo fieldInfoTable = derivedType.GetField("tableNameLower");
            if (fieldInfoTable == null) {
                throw new GenericException("Class [" + derivedType.Name + "] must declare constant [contentTableName].");
            } else {
                string tableName = fieldInfoTable.GetRawConstantValue().ToString();
                FieldInfo fieldInfoDatasource = derivedType.GetField("contentDataSource");
                if (fieldInfoDatasource == null) {
                    throw new GenericException("Class [" + derivedType.Name + "] must declare public constant [contentDataSource].");
                } else {
                    string datasourceName = fieldInfoDatasource.GetRawConstantValue().ToString();
                    string key = createCacheKey_forDbRecord(recordId, tableName, datasourceName);
                    storeObject(key, content);
                    string keyPtr = createCachePtr_forDbRecord_guid(guid, tableName, datasourceName);
                    storePtr(keyPtr, key);
                }
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// set a key ptr. A ptr points to a normal key, creating an altername way to get/invalidate a cache.
        /// ex - image with id=10, guid={999}. The normal key="image/id/10", the alias Key="image/ccguid/{9999}"
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="keyPtr"></param>
        /// <param name="data"></param>
        /// <remarks></remarks>
        public void storePtr(string keyPtr, string key) {
            try {
                keyPtr = Regex.Replace(keyPtr, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                CacheDocumentClass cacheDocument = new CacheDocumentClass(core.dateTimeNowMockable) {
                    saveDate = core.dateTimeNowMockable,
                    invalidationDate = core.dateTimeNowMockable.AddDays(invalidationDaysDefault),
                    keyPtr = key
                };
                storeCacheDocument(keyPtr, cacheDocument);
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// invalidates the entire cache (except those entires written with saveRaw)
        /// </summary>
        /// <remarks></remarks>
        public void invalidateAll()  {
            try {
                string key = Regex.Replace(cacheNameGlobalInvalidationDate, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                storeCacheDocument(key, new CacheDocumentClass(core.dateTimeNowMockable) { saveDate = core.dateTimeNowMockable });
                _globalInvalidationDate = null;
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
        //====================================================================================================
        // <summary>
        // invalidates a tag
        // </summary>
        // <param name="tag"></param>
        // <remarks></remarks>
        public void invalidate(string key, int recursionLimit = 5) {
            try {
                Controllers.LogController.logTrace(core, "invalidate, key [" + key + "], recursionLimit [" + recursionLimit + "]");
                if ((recursionLimit > 0) && (!string.IsNullOrWhiteSpace(key.Trim()))) {
                    key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
                    // if key is a ptr, we need to invalidate the real key
                    CacheDocumentClass cacheDocument = getCacheDocument(key);
                    if (cacheDocument == null) {
                        // no cache for this key, if this is a dependency for another key, save invalidated
                        storeCacheDocument(key, new CacheDocumentClass(core.dateTimeNowMockable) { saveDate = core.dateTimeNowMockable });
                    } else {
                        if (!string.IsNullOrWhiteSpace(cacheDocument.keyPtr)) {
                            // this key is an alias, invalidate it's parent key
                            invalidate(cacheDocument.keyPtr, --recursionLimit);
                        } else {
                            // key is a valid cache, invalidate it
                            storeCacheDocument(key, new CacheDocumentClass(core.dateTimeNowMockable) { saveDate = core.dateTimeNowMockable });
                        }
                    }
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
        //========================================================================
        /// <summary>
        /// invalidate a cache for a single database record. If you know the table is in a content model, call the model's invalidateRecord. Use this when the content is a variable.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="recordId"></param>
        public void invalidateDbRecord(int recordId, string tableName, string dataSourceName = "default") {
            invalidate(createCacheKey_forDbRecord(recordId, tableName, dataSourceName));
        }
        //
        //====================================================================================================
        /// <summary>
        /// invalidates a list of keys 
        /// </summary>
        /// <param name="keyList"></param>
        /// <remarks></remarks>
        public void invalidate(List<string> keyList) {
            try {
                foreach (var key in keyList) {
                    invalidate(key);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
        //=======================================================================
        /// <summary>
        /// Convert a key to be used in the server cache. Normalizes name, adds app name and code version
        /// </summary>
        /// <param name="key">The cache key to be converted</param>
        /// <returns></returns>
        private string createServerKey(string key) {
            string result = core.appConfig.name + "-" + key;
            result = Regex.Replace(result, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return result;
        }
        /// <summary>
        /// return the standard key for Db records. 
        /// setObject for this key should only be the object model for this id
        /// getObject for this key should return null or an object of the model.
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="tableName"></param>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        public static string createCacheKey_forDbRecord(int recordId, string tableName, string dataSourceName) {
            string key = (String.IsNullOrWhiteSpace(dataSourceName)) ? "dbtable/default/" : "dbtable/" + dataSourceName.Trim() + "/";
            key += tableName.Trim() + "/id/" + recordId + "/";
            key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return key;
        }
        public static string createCacheKey_forDbRecord(int recordId, string tableName) => createCacheKey_forDbRecord(recordId, tableName, "default");
        /// <summary>
        /// return the standard key for cache ptr by Guid for Db records.  
        /// Only use this Ptr in setPtr. NEVER setObject for a ptr.
        /// getObject for this key should return null or an object of the model
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="tableName"></param>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        public static string createCachePtr_forDbRecord_guid(string guid, string tableName, string dataSourceName) {
            string key = "dbptr/" + dataSourceName + "/" + tableName + "/ccguid/" + guid + "/";
            key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return key;
        }
        //
        public static string createCachePtr_forDbRecord_guid(string guid, string tableName) => createCachePtr_forDbRecord_guid(guid, tableName, "default");
        /// <summary>
        /// return the standard key for cache ptr by name for Db records. 
        /// ONLY use this for tables where the name is unique.
        /// Only use this Ptr in setPtr. NEVER setObject for a ptr.
        /// getObject for this key should return a list of models that match the name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableName"></param>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        public static string createCachePtr_forDbRecord_uniqueName(string name, string tableName, string dataSourceName) {
            string key = "dbptr/" + dataSourceName + "/" + tableName + "/name/" + name + "/";
            key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return key;
        }
        //
        public static string createCachePtr_forDbRecord_uniqueName(string name, string tableName) => createCachePtr_forDbRecord_uniqueName(name, tableName, "default");
        //
        //====================================================================================================
        /// <summary>
        /// create a cache name for an object composed of data not from a single record
        /// </summary>
        /// <param name="objectName">The key that describes the object. This can be more general like "person" and used with a uniqueIdentities like "5"</param>
        /// <param name="objectUniqueIdentifier"></param>
        /// <returns></returns>
        public static string createCacheKey_forObject(string objectName, string objectUniqueIdentifier = "") {
            string key = "obj/" + objectName + "/" + objectUniqueIdentifier;
            key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return key;
        }
        //
        //====================================================================================================
        /// <summary>
        /// returns the system globalInvalidationDate. This is the date/time when the entire cache was last cleared. Every cache object saved before this date is considered invalid.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        private DateTime globalInvalidationDate {
            get {
                bool setDefault = false;
                if (_globalInvalidationDate == null) {
                    CacheDocumentClass dataObject = getCacheDocument(cacheNameGlobalInvalidationDate);
                    if (dataObject != null) {
                        _globalInvalidationDate = dataObject.saveDate;
                    }
                    if (_globalInvalidationDate == null) {
                        setDefault = true;
                    }
                }
                if (!_globalInvalidationDate.HasValue) {
                    setDefault = true;
                } else {
                    if ((encodeDate(_globalInvalidationDate)).CompareTo(new DateTime(1990, 8, 7)) < 0) {
                        setDefault = true;
                    }
                }
                if (setDefault) {
                    _globalInvalidationDate = new DateTime(1990, 8, 7);
                    storeCacheDocument(cacheNameGlobalInvalidationDate, new CacheDocumentClass(core.dateTimeNowMockable) { saveDate = encodeDate(_globalInvalidationDate) });
                }
                return encodeDate(_globalInvalidationDate);
            }
        }
        private DateTime? _globalInvalidationDate;
        //
        //====================================================================================================
        /// <summary>
        /// Initializes cache client
        /// </summary>
        /// <remarks></remarks>
        public CacheController(CoreController core) {
            try {
                this.core = core;
                //
                _globalInvalidationDate = null;
                remoteCacheInitialized = false;
                if (core.serverConfig.enableRemoteCache) {
                    //
                    // -- leave off, it causes a performance hit
                    if (core.serverConfig.enableEnyimNLog) { Enyim.Caching.LogManager.AssignFactory(new NLogFactory()); }
                    //
                    // -- initialize memcached drive (Enyim)
                    string cacheEndpoint = core.serverConfig.awsElastiCacheConfigurationEndpoint;
                    if (!string.IsNullOrEmpty(cacheEndpoint)) {
                        string[] cacheEndpointSplit = cacheEndpoint.Split(':');
                        int cacheEndpointPort = 11211;
                        if (cacheEndpointSplit.GetUpperBound(0) > 1) {
                            cacheEndpointPort = GenericController.encodeInteger(cacheEndpointSplit[1]);
                        }
                        Amazon.ElastiCacheCluster.ElastiCacheClusterConfig cacheConfig = new Amazon.ElastiCacheCluster.ElastiCacheClusterConfig(cacheEndpointSplit[0], cacheEndpointPort) {
                            Protocol = Enyim.Caching.Memcached.MemcachedProtocol.Binary
                        };
                        cacheClient = new Enyim.Caching.MemcachedClient(cacheConfig);
                        if (cacheClient != null) {
                            remoteCacheInitialized = true;
                        }
                    }
                }
            } catch (Exception ex) {
                //
                // -- client does not throw its own errors, so try to differentiate by message
                throw (new GenericException("Exception initializing remote cache, will continue with cache disabled.", ex));
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// save object directly to cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheDocument">Either a string, a date, or a serializable object</param>
        /// <param name="invalidationDate"></param>
        /// <remarks></remarks>
        private void storeCacheDocument(string key, CacheDocumentClass cacheDocument) {
            try {
                //
                if (string.IsNullOrEmpty(key)) {
                    throw new ArgumentException("cache key cannot be blank");
                }
                string typeMessage = "";
                string serverKey = createServerKey(key);
                if (core.serverConfig.enableLocalMemoryCache) {
                    //
                    // -- save local memory cache
                    typeMessage = "local-memory";
                    storeCacheDocument_MemoryCache(serverKey, cacheDocument);
                }
                if (core.serverConfig.enableLocalFileCache) {
                    //
                    // -- save local file cache
                    typeMessage = "local-file";
                    string serializedData = SerializeObject(cacheDocument);
                    using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, serverKey)) {
                        mutex.WaitOne();
                        core.privateFiles.saveFile("appCache\\" + FileController.encodeDosFilename(serverKey + ".txt"), serializedData);
                        mutex.ReleaseMutex();
                    }
                }
                if (core.serverConfig.enableRemoteCache) {
                    typeMessage = "remote";
                    if (remoteCacheInitialized) {
                        //
                        // -- save remote cache
                        if( !cacheClient.Store(Enyim.Caching.Memcached.StoreMode.Set, serverKey, cacheDocument, cacheDocument.invalidationDate)) {
                            //
                            // -- store failed
                            LogController.logError(core, "Enyim cacheClient.Store failed, no details available.");
                        }
                    }
                }
                //
                LogController.logTrace(core, "cacheType [" + typeMessage + "], key [" + key + "], expires [" + cacheDocument.invalidationDate + "], depends on [" + string.Join(",", cacheDocument.dependentKeyList) + "], points to [" + string.Join(",", cacheDocument.keyPtr) + "]");
                //
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// save cacheDocument to memory cache
        /// </summary>
        /// <param name="serverKey">key converted to serverKey with app name and code version</param>
        /// <param name="cacheDocument"></param>
        public void storeCacheDocument_MemoryCache(string serverKey, CacheDocumentClass cacheDocument) {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy {
                AbsoluteExpiration = cacheDocument.invalidationDate // core.dateTimeMockable.AddMinutes(100);
            };
            cache.Set(serverKey, cacheDocument, policy);
        }
        //
        //====================================================================================================
        /// <summary>
        /// update the cache object that holds the last modified record for any record in this table
        /// Cache objects that want to be automatically invalidated if any record in a table is updated can add a dependency on this key
        /// </summary>
        /// <param name="core"></param>
        public void store_LastRecordModifiedDate(string tableName) {
            storeObject(createCacheKey_LastRecordModifiedDate(tableName), core.dateTimeNowMockable);
        }
        //
        //========================================================================
        /// <summary>
        /// invalidate all cache entries that include data from this table.
        /// when a cacheDocument is saved, it should include a dependancy on key = createCacheKey_TableObjectsInvalidationDate(tablename)
        /// call this method after running a query that updates records that cannot be individually invalidated (like all records created before a specific date, etc.)
        /// </summary>
        /// <param name="dbTableName"></param>
        public void invalidateTableObjects(string dbTableName) {
            storeObject(createCacheKey_TableObjectsInvalidationDate(dbTableName), core.dateTimeNowMockable);
        }
        //
        //====================================================================================================
        /// <summary>
        /// create a key for this table that holds the date of before which all objects created with any data from this table should be invalidated
        /// see CacheController header for more explaination
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string createCacheKey_TableObjectsInvalidationDate(string tableName, string dataSourceName) {
            string key = "tableobjectsinvalidationdate/" + ((String.IsNullOrWhiteSpace(dataSourceName)) ? "default/" + tableName.Trim().ToLowerInvariant() + "/" : dataSourceName.Trim().ToLowerInvariant() + "/" + tableName.Trim().ToLowerInvariant() + "/");
            key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return key;
        }
        //
        //====================================================================================================
        //
        public static string createCacheKey_TableObjectsInvalidationDate(string tableName) => createCacheKey_TableObjectsInvalidationDate(tableName, "default");
        //
        //====================================================================================================
        //
        public static string createCacheKey_LastRecordModifiedDate(string tableName, string dataSourceName) {
            string key = "lastrecordmodifieddate/" + ((String.IsNullOrWhiteSpace(dataSourceName)) ? "default/" + tableName.Trim().ToLowerInvariant() + "/" : dataSourceName.Trim().ToLowerInvariant() + "/" + tableName.Trim().ToLowerInvariant() + "/");
            key = Regex.Replace(key, "0x[a-fA-F\\d]{2}", "_").ToLowerInvariant().Replace(" ", "_");
            return key;
        }
        //
        //====================================================================================================
        //
        public static string createCacheKey_LastRecordModifiedDate(string tableName) => createCacheKey_LastRecordModifiedDate(tableName, "default");
        //
        //====================================================================================================
        //
        private static TData DeserializeFromString<TData>(string settings) {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b)) {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }
        //
        //====================================================================================================
        //
        private static string SerializeToString<TData>(TData settings) {
            using (var stream = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
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
        ~CacheController()  {
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
                    if (cacheClient != null) {
                        cacheClient.Dispose();
                    }
                    //
                    // cleanup managed objects
                }
                //
                // cleanup non-managed objects
            }
        }
        #endregion
    }
    //
}