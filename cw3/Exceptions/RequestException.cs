using System;

namespace cw3.Exceptions
{
    public class RequestException : Exception
    {
        public int Code { get; set; }

        public RequestException(string message, int code) : base(message) => Code = code;

        public RequestException(string message) : base(message)
        {

        }
        
    }
}
