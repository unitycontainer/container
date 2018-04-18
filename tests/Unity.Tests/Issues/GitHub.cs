using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Lifetime;
using Unity.Attributes;

namespace Unity.Tests.Issues
{
    [TestClass]
    public class GitHubIssues
    {
        [TestMethod]
        public void unitycontainer_container_82()
        {
            // Create root container and register classes in root
            var rootContainer = new UnityContainer();
            rootContainer.RegisterType<MainClass>(new PerResolveLifetimeManager());
            rootContainer.RegisterType<IHostClass, MainClass>();
            
            // Create a child container
            var childContainer = rootContainer.CreateChildContainer();

            //Resolve main class from root container - WORKS
            //NOTE: if you uncomment these lines it fixes the issue i guess it's beause the MainClass get's 
            //resolved in the context of the root container
//            var main1 = rootContainer.Resolve<MainClass>();
//            Assert.AreEqual(main1, main1.HelperClass.HostClass);

            //Resolve main class from child container - GENERATES STACK OVERFLOW
            var main2 = childContainer.Resolve<MainClass>();
            Assert.AreEqual(main2, main2.HelperClass.HostClass);
        }

    }

    public class MainClass : IHostClass
    {
        public MainClass()
        {
            Debug.Print("Inside Constructor");
        }

        [Dependency]
        public HelperClass HelperClass { get; set; }

        public void DoSomething()
        {
        }
    }

    public interface IHostClass
    {
        void DoSomething();
    }

    public class HelperClass
    {
        [Dependency] 
        public IHostClass HostClass { get; set; }
    }
}
