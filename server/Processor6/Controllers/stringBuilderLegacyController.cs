//
using System.Text;
//
namespace Contensive.Processor.Controllers {
    public class StringBuilderLegacyController {
        private readonly StringBuilder builder = new StringBuilder();
        public void add(string NewString) {
            builder.Append(NewString);
        }
        public string text {
            get {
                return builder.ToString();
            }
        }
    }
}