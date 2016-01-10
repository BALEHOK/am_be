using System;

namespace AppFramework.Reports.Exceptions
{
    class InvalidReportParameters : ArgumentException
    {
        public InvalidReportParameters(string message)
            : base(message)
        {

        }

    }
}
