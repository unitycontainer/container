using System;

namespace Unity.Policy.Mapping
{
    internal class MakeGenericTypeFailedException : Exception
    {
        public MakeGenericTypeFailedException(ArgumentException innerException)
            : base("MakeGenericType failed", innerException)
        {
        }
    }
}
