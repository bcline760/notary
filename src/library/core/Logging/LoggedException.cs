using System;
using System.Collections.Generic;
using System.Text;

namespace Notary.Logging
{
    public class LoggedException : Exception
    {
        public LoggedException(string message) : base(message) { }

        public LoggedException(string message, Exception ex) : base(message, ex) { }
    }
}
