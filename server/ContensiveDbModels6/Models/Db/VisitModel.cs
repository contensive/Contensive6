
using System;
using System.Linq;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.Models.Db {
    [System.Serializable]
    public class VisitModel : DbBaseModel {
        //
        //====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("visits", "ccvisits", "default", false);
        //
        //====================================================================================================
        public bool bot { get; set; }
        public string browser { get; set; }
        
        public bool cookieSupport { get; set; }
        public bool excludeFromAnalytics { get; set; }
        //public string http_from { get; set; }
        public string http_referer { get; set; }
        //public string http_via { get; set; }
        public DateTime? lastVisitTime { get; set; }
        public int loginAttempts { get; set; }
        public int memberId { get; set; }
        public bool memberNew { get; set; }
        public bool mobile { get; set; }
        public int pageVisits { get; set; }
        public string refererPathPage { get; set; }
        public string remote_addr { get; set; }
        //public string remoteName { get; set; }
        public int startDateValue { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? stopTime { get; set; }
        public int timeToLastHit { get; set; }
        public bool verboseReporting { get; set; }
        public bool visitAuthenticated { get; set; }
        public int visitorId { get; set; }
        public bool visitorNew { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// return a visit object for the visitor's last visit before the provided id
        /// </summary>
        /// <param name="core"></param>
        /// <param name="visitId"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public static VisitModel getLastVisitByVisitor(CPBaseClass cp, int visitId, int visitorId) {
            var visitList = DbBaseModel.createList<VisitModel>(cp, "(id<>" + visitId + ")and(VisitorID=" + visitorId + ")", "id desc");
            if ( visitList.Count>0) {
                return visitList.First();
            }
            return null;
        }
    }
}
