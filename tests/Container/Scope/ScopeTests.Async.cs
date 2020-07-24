using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.BuiltIn;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public virtual void IndexOfTest()
        {
            //int index0 =  0;
            //int index1 = -1;
            //int index2 = -2;
            //int index3 = -3;
            //var test = "test";
            //var testScope = new TestScope((ContainerScope)Scope);

            //Thread thread1 = new Thread(delegate ()
            //{
            //    index1 = testScope.GetIndexOf(Name);
            //})
            //{ Name = "1" };

            //Thread thread2 = new Thread(delegate ()
            //{
            //    index2 = testScope.GetIndexOf(test);
            //})
            //{ Name = "2" };

            //Thread thread3 = new Thread(delegate ()
            //{
            //    index3 = testScope.GetIndexOf("unknown");
            //})
            //{ Name = "3" };

            //Monitor.Enter(testScope.ContractSync);
            //thread1.Start();
            //thread2.Start();
            //thread3.Start();

            //Thread.Sleep(100);

            //index0 = testScope.GetIndexOf(Name);
            //index0 = testScope.GetIndexOf(test);

            //Monitor.Exit(testScope.ContractSync);

            //thread1.Join();
            //thread2.Join();
            //thread3.Join();

            //Assert.AreEqual(1, index1);
            //Assert.AreEqual(2, index2);
            //Assert.AreEqual(3, index3);
            //Assert.AreEqual(1, testScope.GetIndexOf(Name));
        }
    }
}
