using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    [TestClass]
    public partial class Generics : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Test Data

        public class ServiceFoo : Foo<Service>
        {
        }

        public class Refer<TEntity>
        {
            private string str;

            public string Str
            {
                get { return str; }
                set { str = value; }
            }

            public Refer()
            {
                str = "Hello";
            }
        }

        public interface ICommand<T>
        {
            void Execute(T data);
            void ChainedExecute(ICommand<T> inner);
        }

        public class ConcreteCommand<T> : ICommand<T>
        {
            private object p = null;

            public void Execute(T data)
            {
            }

            public void ChainedExecute(ICommand<T> inner)
            {
            }

            public object NonGenericProperty
            {
                get { return p; }
                set { p = value; }
            }
        }

        public class LoggingCommand<T> : ICommand<T>
        {
            private ICommand<T> inner;

            public bool ChainedExecuteWasCalled = false;
            public bool WasInjected = false;

            public LoggingCommand(ICommand<T> inner)
            {
                this.inner = inner;
            }

            public LoggingCommand()
            {
            }

            public ICommand<T> Inner
            {
                get { return inner; }
                set { inner = value; }
            }

            public void Execute(T data)
            {
                // do logging here
                Inner.Execute(data);
            }

            public void ChainedExecute(ICommand<T> innerCommand)
            {
                ChainedExecuteWasCalled = true;
            }

            public void InjectMe()
            {
                WasInjected = true;
            }
        }

        public class DisposableCommand<T> : ICommand<T>, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Execute(T data)
            {
            }

            public void ChainedExecute(ICommand<T> inner)
            {
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }


        public interface IGenericService<T> { }

        public class ServiceA<T> : IGenericService<T> { }

        public class ServiceB<T> : IGenericService<T> { }

        public class ServiceClass<T> : IGenericService<T> where T : class { }

        public class ServiceStruct<T> : IGenericService<T> where T : struct { }

        public class ServiceNewConstraint<T> : IGenericService<T> where T : new() { }

        public class TypeWithNoPublicNoArgCtors
        {
            public TypeWithNoPublicNoArgCtors(int _) { }
            private TypeWithNoPublicNoArgCtors() { }
        }

        public class ServiceInterfaceConstraint<T> : IGenericService<T> where T : IEnumerable { }

        #endregion
    }
}
