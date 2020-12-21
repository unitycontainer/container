using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Unity.Container
{
    public struct ErrorInfo
    {
        #region Fields

        private ErrorType _type;
        private object?   _value;

        public bool IsFaulted;

        #endregion


        #region Properties

        public string? Message
        {
            get => _type switch
            {
                ErrorType.Message => _value as string,
                ErrorType.Exception => (_value as Exception)?.Message,
                ErrorType.DispatchInfo => (_value as ExceptionDispatchInfo)?.SourceException
                                                                            .Message,
                _ => null,
            };
            
            set
            {
                IsFaulted = true;
                _value = value;
                _type = ErrorType.Message;
            }
        }

        public Exception? Exception
        {
            get => _type switch
            {
                ErrorType.Exception => Unsafe.As<Exception>(_value),
                ErrorType.DispatchInfo => Unsafe.As<ExceptionDispatchInfo>(_value)?.SourceException,
                _ => null,
            };

            private set
            {
                IsFaulted = true;
                _value = value;
                _type = ErrorType.Exception;
            }
        }

        #endregion


        #region Methods

        public object Error(string message)
        {
            IsFaulted = true;
            _value = message;
            _type = ErrorType.Message;

            return UnityContainer.NoValue;
        }

        public object Throw(Exception exception)
        {
            IsFaulted = true;
            _value = exception;
            _type = ErrorType.Exception;

            return UnityContainer.NoValue;
        }

        public object Capture(Exception exception)
        {
            IsFaulted = true;
            _value = ExceptionDispatchInfo.Capture(exception);
            _type = ErrorType.DispatchInfo;
            
            return UnityContainer.NoValue;
        }

        #endregion
        public void Throw()
        {
            if (ErrorType.DispatchInfo != _type) return;
            ((ExceptionDispatchInfo)_value!).Throw();
        }


        #region Nested Type

        public enum ErrorType
        {
            None = 0,

            Message,

            Exception,

            DispatchInfo
        }

        #endregion
    }
}
