
using System;
using System.Collections.Generic;
using Contensive.Models.Db;
using Contensive.Processor.Controllers;
using Contensive.Processor.Exceptions;
//
namespace Contensive.Processor.Models.Domain {
    /// <summary>
    /// Dictionary of Routes
    /// </summary>
    [Serializable]
    public class RouteMapModel {
        /// <summary>
        /// cache object name
        /// </summary>
        private const string cacheNameRouteMap = "RouteMapModel";
        //
        //====================================================================================================
        /// <summary>
        /// model for stored route
        /// </summary>
        [Serializable]
        public class RouteClass {
            public string virtualRoute;
            public string physicalRoute;
            public RouteTypeEnum routeType;
            public int remoteMethodAddonId;
            public int linkAliasId;
            public int linkForwardId;
        }
        //
        //====================================================================================================
        /// <summary>
        /// Types of routes stored
        /// </summary>
        [Serializable]
        public enum RouteTypeEnum {
            admin,
            remoteMethod,
            linkAlias,
            linkForward
        }
        //
        //====================================================================================================
        /// <summary>
        /// The date and time when this route dictionary was created. Used by iis app to detect if the route table needs to be updated.
        /// </summary>
        public DateTime dateCreated { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// public dictionary of routes in the model
        /// </summary>
        public Dictionary<string, RouteClass> routeDictionary;
        //
        //===================================================================================================
        /// <summary>
        /// Create a list of routes
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        public static RouteMapModel create(CoreController core) {
            RouteMapModel result = new RouteMapModel();
            try {
                result = getCache(core);
                if (result == null) {
                    result = new RouteMapModel {
                        dateCreated = core.dateTimeNowMockable,
                        routeDictionary = new Dictionary<string, RouteClass>()
                    };
                    string physicalFile = "~/" + core.siteProperties.serverPageDefault;
                    //
                    // -- admin route
                    string adminRoute = GenericController.normalizeRoute(core.appConfig.adminRoute);
                    if (!string.IsNullOrWhiteSpace(adminRoute)) {
                        result.routeDictionary.Add(adminRoute, new RouteClass {
                            physicalRoute = physicalFile,
                            virtualRoute = adminRoute,
                            routeType = RouteTypeEnum.admin
                        });
                    }
                    //
                    // -- remote methods
                    foreach (var remoteMethod in core.addonCache.getRemoteMethodAddonList()) {
                        string route = GenericController.normalizeRoute(remoteMethod.name);
                        if (!string.IsNullOrWhiteSpace(route)) {
                            if (result.routeDictionary.ContainsKey(route)) {
                                LogController.logWarn(core, new GenericException("Route [" + route + "] cannot be added because it is a matches the Admin Route or another Remote Method."));
                            } else {
                                result.routeDictionary.Add(route, new RouteClass {
                                    physicalRoute = physicalFile,
                                    virtualRoute = route,
                                    routeType = RouteTypeEnum.remoteMethod,
                                    remoteMethodAddonId = remoteMethod.id
                                });
                            }
                        }
                    }
                    //
                    // -- link forwards
                    foreach (var linkForward in DbBaseModel.createList<LinkForwardModel>(core.cpParent, "name Is Not null")) {
                        string route = GenericController.normalizeRoute(linkForward.sourceLink);
                        if (!string.IsNullOrEmpty(route)) {
                            if (result.routeDictionary.ContainsKey(route)) {
                                LogController.logError( core,new GenericException("Link Forward Route [" + route + "] cannot be added because it is a matches the Admin Route, a Remote Method or another Link Forward."));
                            } else {
                                result.routeDictionary.Add(route, new RouteClass {
                                    physicalRoute = physicalFile,
                                    virtualRoute = route,
                                    routeType = RouteTypeEnum.linkForward,
                                    linkForwardId = linkForward.id
                                });
                            }
                        }
                    }
                    //
                    // -- link aliases
                    foreach (var linkAlias in DbBaseModel.createList<LinkAliasModel>(core.cpParent, "name Is Not null")) {
                        string route = GenericController.normalizeRoute(linkAlias.name);
                        if (!string.IsNullOrEmpty(route)) {
                            if (result.routeDictionary.ContainsKey(route)) {
                                LogController.logError( core,new GenericException("Link Alias route [" + route + "] cannot be added because it is a matches the Admin Route, a Remote Method, a Link Forward o another Link Alias."));
                            } else {
                                result.routeDictionary.Add(route, new RouteClass {
                                    physicalRoute = physicalFile,
                                    virtualRoute = route,
                                    routeType = RouteTypeEnum.linkAlias,
                                    linkAliasId = linkAlias.id
                                });
                            }
                        }
                    }
                    setCache(core, result);
                }
            } catch (Exception ex) {
                LogController.logError( core,ex);
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// save the model afer it is created. Depends on addons, linkAlias and LinkForwards
        /// </summary>
        /// <param name="core"></param>
        /// <param name="routeDictionary"></param>
        private static void setCache(CoreController core, RouteMapModel routeDictionary) {
            var dependentKeyList = new List<string> {
                CacheController.createCacheKey_TableObjectsInvalidationDate(AddonModel.tableMetadata.tableNameLower),
                CacheController.createCacheKey_TableObjectsInvalidationDate(LinkAliasModel.tableMetadata.tableNameLower),
                CacheController.createCacheKey_TableObjectsInvalidationDate(LinkForwardModel.tableMetadata.tableNameLower)
            };
            core.cache.storeObject(cacheNameRouteMap, routeDictionary,dependentKeyList);
        }
        //
        //====================================================================================================
        /// <summary>
        /// load the model from cache. returns null if cache not valid
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        private static RouteMapModel getCache(CoreController core) {
            return core.cache.getObject<RouteMapModel>(cacheNameRouteMap);
        }
        //
        //====================================================================================================
        /// <summary>
        /// invalidate cache if anything is modified
        /// </summary>
        /// <param name="core"></param>
        public static void invalidateCache(CoreController core) {
            core.cache.invalidate(cacheNameRouteMap);
        }
    }
}

