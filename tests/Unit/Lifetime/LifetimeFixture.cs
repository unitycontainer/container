using Microsoft.Practices.Unity.Tests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Lifetime;
using Unity.Tests.TestObjects;

namespace Unity.Tests.v5.Lifetime
{
    /// <summary>
    /// Summary description for MyTest
    /// </summary>
    [TestClass]
    public class LifetimeFixture
    {
        /// <summary>
        /// Registering a type twice with SetSingleton method. once with default and second with name.
        /// </summary>
        [TestMethod]
        public void CheckSetSingletonDoneTwice()
        {
            IUnityContainer uc = new UnityContainer();
            
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterType<A>("hello", new ContainerControlledLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreNotSame(obj, obj1);
        }

        [TestMethod]
        public void CheckSingletonWithDependencies()
        {
            var uc = new UnityContainer();

            uc.RegisterType<ObjectWithOneDependency>(new ContainerControlledLifetimeManager());

            var result1 = uc.Resolve<ObjectWithOneDependency>();
            var result2 = uc.Resolve<ObjectWithOneDependency>();

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result1.InnerObject);
            Assert.IsNotNull(result2.InnerObject);
            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void CheckSingletonAsDependencies()
        {
            var uc = new UnityContainer();

            uc.RegisterType<ObjectWithOneDependency>(new ContainerControlledLifetimeManager());

            var result1 = uc.Resolve<ObjectWithTwoConstructorDependencies>();
            var result2 = uc.Resolve<ObjectWithTwoConstructorDependencies>();

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result1.OneDep);
            Assert.IsNotNull(result2.OneDep);
            Assert.AreNotSame(result1, result2);
            Assert.AreSame(result1.OneDep, result2.OneDep);
        }

        /// <summary>
        /// Registering a type twice with SetSingleton method. once with default and second with name.
        /// </summary>
        [TestMethod]
        public void CheckRegisterInstanceDoneTwice()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance).RegisterInstance<A>("hello", aInstance);
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreSame(obj, aInstance);
            Assert.AreSame(obj1, aInstance);
            Assert.AreSame(obj, obj1);
        }

        /// <summary>
        /// Registering a type as singleton and handling its lifetime. Using SetLifetime method.
        /// </summary>
        [TestMethod]
        public void SetLifetimeTwiceWithLifetimeHandle()
        {
            IUnityContainer uc = new UnityContainer();
            
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
              .RegisterType<A>("hello", new HierarchicalLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A twice. once by default second by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonRegisterInstanceTwice()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance).RegisterInstance<A>("hello", aInstance);
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreSame(obj, obj1);
        }

        /// <summary>
        /// SetLifetime class A. Then use GetOrDefault method to get the instances, once without name, second with name.
        /// </summary>
        [TestMethod]
        public void SetLifetimeGetTwice()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<A>(new ContainerControlledLifetimeManager());
            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
         
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A twice. once by default second by name. 
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonRegisterInstanceTwiceSetLifetimeTwice()
        {
            IUnityContainer container = new UnityContainer();

            A aInstance = new A();

            container.RegisterInstance(aInstance);
            container.RegisterInstance("hello", aInstance);
            container.RegisterType<A>(new ContainerControlledLifetimeManager());
            container.RegisterType<A>("hello1", new ContainerControlledLifetimeManager());

            A obj = container.Resolve<A>();
            A obj1 = container.Resolve<A>("hello1");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonNoNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");

            Assert.AreSame(obj, obj1);
            Assert.AreSame(obj1, obj2);
        }

        /// <summary>
        /// SetLifetime class A. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetLifetimeNoNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");
            
            Assert.AreSame(obj, obj1);
            Assert.AreSame(obj1, obj2);
        }

        /// <summary>
        /// SetSingleton class A with name. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonWithNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>("set", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>("set");
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");
            
            Assert.AreNotSame(obj, obj1);
            Assert.AreSame(obj1, obj2);
            Assert.AreSame(aInstance, obj1);
        }

        /// <summary>
        /// SetLifetime class A with name. Then register instance of class A once by default second by name and
        /// again register instance by another name with lifetime control as false.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetLifetimeWithNameRegisterInstanceDiffNames()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>("set", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<A>("hi", aInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>("set");
            A obj1 = uc.Resolve<A>("hello");
            A obj2 = uc.Resolve<A>("hi");
            
            Assert.AreNotSame(obj, obj1);
            Assert.AreSame(aInstance, obj1);
            Assert.AreSame(obj1, obj2);
        }

        /// <summary>
        /// SetSingleton class A. Then register instance of class A once by default second by name and
        /// lifetime as true. Then again register instance by another name with lifetime control as true
        /// then register.
        /// Then SetLifetime once default and then by name.
        /// </summary>
        [TestMethod]
        public void SetSingletonClassARegisterInstanceOfAandBWithSameName()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            B bInstance = new B();
            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance)
                .RegisterInstance<B>("hi", bInstance)
                .RegisterInstance<B>("hello", bInstance, new ExternallyControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            B obj2 = uc.Resolve<B>("hello");
            B obj3 = uc.Resolve<B>("hi");
            
            Assert.AreSame(obj, obj1);
            Assert.AreNotSame(obj, obj2);
            Assert.AreNotSame(obj1, obj2);
            Assert.AreSame(obj2, obj3);
        }

        /// <summary>defect
        /// SetSingleton class A with name. then register instance of A twice. Once by name, second by default.       
        /// </summary>
        [TestMethod]
        public void SetSingletonByNameRegisterInstanceOnit()
        {
            IUnityContainer uc = new UnityContainer();

            A aInstance = new A();
            uc.RegisterType<A>("SetA", new ContainerControlledLifetimeManager())
                .RegisterInstance<A>(aInstance)
                .RegisterInstance<A>("hello", aInstance);

            A obj = uc.Resolve<A>("SetA");
            A obj1 = uc.Resolve<A>();
            A obj2 = uc.Resolve<A>("hello");
            
            Assert.AreSame(obj1, obj2);
            Assert.AreNotSame(obj, obj2);
        }

        /// <summary>
        /// Use SetLifetime twice, once with parameter, and without parameter
        /// </summary>
        [TestMethod]
        public void TestSetLifetime()
        {
            IUnityContainer uc = new UnityContainer();

            uc.RegisterType<A>(new ContainerControlledLifetimeManager())
               .RegisterType<A>("hello", new ContainerControlledLifetimeManager());

            A obj = uc.Resolve<A>();
            A obj1 = uc.Resolve<A>("hello");
            
            Assert.AreNotSame(obj, obj1);
        }

        /// <summary>
        /// Register class A as singleton then use RegisterInstance to register instance 
        /// of class A.
        /// </summary>
        [TestMethod]
        public void SetSingletonDefaultNameRegisterInstance()
        {
            IUnityContainer uc = new UnityContainer();

            var aInstance = new EmailService();

            uc.RegisterType((Type)null, typeof(EmailService), null, new ContainerControlledLifetimeManager(), null);
            uc.RegisterType((Type)null, typeof(EmailService), "SetA", new ContainerControlledLifetimeManager(), null);
            uc.RegisterInstance(aInstance);
            uc.RegisterInstance("hello", aInstance);
            uc.RegisterInstance("hello", aInstance, new ExternallyControlledLifetimeManager());

            var obj =  uc.Resolve<EmailService>();
            var obj1 = uc.Resolve<EmailService>("SetA");
            var obj2 = uc.Resolve<EmailService>("hello");

            Assert.AreNotSame(obj, obj1);
            Assert.AreSame(obj, obj2);
        }

        /// <summary>
        /// Registering a type in both parent as well as child. Now trying to Resolve from both
        /// check if same or diff instances are returned.
        /// </summary>
        [TestMethod]
        public void RegisterWithParentAndChild()
        {
            //create unity container
            IUnityContainer parentuc = new UnityContainer();

            //register type UnityTestClass
            parentuc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass mytestparent = parentuc.Resolve<UnityTestClass>();
            mytestparent.Name = "Hello World";
            IUnityContainer childuc = parentuc.CreateChildContainer();
            childuc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass mytestchild = childuc.Resolve<UnityTestClass>();

            Assert.AreNotSame(mytestparent.Name, mytestchild.Name);
        }

        /// <summary>
        /// Verify WithLifetime managers. When registered using container controlled and freed, even then
        /// same instance is returned when asked for Resolve.
        /// </summary>
        [TestMethod]
        public void UseContainerControlledLifetime()
        {
            UnityTestClass obj1 = new UnityTestClass();

            obj1.Name = "InstanceObj";

            UnityContainer parentuc = new UnityContainer();
            parentuc.RegisterType<UnityTestClass>(new ContainerControlledLifetimeManager());

            UnityTestClass parentinstance = parentuc.Resolve<UnityTestClass>();
            parentinstance.Name = "Hello World Ob1";
            parentinstance = null;
            GC.Collect();

            UnityTestClass parentinstance1 = parentuc.Resolve<UnityTestClass>();

            Assert.AreSame("Hello World Ob1", parentinstance1.Name);
        }

        /// <summary>
        /// The Resolve method returns the object registered with the named mapping, 
        /// or raises an exception if there is no mapping that matches the specified name. Testing this scenario
        /// Bug ID : 16371
        /// </summary>
        [TestMethod]
        public void TestResolveWithName()
        {
            IUnityContainer uc = new UnityContainer();

            UnityTestClass obj = uc.Resolve<UnityTestClass>("Hello");

            Assert.IsNotNull(obj);
        }
    }
}