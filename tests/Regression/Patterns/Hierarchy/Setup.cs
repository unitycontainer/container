using Regression;
using System;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Container
{
    public abstract partial class Pattern : PatternBase
    {
        #region Test Data

        interface ISingletonService : IDisposable
        {
            long ContainerId { get; }

            bool IsDisposed { get; }
        }

        interface ISingletonServiceWithDependency : ISingletonService
        {
            ITestElement Element { get; }
        }

        interface ISingletonServiceWithFactory : ISingletonService
        {
            IEnumerable<ITestElement> GetElements();
        }

        interface ISingletonConsumer : IDisposable
        {
            long ContainerId { get; }

            bool IsDisposed { get; }

            ISingletonService SingletonService { get; }
        }

        interface ITestElement : IDisposable
        {
            long ContainerId { get; }

            bool IsDisposed { get; }
        }

        interface ITestElementFactory
        {
            ITestElement CreateElement();
        }

        class SingletonService : ISingletonService
        {
            public long ContainerId { get; }

            public bool IsDisposed { get; private set; }

            public SingletonService(IUnityContainer container)
            {
                ContainerId = container.GetHashCode();
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        class SingletonServiceWithFactory : ISingletonServiceWithFactory
        {
            private readonly ITestElementFactory _elementFactory;

            public long ContainerId { get; }

            public bool IsDisposed { get; private set; }

            public SingletonServiceWithFactory(IUnityContainer container, ITestElementFactory factory)
            {
                ContainerId = container.GetHashCode();
                _elementFactory = factory;
            }

            public IEnumerable<ITestElement> GetElements()
            {
                for (int i = 0; i < 10; i++)
                    yield return _elementFactory.CreateElement();
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        class SingletonServiceWithDependency : ISingletonServiceWithDependency
        {
            public long ContainerId { get; }

            public ITestElement Element { get; }

            public bool IsDisposed { get; private set; }

            public SingletonServiceWithDependency(IUnityContainer container, ITestElement element)
            {
                ContainerId = container.GetHashCode();
                Element = element;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        class SingletonConsumer : ISingletonConsumer
        {
            public long ContainerId { get; }

            public bool IsDisposed { get; private set; }

            public ISingletonService SingletonService { get; }

            public SingletonConsumer(IUnityContainer container, ISingletonService singleton)
            {
                ContainerId = container.GetHashCode();
                SingletonService = singleton;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        class TestElement : ITestElement
        {
            public long ContainerId { get; }
            public bool IsDisposed { get; private set; }

            public TestElement(IUnityContainer container)
            {
                ContainerId = container.GetHashCode();
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        class TestElementFactory : ITestElementFactory
        {
            private readonly IUnityContainer _container;

            public TestElementFactory(IUnityContainer container)
            {
                _container = container;
            }

            public ITestElement CreateElement()
            {
                return _container.Resolve<ITestElement>();
            }
        }

        #endregion
    }
}
