using System.ComponentModel;

namespace Unity.Disgnostics
{
    public delegate void WriteEventDelegate(string name, object? payload);

    public delegate void TelemetryEventHandler(string name, object? payload);

    public partial class Telemetry
    {
        #region Public

        public static bool IsEnabled { get; private set; }

        #endregion


        #region Write

        public static WriteEventDelegate? WriteCritical { get; protected set; }

        public static WriteEventDelegate? WriteError { get; protected set; }

        public static WriteEventDelegate? WriteWarning { get; protected set; }

        public static WriteEventDelegate? WriteInfo { get; protected set; }

        public static WriteEventDelegate? WriteVerbose { get; protected set; }

        #endregion


        #region Channels

        public static event TelemetryEventHandler CriticalChannel
        {
            add => Source.CriticalEvent += value;
            remove => Source.CriticalEvent -= value;
        }

        public static event TelemetryEventHandler ErrorChannel
        {
            add => Source.ErrorEvent += value;
            remove => Source.ErrorEvent -= value;
        }

        public static event TelemetryEventHandler WarningChannel
        {
            add => Source.WarningEvent += value;
            remove => Source.WarningEvent -= value;
        }

        public static event TelemetryEventHandler InfoChannel
        {
            add => Source.InfoEvent += value;
            remove => Source.InfoEvent -= value;
        }

        public static event TelemetryEventHandler VerboseChannel
        {
            add => Source.VerboseEvent += value;
            remove => Source.VerboseEvent -= value;
        }


        #endregion


        #region INotifyPropertyChanged

        public static event PropertyChangedEventHandler? PropertyChanged
        {
            add => Source._propertyChanged += value;
            remove => Source._propertyChanged -= value;
        }

        #endregion
    }
}
