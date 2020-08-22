using System;
using System.Runtime.CompilerServices;

namespace Unity.Telemetry
{
    public delegate void WriteEventDelegate(string name);

    public delegate void TelemetryEventHandler(string frame, string name, object? payload);

    public abstract class EventSource
    {
        #region Fields

        private bool _isEnabled;

        private TelemetryEventHandler? _critical;
        private TelemetryEventHandler? _error;
        private TelemetryEventHandler? _warning;
        private TelemetryEventHandler? _info;
        private TelemetryEventHandler? _verbose;

        #endregion


        #region Events

        public event TelemetryEventHandler CriticalChannel
        {
            add
            {
                _critical += value;
                if (!_isEnabled) _isEnabled = true;
            }
            remove
            {
                _critical -= value;
                _isEnabled = IsTelementryEnabled();
            }
        }

        public event TelemetryEventHandler ErrorChannel
        {
            add 
            {
                _error += value;
                if (!_isEnabled) _isEnabled = true;
            }
            remove 
            {
                _error -= value;
                _isEnabled = IsTelementryEnabled();
            }
        }

        public event TelemetryEventHandler WarningChannel
        {
            add 
            { 
                _warning += value;
                if (!_isEnabled) _isEnabled = true;
            }
            remove 
            {
                _warning -= value;
                _isEnabled = IsTelementryEnabled();
            }
        }

        public event TelemetryEventHandler InfoChannel
        {
            add 
            { 
                _info += value;
                if (!_isEnabled) _isEnabled = true;
            }
            remove
            {
                _info -= value;
                _isEnabled = IsTelementryEnabled();
            }
        }

        public event TelemetryEventHandler VerboseChannel
        {
            add
            { 
                _verbose += value;
                if (!_isEnabled) _isEnabled = true;
            }
            remove
            { 
                _verbose -= value;
                _isEnabled = IsTelementryEnabled();
            }
        }

        #endregion


        #region Public

        public bool IsEnabled => _isEnabled;

        public EventFrame StartInfoFrame(string name) => new EventFrame(this, name);

        public EventFrame StartVerboseFrame(string name) => new EventFrame(this, name, true);

        #endregion


        #region Frame

        public readonly ref struct EventFrame
        {
            #region Fields

            private readonly string _name;

            #endregion


            #region Constructors

            public EventFrame(EventSource source, string name, bool verbose = false)
            {
                _name = name;
                
                // Capture delegates
                var critical = source._critical;
                var error    = source._error;
                var warning  = source._warning;
                var info     = source._info;
                //var verbose  = source._verbose;

                WriteCritical = null == critical ? null : (WriteEventDelegate)((n) => critical(name, n, null));
                WriteError    = null;
                WriteWarning  = null;
                WriteInfo     = null;
                WriteVerbose  = (n) => { };


                //if (null != _critical) WriteCritical = WriteCriticalMethod;
            }


            #endregion


            #region Public

            public WriteEventDelegate? WriteCritical { get; }

            public WriteEventDelegate? WriteError { get; }

            public WriteEventDelegate? WriteWarning { get; }

            public WriteEventDelegate? WriteInfo { get; }

            public WriteEventDelegate? WriteVerbose { get; }

            #endregion


            #region Disposable

            public void Dispose()
            {

            }

            #endregion
        }

        #endregion


        #region Implementation

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
