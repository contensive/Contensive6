
namespace Contensive.Models.Db {
    [System.Serializable]
    public class MetaKeywordRuleModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Meta Keyword Rules", "ccMetaKeywordRules", "default", false);
        //
        //====================================================================================================
        //
        public int metaContentId { get; set; }
        public int metaKeywordId { get; set; }
    }
}
