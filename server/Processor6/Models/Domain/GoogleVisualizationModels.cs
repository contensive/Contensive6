
using System.Collections.Generic;
//
namespace Contensive.Processor.Models.Domain {
    //
    // Google Data Object construction in GetRemoteQuery
    //
    [System.Serializable]
    public class ColsType {
        public string Type;
        public string Id;
        public string Label;
        public string Pattern;
    }
    //
    [System.Serializable]
    public class CellType {
        public string v;
        public string f;
        public string p;
    }
    //
    [System.Serializable]
    public class RowsType {
        public List<CellType> Cell;
    }
    //
    [System.Serializable]
    public class GoogleDataType {
        public bool IsEmpty;
        public List<ColsType> col;
        public List<RowsType> row;
    }
    //
    [System.Serializable]
    public enum GoogleVisualizationStatusEnum {
        OK = 1,
        warning = 2,
        ErrorStatus = 3
    }
    //
    [System.Serializable]
    public class GoogleVisualizationType {
        public string version;
        public string reqid;
        public GoogleVisualizationStatusEnum status;
        public string[] warnings;
        public string[] errors;
        public string sig;
        public GoogleDataType table;
    }

}