
namespace Contensive.Models.Db {
    [System.Serializable]
    public class LibraryFolderModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Library Folders", "ccLibraryFolders", "default", false);
        //
        //====================================================================================================
        //
        public string description { get; set; }
        public int parentId { get; set; }
    }
}
