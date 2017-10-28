using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Tests.TestObjects;

namespace Unity.Tests.Registration
{
    [TestClass]
    public class RegisterInstanceFixture
    {
        [TestMethod]
        public void RegisterInstance_IUC_SimpleObject()
        {
            var instance = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(null, null, instance, null);
            Assert.AreEqual(container.Resolve<string>(), instance);
        }

        [TestMethod]
        public void RegisterInstance_IUC_NamedObject()
        {
            var instance = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(null, instance, instance, null);

            Assert.AreEqual(container.Resolve<string>(instance), instance);
            Assert.ThrowsException<ResolutionFailedException>(() => container.Resolve<string>());
        }

        [TestMethod]
        public void RegisterInstance_IUC_InterfacedObject()
        {
            var instance = new EmailService();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(typeof(IService), null, instance, null);

            Assert.AreEqual(container.Resolve<IService>(), instance);
            Assert.AreNotEqual(container.Resolve<EmailService>(), instance);
        }

        [TestMethod]
        public void RegisterInstance_IUC_NamedInterfacedObject()
        {
            var instance = new EmailService();
            var name = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(typeof(IService), name, instance, null);

            Assert.AreEqual(container.Resolve<IService>(name), instance);
            Assert.AreNotEqual(container.Resolve<EmailService>(), instance);
            Assert.ThrowsException<ResolutionFailedException>(() => container.Resolve<IService>());
        }

        [TestMethod]
        public void RegisterInstance_SimpleObject()
        {
            var instance = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(instance);
            
            Assert.AreEqual(container.Resolve<string>(), instance);
        }

        [TestMethod]
        public void RegisterInstance_NamedObject()
        {
            var instance = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(instance, instance);

            Assert.AreEqual(container.Resolve<string>(instance), instance);
            Assert.ThrowsException<ResolutionFailedException>(() => container.Resolve<string>());
        }

        [TestMethod]
        public void RegisterInstance_InterfacedObject()
        {
            var instance = new EmailService();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IService>(instance);

            Assert.AreEqual(container.Resolve<IService>(), instance);
            Assert.AreNotEqual(container.Resolve<EmailService>(), instance);
        }

        [TestMethod]
        public void RegisterInstance_NamedInterfacedObject()
        {
            var instance = new EmailService();
            var name = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IService>(name, instance);

            Assert.AreEqual(container.Resolve<IService>(name), instance);
            Assert.AreNotEqual(container.Resolve<EmailService>(), instance);
            Assert.ThrowsException<ResolutionFailedException>(() => container.Resolve<IService>());
        }

        [TestMethod]
        public void RegisterInstance_ExternallyControlledLifetimeManager()
        {
            var instance = Guid.NewGuid().ToString();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(null, null, instance, new ExternallyControlledLifetimeManager());
            Assert.AreEqual(container.Resolve<string>(), instance);
        }


        [TestMethod]
        public void RegisterInstance_ChainRegistrations()
        {
            var instance = new EmailService();

            IUnityContainer container = new UnityContainer();

            container.RegisterInstance(instance);
            container.RegisterType<IService, EmailService>();

            Assert.AreEqual(container.Resolve<IService>(), instance);
        }

        [TestMethod]
        public void RegisterInstance_RegisterWithParentAndChild()
        {
            //create unity container
            var parent = new UnityContainer();
            parent.RegisterInstance(null, null, Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager());

            var child = parent.CreateChildContainer();
            child.RegisterInstance(null, null, Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager());

            Assert.AreSame(parent.Resolve<string>(), parent.Resolve<string>());
            Assert.AreSame(child.Resolve<string>(), child.Resolve<string>());
            Assert.AreNotSame(parent.Resolve<string>(), child.Resolve<string>());
        }


        [TestMethod]
        public void RegisterInstance_HierarchicalLifetimeManager()
        {
//            Assert.ThrowsException<Exception>(() => new UnityContainer().RegisterInstance(null, null, Guid.NewGuid().ToString(), new HierarchicalLifetimeManager()));
        }
    }
}
