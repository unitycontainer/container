using System;

namespace Unity.Container
{
    public struct ErrorInfo
    {
        public bool IsFaulted;
        public string? Message;
        public Exception? Exception;
    }
}
