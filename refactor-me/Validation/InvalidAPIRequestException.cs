using System;

namespace ProductAPI.Validation
{
    public class InvalidAPIRequestException : Exception
    {
        public InvalidAPIRequestException() { }

        public InvalidAPIRequestException(string message) : base(message) { }
    }
}