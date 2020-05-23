
using System.Collections.Generic;
//
namespace Contensive.Processor.Models.Domain {
    [System.Serializable]
    internal class PageFormFieldModel {
        internal int type { get; set; }
        internal string caption { get; set; }
        internal bool required { get; set; }
        internal string peopleFieldName { get; set; }
        internal string groupName { get; set; }
    }
}