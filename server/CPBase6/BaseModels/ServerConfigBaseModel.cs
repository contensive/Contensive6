
using System;

namespace Contensive.BaseModels {
    //
    //====================================================================================================
    /// <summary>
    /// configuration of the server (on or more apps in the serer)
    /// -- new() - to allow deserialization (so all methods must pass in cp)
    /// -- shared getObject( cp, id ) - returns loaded model
    /// -- saveObject( cp ) - saves instance properties, returns the record id
    /// </summary>
    [Serializable]
    public abstract class ServerConfigBaseModel {
        /// <summary>
        /// full dos path to the contensive program file installation. 
        /// </summary>
        public abstract string programFilesPath { get; set; }
        //
        /// <summary>
        /// control the task runner and task scheduler for the server group
        /// </summary>
        public abstract bool allowTaskRunnerService { get; set; }
        /// <summary>
        /// control the task runner and task scheduler for the server group
        /// </summary>
        public abstract bool allowTaskSchedulerService { get; set; }
        /// <summary>
        /// control the task runner and task scheduler for the server group
        /// </summary>
        public abstract int maxConcurrentTasksPerServer { get; set; }
        //
        /// <summary>
        /// name for this server group
        /// </summary>
        public abstract string name { get; set; }
        //
        /// <summary>
        /// If true, use local dotnet memory cache backed by filesystem
        /// </summary>
        public abstract bool enableLocalMemoryCache { get; set; }
        //
        /// <summary>
        /// if true, used local files to cache, backing up local cache, then remote cache
        /// </summary>
        public abstract bool enableLocalFileCache { get; set; }
        //
        /// <summary>
        /// AWS dotnet elaticcache client wraps enyim, and provides node autodiscovery through the configuration object. this is the srver:port to the config file it uses.
        /// </summary>
        public abstract bool enableRemoteCache { get; set; }
        /// <summary>
        /// AWS dotnet elaticcache client wraps enyim, and provides node autodiscovery through the configuration object. this is the srver:port to the config file it uses.
        /// </summary>
        public abstract string awsElastiCacheConfigurationEndpoint { get; set; }
        //
        /// <summary>
        /// includes NLog logging into Enyim. Leave off as it causes performance issues
        /// </summary>
        public abstract bool enableEnyimNLog { get; set; }
        //
        /// <summary>
        /// datasource for the cluster (only sql support for now)
        /// </summary>
        public abstract DataSourceTypeEnum defaultDataSourceType { get; set; }
        /// <summary>
        /// datasource for the cluster (only sql support for now)
        /// </summary>
        public abstract string defaultDataSourceAddress { get; set; }
        /// <summary>
        /// datasource for the cluster (only sql support for now)
        /// </summary>
        public abstract string defaultDataSourceUsername { get; set; }
        /// <summary>
        /// datasource for the cluster (only sql support for now)
        /// </summary>
        public abstract string defaultDataSourcePassword { get; set; }
        /// <summary>
        /// If true, the connection will be forced secure
        /// </summary>
        public abstract bool defaultDataSourceSecure { get; set; }
        //
        /// <summary>
        /// aws programmatic user for all services
        /// </summary>
        public abstract string awsAccessKey { get; set; }
        /// <summary>
        /// aws programmatic user for all services
        /// </summary>
        public abstract string awsSecretAccessKey { get; set; }
        //
        /// <summary>
        /// aws region for this server (default us-east-1)
        /// </summary>
        public abstract string awsRegionName { get; set; }
        //
        /// <summary>
        /// endpoint for cluster files (not sure how it works, maybe this will be an object taht includes permissions, for now an fpo)
        /// </summary>
        public abstract bool isLocalFileSystem { get; set; }
        /// <summary>
        /// endpoint for cluster files (not sure how it works, maybe this will be an object taht includes permissions, for now an fpo)
        /// </summary>
        public abstract string localDataDriveLetter { get; set; }
        /// <summary>
        /// endpoint for cluster files (not sure how it works, maybe this will be an object taht includes permissions, for now an fpo)
        /// </summary>
        public abstract string awsBucketName { get; set; }
        //
        /// <summary>
        /// if provided, NLog data will be sent to this CloudWatch LogGroup 
        /// </summary>
        public abstract string awsCloudWatchLogGroup { get; set; }
        //
        /// <summary>
        /// used by applications to enable/disable features, like ecommerce batch should only run in production, todo figure out how to expose this, add it to configuration setup
        /// </summary>
        public abstract bool productionEnvironment { get; set; }
        //
        public enum DataSourceTypeEnum {
            legacy = 1,
            sqlServer = 2
        }
    }
}

