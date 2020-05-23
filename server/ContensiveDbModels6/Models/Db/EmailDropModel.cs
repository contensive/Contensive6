

namespace Contensive.Models.Db {
    [System.Serializable]
    public class EmailDropModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Email Drops", "ccemaildrops", "default", false);
        //
        //====================================================================================================
        public int emailId { get; set; }
    }
}
