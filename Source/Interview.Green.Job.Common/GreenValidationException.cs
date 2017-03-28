using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Common
{
    public class GreenValidationException : GreenException
    {
        public string Parameter { get; set; }

        public GreenValidationException(string parameter, string message)
            : base(message)
        {
            Parameter = parameter;
        }

        public GreenValidationException(string parameter, string message, Exception innerException)
            : base(message, innerException)
        {
            Parameter = parameter;
        }
    }
}
