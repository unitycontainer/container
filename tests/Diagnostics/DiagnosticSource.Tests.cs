using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Unity.Diagnostics
{
    public partial class DiagnosticSourceTests
    {
        [TestMethod]
        public void Baseline()
        {

            var eventQueue = this.CreateEventQueue();
            var entityName = $"Subscriptions";
            var current = Activity.Current;

            using (var listener = CreateEventListener(entityName, eventQueue))
            using (var subscription = DiagnosticListener.AllListeners.Subscribe(listener))
            using (var activity = DiagnosticSource.StartActivity(nameof(Baseline)))
            {
                TestMethod();

                Resolve(typeof(Type));
            }

            Assert.IsFalse(eventQueue.IsEmpty, "There were events present when none were expected");

        }

        public void Resolve(Type type)
        {
            var enabled = DiagnosticSource.DiagnosticListener.IsEnabled("Resolve", null);
            using Activity activity = new Activity("Resolve").AddTag("type", type.FullName)
                                                             .AddTag("name", "contract.Name");
            try
            {
                activity.Start();

                if (enabled) DiagnosticSource.DiagnosticListener.Write("Resolve.Start", null);

                var container = this;

                    
                if (enabled) DiagnosticSource.DiagnosticListener.Write("Resolve.Value", "value");

                // No registration found, resolve unregistered
                return;
            }
            catch (Exception ex)
            {
                if (enabled) DiagnosticSource.DiagnosticListener.Write("Resolve.Exception", ex);
                throw;
            }
            finally
            {
                activity.Stop();
                if (enabled) DiagnosticSource.DiagnosticListener.Write("Resolve.Stop", activity);
            }
        }

        [TestMethod]
        public void References()
        {
            {
                using var book = new ToBeDisposed();
            
            }

        }



        public ref struct ToBeDisposed
        {
            public bool IsDisposed;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
