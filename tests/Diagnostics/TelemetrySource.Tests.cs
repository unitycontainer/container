using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Disgnostics
{
    public partial class DiagnosticSourceTests
    {
        [TestMethod]
        public void Baseline()
        {
            Assert.IsFalse(Telemetry.IsEnabled);
            Assert.IsNull( Telemetry.WriteCritical);
            Assert.IsNull( Telemetry.WriteError);
            Assert.IsNull( Telemetry.WriteInfo);
            Assert.IsNull( Telemetry.WriteVerbose);
        }

        [TestMethod]
        public void SubscribeToCritical()
        {
            Telemetry.CriticalChannel += Telemetry_Handler;

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            Telemetry.CriticalChannel -= Telemetry_Handler;
            
            Assert.IsFalse(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);
        }

        [TestMethod]
        public void SubscribeToCriticalTwice()
        {
            Telemetry.CriticalChannel += Telemetry_Handler;
            Telemetry.CriticalChannel += Other_Handler;

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            Telemetry.CriticalChannel -= Telemetry_Handler;

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

        }


        [Ignore]
        [TestMethod]
        public void SubscribeToError()
        {
            Telemetry.ErrorChannel += Telemetry_Handler;

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            Telemetry.ErrorChannel -= Telemetry_Handler;

            Assert.IsFalse(Telemetry.IsEnabled);
            Assert.IsNull(Telemetry.WriteCritical);
            Assert.IsNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);
        }

        [TestMethod]
        public void SubscribeToErrorTwice()
        {
            Telemetry.ErrorChannel += Telemetry_Handler;
            Telemetry.ErrorChannel += Other_Handler;

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

            Telemetry.ErrorChannel -= Telemetry_Handler;

            Assert.IsTrue(Telemetry.IsEnabled);
            Assert.IsNotNull(Telemetry.WriteCritical);
            Assert.IsNotNull(Telemetry.WriteError);
            Assert.IsNull(Telemetry.WriteInfo);
            Assert.IsNull(Telemetry.WriteVerbose);

        }


        [TestMethod]
        public void SubscribeVerbose()
        {
            Telemetry.VerboseChannel += Telemetry_VerboseChannel;

            //Assert.IsTrue(Telemetry.IsEnabled);
            //Assert.IsNull(Telemetry.WriteCritical);
            //Assert.IsNull(Telemetry.WriteError);
            //Assert.IsNull(Telemetry.WriteInfo);
            //Assert.IsNull(Telemetry.WriteVerbose);

            Telemetry.VerboseChannel -= Telemetry_VerboseChannel;


            void Telemetry_VerboseChannel(string name, object payload) => throw new System.NotImplementedException();
        }

        [TestMethod]
        public void SubscribeVerboseTwice()
        {
            Telemetry.VerboseChannel += Telemetry_VerboseChannel;
            Telemetry.VerboseChannel += Other_Handler;

            //Assert.IsTrue(Telemetry.IsEnabled);
            //Assert.IsNull(Telemetry.WriteCritical);
            //Assert.IsNull(Telemetry.WriteError);
            //Assert.IsNull(Telemetry.WriteInfo);
            //Assert.IsNull(Telemetry.WriteVerbose);

            Telemetry.VerboseChannel -= Telemetry_VerboseChannel;


            void Telemetry_VerboseChannel(string name, object payload) => throw new System.NotImplementedException();
        }

        void Other_Handler(string name, object payload) => throw new System.NotImplementedException();
        void Telemetry_Handler(string name, object payload) => throw new System.NotImplementedException();


        //[TestMethod]
        //public void Subscription()
        //{
        //    Assert.IsFalse(Telemetry.IsEnabled);

        //    // Subscribe Critical
        //    Telemetry.CriticalChannel += TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNotNull(Telemetry.WriteCritical);
        //    Assert.IsNull(Telemetry.WriteError);
        //    Assert.IsNull(Telemetry.WriteInfo);
        //    Assert.IsNull(Telemetry.WriteVerbose);

        //    // Subscribe Error
        //    Telemetry.ErrorChannel += TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNotNull(Telemetry.WriteCritical);
        //    Assert.IsNotNull(Telemetry.WriteError);
        //    Assert.IsNull(Telemetry.WriteInfo);
        //    Assert.IsNull(Telemetry.WriteVerbose);

        //    // Subscribe Info
        //    Telemetry.InfoChannel += TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNotNull(Telemetry.WriteCritical);
        //    Assert.IsNotNull(Telemetry.WriteError);
        //    Assert.IsNotNull(Telemetry.WriteInfo);
        //    Assert.IsNull(Telemetry.WriteVerbose);

        //    // Subscribe Verbose
        //    Telemetry.VerboseChannel += TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNotNull(Telemetry.WriteCritical);
        //    Assert.IsNotNull(Telemetry.WriteError);
        //    Assert.IsNotNull(Telemetry.WriteInfo);
        //    Assert.IsNotNull(Telemetry.WriteVerbose);

        //    // Unsubscribe Critical
        //    Telemetry.CriticalChannel -= TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNull(Telemetry.WriteCritical);
        //    Assert.IsNotNull(Telemetry.WriteError);
        //    Assert.IsNotNull(Telemetry.WriteInfo);
        //    Assert.IsNotNull(Telemetry.WriteVerbose);

        //    // Unsubscribe Error
        //    Telemetry.ErrorChannel -= TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNull(Telemetry.WriteCritical);
        //    Assert.IsNull(Telemetry.WriteError);
        //    Assert.IsNotNull(Telemetry.WriteInfo);
        //    Assert.IsNotNull(Telemetry.WriteVerbose);

        //    // Unsubscribe Info
        //    Telemetry.InfoChannel -= TestHandler;
        //    Assert.IsTrue(Telemetry.IsEnabled);
        //    Assert.IsNull(Telemetry.WriteCritical);
        //    Assert.IsNull(Telemetry.WriteError);
        //    Assert.IsNull(Telemetry.WriteInfo);
        //    Assert.IsNotNull(Telemetry.WriteVerbose);

        //    // Unsubscribe Info
        //    Telemetry.VerboseChannel -= TestHandler;
        //    Assert.IsFalse(Telemetry.IsEnabled);
        //    Assert.IsNull(Telemetry.WriteCritical);
        //    Assert.IsNull(Telemetry.WriteError);
        //    Assert.IsNull(Telemetry.WriteInfo);
        //    Assert.IsNull(Telemetry.WriteVerbose);

        //    static void TestHandler(string name, object payload) { }
        //}

    }
}
