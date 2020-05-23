
namespace Contensive.Processor.Models.Domain {
    //
    //====================================================================================================
    [System.Serializable]
    public class StylesheetContextModel {
        public int templateId { get; set; }
        public int emailId { get; set; }
        public string styleSheet { get; set; }
    }
}