
//
using Contensive.Processor.Controllers;

namespace Contensive.Processor.Models.Domain {
    //
    //====================================================================================================
    //
    public class DocPropertyModel {
        public string name { get; set; }
        public string value { get; set; }
        public string nameValue { get; set; }
        public string tempfilename { get; set; }
        public int fileSize { get; set; }
        public string fileType { get; set; }
        public DocPropertyTypesEnum propertyType { get; set; }
        //
        public enum DocPropertyTypesEnum {
            serverVariable = 1,
            header = 2,
            form = 3,
            file = 4,
            queryString = 5,
            userDefined = 6
        }
    }
}