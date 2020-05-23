
namespace Contensive.Models.Db {
    [System.Serializable]
    public class LayoutModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("layouts", "cclayouts", "default", true);
        //
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        public FieldTypeTextFile layout { get; set; }
        /// <summary>
        /// the addon collection that installed this record
        /// </summary>
        public int installedByCollectionId { get; set; }
        /// <summary>
        /// deprecated. styles are implemented only through addons
        /// </summary>
        public string stylesFilename { get; set; }
    }
}
