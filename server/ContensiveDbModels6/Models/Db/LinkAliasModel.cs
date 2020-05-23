
using Contensive.BaseClasses;
using System.Collections.Generic;

namespace Contensive.Models.Db {
    [System.Serializable]
    public class LinkAliasModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("link aliases", "cclinkaliases", "default", true);
        //
        //====================================================================================================
        public int pageId { get; set; }
        public string queryStringSuffix { get; set; }
        //
        //====================================================================================================
        public static List<LinkAliasModel> createPageList(CPBaseClass cp, int pageId, string queryStringSuffix) {
            if (string.IsNullOrEmpty(queryStringSuffix)) {
                return createList<LinkAliasModel>(cp, "(pageId=" + pageId + ")and((QueryStringSuffix='')or(QueryStringSuffix is null))", "id desc");
            } else {
                return createList<LinkAliasModel>(cp, "(pageId=" + pageId + ")and(QueryStringSuffix=" + cp.Db.EncodeSQLText(queryStringSuffix) + ")", "id desc");
            }
        }
        //
        //====================================================================================================
        public static List<LinkAliasModel> createPageList(CPBaseClass cp, int pageId)
            => createPageList(cp, pageId, string.Empty);
    }
}
