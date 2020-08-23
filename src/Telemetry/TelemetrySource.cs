using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Telemetry
{
    public delegate void WriteEventDelegate(string name, object? payload);

    public delegate void TelemetryEventHandler(string name, object? payload);

    public abstract class EventSource
    {
        #region Fields

        private bool _isEnabled;
        private object _sync = new object();

        private TelemetryEventHandler? _critical;
        private TelemetryEventHandler? _error;
        private TelemetryEventHandler? _warning;
        private TelemetryEventHandler? _info;
        private TelemetryEventHandler? _verbose;

        protected static MethodInfo WriteHandlerInfo = 
            typeof(EventSource).GetMethod(nameof(WriteHandler), BindingFlags.NonPublic | 
                                                                BindingFlags.Static)   !;

        #endregion


        #region Channels

        public event TelemetryEventHandler CriticalChannel
        {
            add
            {
                lock (_sync)
                {
                    _critical += value;
                    _isEnabled = true;
                    WriteCritical = (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _critical);
                }
            }
            remove
            {
                lock (_sync)
                {
                    _critical -= value;
                    _isEnabled = IsTelementryEnabled();
                    WriteCritical = null == _critical 
                                  ? null 
                                  : (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _critical);
                }
            }
        }

        public event TelemetryEventHandler ErrorChannel
        {
            add 
            {
                lock (_sync)
                { 
                    _error += value;
                    _isEnabled = true;
                    WriteError = (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _error);
                }
            }
            remove 
            {
                lock (_sync)
                { 
                    _error -= value;
                    _isEnabled = IsTelementryEnabled();
                    WriteError = null == _critical
                               ? null
                               : (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _error);
                }
            }
        }

        public event TelemetryEventHandler WarningChannel
        {
            add 
            {
                lock (_sync)
                { 
                    _warning += value;
                    _isEnabled = true;
                    WriteWarning = (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _warning);
                }
            }
            remove 
            {
                lock (_sync)
                {
                    _warning -= value;
                    _isEnabled = IsTelementryEnabled();
                    WriteWarning = null == _critical
                               ? null
                               : (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _warning);
                }
            }
        }

        public event TelemetryEventHandler InfoChannel
        {
            add 
            {
                lock (_sync)
                {
                    _info += value;
                    _isEnabled = true;
                    WriteInfo = (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _info);
                }
            }
            remove
            {
                lock (_sync)
                {
                    _info -= value;
                    _isEnabled = IsTelementryEnabled();
                    WriteInfo = null == _critical
                              ? null
                              : (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _info);
                }
            }
        }

        public event TelemetryEventHandler VerboseChannel
        {
            add
            {
                lock (_sync)
                {
                    _verbose += value;
                    _isEnabled = true;
                    WriteVerbose = (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _verbose);
                }
            }
            remove
            {
                lock (_sync)
                {
                    _verbose -= value;
                    _isEnabled = IsTelementryEnabled();
                    WriteVerbose = null == _critical
                                 ? null
                                 : (WriteEventDelegate)WriteHandlerInfo.CreateDelegate(typeof(WriteEventDelegate), _verbose);
                }
            }
        }

        #endregion


        #region Writers

        public WriteEventDelegate? WriteCritical { get; private set; }

        public WriteEventDelegate? WriteError { get; private set; }

        public WriteEventDelegate? WriteWarning { get; private set; }

        public WriteEventDelegate? WriteInfo { get; private set; }

        public WriteEventDelegate? WriteVerbose { get; private set; }

        #endregion



        #region Public

        public bool IsEnabled => _isEnabled;

        #endregion


        #region Implementation

        private static void WriteHandler(TelemetryEventHandler hadler, string name, object payload) 
            => hadler(name, payload);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTelementryEnabled() 
            => null != _critical ||
               null != _error    ||
               null != _warning  ||
               null != _info     ||
               null != _verbose;

        #endregion
    }
}
