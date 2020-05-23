
namespace Contensive.Models.Db {
    [System.Serializable]
    public class AddonEventThrowerModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Add-on Event Throwers", "ccAddonEventThrowers", "default", false);
        //
        //====================================================================================================
        //
        public int addonId { get; set; }
        public int eventId { get; set; }
    }
}
