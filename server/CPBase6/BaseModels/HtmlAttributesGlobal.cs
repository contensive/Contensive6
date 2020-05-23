
using System.Collections.Generic;

namespace Contensive.CPBase.BaseModels {
    /// <summary>
    /// Attributes avalable for all html5 elements
    /// </summary>
    public partial class HtmlAttributesGlobal {
        /// <summary>
        /// Specifies a shortcut key to activate/focus an element
        /// </summary>
        public string accesskey;
        /// <summary>
        /// Specifies one or more classnames for an element (refers to a class in a style sheet)
        /// </summary>
        public string @class;
        /// <summary>
        /// Specifies whether the content of an element is editable or not
        /// </summary>
        public bool contenteditable;
        /// <summary>
        /// Used to store custom data private to the page or application
        /// </summary>
        public List<KeyValuePair<string, string>> data;
        /// <summary>
        /// Specifies the text direction for the content in an element
        /// </summary>
        public string dir;
        /// <summary>
        /// Specifies whether an element is draggable or not
        /// </summary>
        public bool draggable;
        /// <summary>
        /// Specifies whether the dragged data is copied, moved, or linked, when dropped
        /// </summary>
        public string dropzone;
        /// <summary>
        /// Specifies that an element is not yet, or is no longer, relevant
        /// </summary>
        public bool hidden;
        /// <summary>
        /// Specifies a unique id for an element
        /// </summary>
        public string id;
        /// <summary>
        /// Specifies the language of the element's content
        /// </summary>
        public string lang;
        /// <summary>
        /// Specifies whether the element is to have its spelling and grammar checked or not
        /// </summary>
        public bool spellcheck;
        /// <summary>
        /// Specifies an inline CSS style for an element
        /// </summary>
        public string style;
        /// <summary>
        /// Specifies the tabbing order of an element
        /// </summary>
        public string tabindex;
        /// <summary>
        /// Specifies extra information about an element
        /// </summary>
        public string title;
        /// <summary>
        /// Specifies whether the content of an element should be translated or not
        /// </summary>
        public bool translate;
    }
}
