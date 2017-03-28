using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    public class GreenException : ApplicationException
    {
        public GreenException()
            : base()
        { }

        public GreenException(string message)
            : base(message)
        { }

        public GreenException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected GreenException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }
}
