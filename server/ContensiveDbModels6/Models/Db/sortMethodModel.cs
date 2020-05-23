
namespace Contensive.Models.Db {
    [System.Serializable]
    public class SortMethodModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("sort methods", "ccSortMethods", "default", true);
        //
        //====================================================================================================
        public string orderByClause { get; set; }
    }
}
