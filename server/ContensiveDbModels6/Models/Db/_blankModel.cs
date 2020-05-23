
namespace Contensive.Models.Db {
    [System.Serializable]
    public class _blankModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("content", "table", "default", false);
        //
        //====================================================================================================
        //
    }
}
