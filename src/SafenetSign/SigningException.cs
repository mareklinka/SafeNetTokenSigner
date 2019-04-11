using System;

namespace SafenetSign
{
    public sealed class SigningException : Exception
    {
        public SigningException(string message) : base(message)
        {
        }

        public SigningException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
