
namespace Contensive.Models.Db {
    [System.Serializable]
    public class AddonCategoryModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("add-on categories", "ccaddoncategories", "default", true);
        //
        //====================================================================================================
        /// <summary>
        /// field properties
        /// </summary>
    }
}
