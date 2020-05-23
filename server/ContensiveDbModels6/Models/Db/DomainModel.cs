
using Contensive.BaseClasses;
using System.Collections.Generic;

namespace Contensive.Models.Db {
    [System.Serializable]
    public class DomainModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("domains", "ccdomains", "default", true);
        //
        //====================================================================================================
        /// <summary>
        /// the template used for this domain. Can be overridden by page
        /// </summary>
        public int defaultTemplateId { get; set; }
        /// <summary>
        /// forward traffic to this domain to another domain
        /// </summary>
        public int forwardDomainId { get; set; }
        /// <summary>
        /// forward traffic to this url
        /// </summary>
        public string forwardUrl { get; set; }
        /// <summary>
        /// set response header to noFollow for this domain
        /// </summary>
        public bool noFollow { get; set; }
        /// <summary>
        /// for this domain, display this page not found
        /// </summary>
        public int pageNotFoundPageId { get; set; }
        /// <summary>
        /// for this domain, the home/landing page
        /// </summary>
        public int rootPageId { get; set; }
        /// <summary>
        /// determines the type of response
        /// </summary>
        public int typeId { get; set; }
        /// <summary>
        /// true if this domain has received traffic
        /// </summary>
        public bool visited { get; set; }
        /// <summary>
        /// the default code to execute for this domain
        /// </summary>
        public int defaultRouteId { get; set; }
        /// <summary>
        /// if true, add the default CORS headers
        /// </summary>
        public bool allowCORS { get; set; }
        //
        //====================================================================================================
        public static Dictionary<string, DomainModel> createDictionary(CPBaseClass cp, string sqlCriteria) {
            var result = new Dictionary<string, DomainModel> { };
            foreach (var domain in DbBaseModel.createList<DomainModel>(cp, sqlCriteria)) {
                if (!result.ContainsKey(domain.name.ToLowerInvariant())) {
                    result.Add(domain.name.ToLowerInvariant(), domain);
                }
            }
            return result;
        }
        //
        public enum DomainTypeEnum {
            Normal = 1,
            ForwardToUrl = 2,
            ForwardToReplacementDomain = 3
        }



    }

}
