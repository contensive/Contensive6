
using Contensive.Processor.Controllers;
//
namespace Contensive.Processor.Models.Domain {
    [System.Serializable]
    public class _BlankModel {
        private readonly CoreController core;
        //
        //====================================================================================================
        /// <summary>
        /// new
        /// </summary>
        /// <param name="core"></param>
        public _BlankModel(CoreController core) {
            this.core = core;
        }
        //
        //
        //
    }
}