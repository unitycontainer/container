using System;

namespace Unity.Exceptions
{
    public class InvalidRegistrationException : Exception
    {
        internal InvalidRegistrationException(string message)
            : base(message)
        {

        }
    }
}
