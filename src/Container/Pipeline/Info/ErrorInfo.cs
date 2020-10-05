using System;

namespace Unity.Container
{
    public struct ErrorInfo
    {
        public bool IsFaulted;
        public object? Error;
        public Exception? Exception;
    }
}
