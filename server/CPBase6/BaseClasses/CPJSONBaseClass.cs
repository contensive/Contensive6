
namespace Contensive.BaseClasses {
    public abstract class CPJSONBaseClass {
        //
        //==========================================================================================
        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="groupName"></param>
        public abstract string Serialize(object obj);
        //
        //==========================================================================================
        /// <summary>
        /// Deserialize a
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public abstract T Deserialize<T>(string JSON);
    }
}

