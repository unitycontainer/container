using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Telemetry
{
    public partial class DiagnosticSourceTests
    {
        [TestMethod]
        public void Baseline()
        {
            Assert.IsFalse(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);
        }

        [TestMethod]
        public void Subscription()
        {
            Assert.IsFalse(Telemetry.IsEnabled);

            // Subscribe Critical
            Telemetry.CriticalChannel += TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            // Subscribe Error
            Telemetry.ErrorChannel += TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            // Subscribe Info
            Telemetry.InfoChannel += TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNotNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            // Subscribe Verbose
            Telemetry.VerboseChannel += TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNotNull(Telemetry.WriteInfo);
            Assert.IsNotNull(Telemetry.WriteVerbose);

            // Unsubscribe Critical
            Telemetry.CriticalChannel -= TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNotNull(Telemetry.WriteInfo);
            Assert.IsNotNull(Telemetry.WriteVerbose);

            // Unsubscribe Error
            Telemetry.ErrorChannel -= TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNotNull(Telemetry.WriteInfo);
            Assert.IsNotNull(Telemetry.WriteVerbose);

            // Unsubscribe Info
            Telemetry.InfoChannel -= TestHandler;
            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNotNull(Telemetry.WriteVerbose);

            // Unsubscribe Info
            Telemetry.VerboseChannel -= TestHandler;
            Assert.IsFalse(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            static void TestHandler(string name, object payload) { }
        }

    }
}
