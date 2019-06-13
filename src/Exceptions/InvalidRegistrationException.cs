using System;

namespace Unity.Exceptions
{
    public class InvalidRegistrationException : Exception
    {
        internal InvalidRegistrationException(Exception ex)
            : base(ex.Message, ex)
        {

        }

        internal InvalidRegistrationException(string message)
            : base(message)
        {

        }

        internal InvalidRegistrationException(string message, object info)
            : base(message)
        {
            Data.Add(Guid.NewGuid(), info);
        }

    }
}
