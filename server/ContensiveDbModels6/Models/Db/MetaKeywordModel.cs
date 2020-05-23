
namespace Contensive.Models.Db {
    [System.Serializable]
    public class MetaKeywordModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Meta Keywords", "ccMetaKeywords", "default", false);
        //
        //====================================================================================================
        //
    }
}
