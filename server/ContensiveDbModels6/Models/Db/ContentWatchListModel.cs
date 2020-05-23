
namespace Contensive.Models.Db {
    [System.Serializable]
    public class ContentWatchListModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Content Watch Lists", "ccContentWatchLists", "default", false);
        //
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
    }
}
