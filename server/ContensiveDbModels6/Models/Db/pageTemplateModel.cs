
namespace Contensive.Models.Db {
    [System.Serializable]
    public class PageTemplateModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("page templates", "cctemplates", "default", true);
        //
        //====================================================================================================
        public string addonList { get; set; }
        public string bodyHTML { get; set; }
        public bool isSecure { get; set; }
        public int collectionId { get; set; }
    }
}
