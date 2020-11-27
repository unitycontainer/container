using System;
using System.Runtime.ExceptionServices;

namespace Unity.Container
{
    public struct ErrorInfo
    {
        #region Fields

        private ExceptionDispatchInfo? _exception;
        public bool IsFaulted;
        public string? Message;

        #endregion

        public void Capture(Exception exception)
        {
            IsFaulted = true;
            Message   = exception.Message;

            _exception = ExceptionDispatchInfo.Capture(exception);
        }

        public void TypeLoadException(ArgumentException exception)
        {
            IsFaulted = true;
            Message = exception.Message;

            _exception = ExceptionDispatchInfo.Capture(exception);
        }

        public void Throw() => _exception?.Throw();
    }
}
