using System;

namespace Unity.Exceptions
{
    internal class InvalidRegistrationException : Exception
    {
        public InvalidRegistrationException()
            : base()
        {

        }

        public InvalidRegistrationException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
