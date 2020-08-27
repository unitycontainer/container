using System;

namespace Unity.Exceptions
{
    internal class InvalidRegistrationException : Exception
    {
        public InvalidRegistrationException(string message)
            : base(message)
        {

        }

        public InvalidRegistrationException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
