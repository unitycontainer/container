using System.ComponentModel;

namespace Unity.Disgnostics
{
    public partial class Telemetry
    {
        #region Fields

        private TelemetryEventHandler? _critical;
        private TelemetryEventHandler? _error;
        private TelemetryEventHandler? _warning;
        private TelemetryEventHandler? _info;
        private TelemetryEventHandler? _verbose;

        #endregion


        #region Critical

        public virtual event TelemetryEventHandler CriticalEvent
        {
            add
            {
                bool disabled;

                lock (Sync)
                {
                    disabled = null == _critical;

                    _critical += value;

                    if (disabled) IsEnabled = true;
                    
                    WriteCritical = WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _critical);
                }

                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteCritical)));
                if (disabled) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
            remove
            {
                bool enabled;

                lock (Sync)
                {
                    enabled = IsEnabled;

                    _critical -= value;
                
                    if (null == _critical) IsEnabled = false;

                    WriteCritical = null == _critical 
                                  ? null 
                                  : WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _critical);
                }
                
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteCritical)));
                if (enabled != IsEnabled) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        #endregion
        
        
        #region Error

        public virtual event TelemetryEventHandler ErrorEvent
        {
            add
            {
                bool observed;

                lock (Sync)
                {
                    // Remember state
                    observed = null != _error;

                    // Subscribe handler
                    _error += value;

                    // Subscribe downstream if first
                    if (!observed) CriticalEvent += OnCriticalEvent;

                    // Update writer
                    WriteError = WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _error);
                }

                // Notify if changed
                if (!observed) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteError)));
            }
            remove
            {
                lock (Sync)
                {
                    if (null == _error) return;

                    // Safely unsubscribe via buffer
                    var error = _error;
                    error -= value;

                    // Unsubscribe from downstream events
                    if (null == error) CriticalEvent -= OnCriticalEvent;

                    // Update writer
                    WriteError = null == error
                                 ? null
                                 : WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _error);

                    // Save unsubscribed
                    _error = error;
                }

                // Notify if changed
                if (null == _error) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteError)));
            }
        }

        #endregion


        #region Warning

        public virtual event TelemetryEventHandler WarningEvent
        {
            add
            {
                bool observed;

                lock (Sync)
                {
                    // Remember state
                    observed = null != _warning;

                    // Subscribe handler
                    _warning += value;

                    // Subscribe downstream if first
                    if (!observed) ErrorEvent += OnErrorEvent;

                    // Update writer
                    WriteWarning = WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _warning);
                }

                // Notify if changed
                if (!observed) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteWarning)));
            }
            remove
            {
                lock (Sync)
                {
                    if (null == _warning) return;

                    // Safely unsubscribe via buffer
                    var warning = _warning;
                    warning -= value;

                    // Unsubscribe from downstream events
                    if (null == warning) ErrorEvent -= OnErrorEvent;

                    // Update writer
                    WriteWarning = null == warning
                                 ? null
                                 : WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _warning);

                    // Save unsubscribed
                    _warning = warning;
                }

                // Notify if changed
                if (null == _warning) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteWarning)));
            }
        }

        #endregion


        #region Info

        public virtual event TelemetryEventHandler InfoEvent
        {
            add 
            {
                bool observed;

                lock (Sync)
                {
                    // Remember state
                    observed = null != _info;

                    // Subscribe handler
                    _info += value;

                    // Subscribe downstream if first
                    if (!observed) ErrorEvent += OnErrorEvent;

                    // Update writer
                    WriteVerbose = WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _info);
                }

                // Notify if changed
                if (!observed) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteVerbose)));
            }
            remove
            {
                lock (Sync)
                {
                    if (null == _info) return;

                    // Safely unsubscribe via buffer
                    var info = _info;
                    info -= value;

                    // Unsubscribe from downstream events
                    if (null == info) ErrorEvent -= OnErrorEvent;

                    // Update writer
                    WriteVerbose = null == info
                                 ? null
                                 : WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _info);

                    // Save unsubscribed
                    _info = info;
                }

                // Notify if changed
                if (null == _info) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteVerbose)));
            }
        }

        #endregion


        #region Verbose

        public virtual event TelemetryEventHandler VerboseEvent
        {
            add
            {
                bool observed;

                lock (Sync)
                {
                    // Remember state
                    observed = null != _verbose;

                    // Subscribe handler
                    _verbose += value;

                    // Subscribe downstream if first
                    if (!observed) InfoEvent += OnInfoEvent;

                    // Update writer
                    WriteVerbose = WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _verbose);
                }

                // Notify if changed
                if (!observed) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteVerbose)));
            }
            remove
            {
                lock (Sync)
                {
                    if (null == _verbose) return;

                    // Safely unsubscribe via buffer
                    var verbose = _verbose;
                    verbose -= value;

                    // Unsubscribe from downstream events
                    if (null == verbose) InfoEvent -= OnInfoEvent;

                    // Update writer
                    WriteVerbose = null == verbose
                                 ? null
                                 : WriteHandlerInfo.CreateDelegate<WriteEventDelegate>(typeof(WriteEventDelegate), _verbose);
                    
                    // Save unsubscribed
                    _verbose = verbose;
                }

                // Notify if changed
                if( null == _verbose) _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteVerbose)));
            }
        }

        #endregion


        #region Event Propagation Handlers

        private void OnInfoEvent(string name, object? payload)     => _verbose!(name, payload);
        private void OnWarningEvent(string name, object? payload)  => _info!(name, payload);
        private void OnErrorEvent(string name, object? payload)    => _warning!(name, payload);
        private void OnCriticalEvent(string name, object? payload) => _error!(name, payload);


        #endregion
    }
}
