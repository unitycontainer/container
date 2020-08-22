using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Unity.Telemetry
{
    public partial class DiagnosticSourceTests
    {
        [TestMethod]
        public void Baseline()
        {
            var frame = Telemetry.StartInfoFrame("test");

            Assert.IsFalse(Telemetry.IsEnabled);
            Assert.IsNull(frame.WriteCritical);
            Assert.IsNull(frame.WriteError);
            Assert.IsNull(frame.WriteInfo);
            Assert.IsNull(frame.WriteVerbose);
        }

        [TestMethod]
        public void Subscriber()
        {
            Telemetry.CriticalChannel += TestHandler;

            using var frame = Telemetry.StartInfoFrame("test");

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(frame.WriteCritical);
            Assert.IsNull(frame.WriteError);
            Assert.IsNull(frame.WriteInfo);
            Assert.IsNull(frame.WriteVerbose);
        }

        private void TestHandler(string frame, string name, object payload)
        {
        }
    }
}
