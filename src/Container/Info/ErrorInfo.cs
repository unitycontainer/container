using System;
using System.Runtime.ExceptionServices;

namespace Unity.Container
{
    public struct ErrorInfo
    {
        #region Fields

        public bool IsFaulted;

        public string? Message { get; private set; }
        public Exception? Exception { get; private set; }
        public ExceptionDispatchInfo? DispatchInfo { get; private set; }

        #endregion

        public object Error(string message)
        {
            IsFaulted = true;
            Message = message;

            return RegistrationManager.NoValue;
        }

        public object Throw(Exception exception)
        {
            IsFaulted = true;
            Message = exception.Message;
            Exception = exception;
            
            return RegistrationManager.NoValue;
        }

        public object Capture(Exception exception)
        {
            IsFaulted = true;
            Message   = exception.Message;
            DispatchInfo = ExceptionDispatchInfo.Capture(exception);
            
            return RegistrationManager.NoValue;
        }

        public void Throw() => DispatchInfo?.Throw();
    }
}
