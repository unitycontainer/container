using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Disgnostics
{
    public partial class Telemetry
    {
        #region Fields

        private PropertyChangedEventHandler? _propertyChanged;

        protected readonly object Sync = new object();

        protected static MethodInfo WriteHandlerInfo = 
            typeof(Telemetry).GetMethod(nameof(TranslateWriteEvent), 
                BindingFlags.NonPublic | BindingFlags.Static)!;

        #endregion


        #region Singleton

        protected static Telemetry Source { get; set; } = new Telemetry();

        #endregion


        #region Implementation

        protected void OnPropertyChanged([CallerMemberName] string? name = null) 
            => _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static void TranslateWriteEvent(TelemetryEventHandler hadler, string name, object payload) 
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
