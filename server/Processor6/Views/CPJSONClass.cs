
using System;

namespace Contensive.Processor {
    public class CPJSONClass : BaseClasses.CPJSONBaseClass {
        //
        // ====================================================================================================
        //
        public override string Serialize(object obj) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        //
        // ====================================================================================================
        //
        public override T Deserialize<T>(string JSON) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JSON);
        }
   }
}