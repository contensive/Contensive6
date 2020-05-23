
namespace Contensive.Models.Db {
    [System.Serializable]
    public class ActivityLogModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Activity Logs", "ccActivityLogs", "default", false);
        //
        //====================================================================================================
        //
        public string link { get; set; }
        public int memberId { get; set; }
        public string message { get; set; }
        public int organizationId { get; set; }
        public int visitId { get; set; }
        public int visitorId { get; set; }
    }
}
