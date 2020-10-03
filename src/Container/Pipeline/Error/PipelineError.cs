using System;

namespace Unity.Container
{
    public struct PipelineError
    {
        public bool IsFaulted;
        public object? Error;
        public Exception? Exception;
    }
}
