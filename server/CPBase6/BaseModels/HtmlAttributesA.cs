
namespace Contensive.CPBase.BaseModels {
    /// <summary>
    /// Attributes for html form
    /// </summary>
    public class HtmlAttributesA : HtmlAttributesGlobal {
        /// <summary>
        /// Specifies that the target will be downloaded when a user clicks on the hyperlink
        /// </summary>
        public string download;
        /// <summary>
        /// Specifies the URL of the page the link goes to
        /// </summary>
        public string href;
        /// <summary>
        /// Specifies the language of the linked document
        /// </summary>
        public string hreflang;
        /// <summary>
        /// Specifies what media/device the linked document is optimized for
        /// </summary>
        public string media;
        /// <summary>
        /// Specifies a space-separated list of URLs to which, when the link is followed, post requests with the body ping will be sent by the browser (in the background). Typically used for tracking
        /// </summary>
        public string ping;
        /// <summary>
        /// Specifies which referrer to send
        /// </summary>
        public HtmlAttributeReferrerPolicy referrerpolicy;
        /// <summary>
        /// 
        /// </summary>
        public enum HtmlAttributeReferrerPolicy {
            none = 0,
            no_referrer = 1,
            no_referrer_when_downgrade = 2,
            origin = 3,
            origin_when_cross_origin = 4,
            unsafe_url = 5
        }
        /// <summary>
        /// Specifies the relationship between the current document and the linked document
        /// </summary>
        public HtmlAttributeRel rel;
        /// <summary>
        /// values for html form encodetype
        /// </summary>
        public enum HtmlAttributeRel {
            none = 0,
            alternate = 1,
            author = 2,
            bookmark = 3,
            external = 4,
            help = 5,
            license = 6,
            next = 7,
            nofollow = 8,
            noreferrer = 9,
            noopener = 10,
            prev = 11,
            search = 12,
            tag = 13,
        }
        /// <summary>
        /// Specifies where to open the linked document
        /// </summary>
        public string target;
        /// <summary>
        /// Specifies the media type of the linked document
        /// </summary>
        public string type;
    }
}
