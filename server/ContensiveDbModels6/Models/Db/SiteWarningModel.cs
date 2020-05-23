
using System;

namespace Contensive.Models.Db {
    [System.Serializable]
    public class SiteWarningModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("content", "table", "default", false);
        //
        //====================================================================================================
        //
        public int count { get; set; }
        public DateTime dateLastReported { get; set; }
        public string description { get; set; }
        public string generalKey { get; set; }
        public string location { get; set; }
        public int pageId { get; set; }
        public string shortDescription { get; set; }
        public string specificKey { get; set; }
    }
}
