using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class HttpException : Exception
    {
        public int StatusCode { get; set; }

        public HttpException() : base() { }
        public HttpException(string message) : base(message) { }
        public HttpException(string message, System.Exception inner) : base(message, inner) { }

        public HttpException(int code, string message) : base(message)
        {
            StatusCode = code;
        }
    }
}
