
using System.Collections.Generic;

namespace Contensive.Models.Db {
    [System.Serializable]
    public class EmailQueueModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("email queue", "ccemailqueue", "default", false);
        //
        //====================================================================================================
        public string toAddress { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public bool immediate { get; set;  }
        public int attempts { get; set; }
    }
}
