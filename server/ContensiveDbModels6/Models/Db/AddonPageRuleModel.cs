
namespace Contensive.Models.Db {
    [System.Serializable]
    public class AddonPageRuleModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("add-on page rules", "ccaddonpagerules", "default", false);
        //
        //====================================================================================================
        /// <summary>
        /// field properties
        /// </summary>
        public int addonId { get; set; }
        public int pageId { get; set; }
    }
}
