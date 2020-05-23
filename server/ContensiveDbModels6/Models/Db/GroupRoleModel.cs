
using System;

namespace Contensive.Models.Db {
    [System.Serializable]
    public class GroupRoleModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Group Roles", "ccgrouproles", "default", false);
        //
        //====================================================================================================
    }
}
