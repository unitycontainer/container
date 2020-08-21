using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace Unity.Diagnostics
{
    internal static class UnityDiagnosticSource
    {
        #region Constants

        public const string ListenerName = "Unity.Container";
        public const string BaseActivityName = ListenerName + ".";

        public const string ExceptionEventName = BaseActivityName + "Exception";
        public const string DiagnosticEventName = BaseActivityName + "Diagnostic";

        #endregion


        #region Fields

        private static readonly object LogAlways = EventLevel.LogAlways;
        private static readonly object Critical = EventLevel.Critical;
        private static readonly object Error = EventLevel.Error;
        private static readonly object Warning = EventLevel.Warning;
        private static readonly object Informational = EventLevel.Informational;
        private static readonly object Verbose = EventLevel.Verbose;
#if NET45
        private static readonly object None        = 0;
        private static readonly object Admin       = 16;
        private static readonly object Operational = 17;
        private static readonly object Analytic    = 18;
        private static readonly object Debug       = 19;
#else
        private static readonly object None        = EventChannel.None;
        private static readonly object Admin       = EventChannel.Admin;
        private static readonly object Operational = EventChannel.Operational;
        private static readonly object Analytic    = EventChannel.Analytic;
        private static readonly object Debug       = EventChannel.Debug;
#endif

        public static readonly DiagnosticListener DiagnosticListener = new DiagnosticListener(ListenerName);

        #endregion


        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled() => DiagnosticListener.IsEnabled();


        public static Activity StartActivity(string name, object? payload = null)
        {
            return DiagnosticListener.IsEnabled(name, LogAlways, None)
                 ? DiagnosticListener.StartActivity(new Activity(name), payload)
                 : new Activity(name).Start();
        }

        public static void StopActivity(Activity activity, object? payload = null)
        {
            DiagnosticListener.StopActivity(activity, payload);
        }


        //public Activity? Start(string operationName, object? payload)
        //{
        //    Activity? activity = null;

        //    string activityName = BaseActivityName + operationName;
        //    if (DiagnosticListener.IsEnabled(activityName, _entity))
        //    {
        //        activity = new Activity(activityName);

        //        if (DiagnosticListener.IsEnabled(activityName + ".Start"))
        //        {
        //            DiagnosticListener.StartActivity(activity, payload);
        //        }
        //        else
        //        {
        //            activity.Start();
        //        }
        //    }

        //    return activity;
        //}

        //public void Stop(Activity activity, object? payload)
        //{
        //    if (activity != null) 
        //    {
        //        DiagnosticListener.StopActivity(activity, payload);
        //    }
        //}

        //public void Write()
        //{

        //    DiagnosticListener.Write(BaseActivityName + _entity + ".Write", "ss");
        //}


        #endregion

        // StartActivity(String, ActivityKind)


        #region Exception

        //internal void ReportException(Exception ex)
        //{
        //    if (DiagnosticListener.IsEnabled(ExceptionEventName))
        //    {
        //        DiagnosticListener.Write(ExceptionEventName,
        //            new
        //            {
        //                Exception = ex,
        //                Entity = _entity
        //            });
        //    }
        //}

        #endregion
    }


    internal static class UnityDiagnosticSourceExtensions
    {
        public static void StopActivity(this Activity? activity, object? payload = null)
        {
            if (null == activity) return;

            UnityDiagnosticSource.StopActivity(activity, payload);
        }

    }
}
