
using System;

namespace Contensive.Processor.Exceptions {
    /// <summary>
    /// Non-specific exception. Used during code conversion. Do not add to future code.
    /// </summary>
    public class GenericException : ApplicationException {
        public GenericException(string message) : base(message) { }
        public GenericException(string message, Exception innerException) : base(message, innerException) { }
    }
    // todo - create custom exceptions for each case that must be caught uniquely, and each case exposed to consumers
}
