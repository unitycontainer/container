using System;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Regression
{
    public interface IService { }

    public interface IService1 { }
    
    public interface IService2 { }

    public class Service : IService, IService1, IService2, IDisposable
    {
        #region Fields

        public readonly string Id = Guid.NewGuid().ToString();
        public static int Instances;

        #endregion


        #region Constructors

        public Service()
        {
            Interlocked.Increment(ref Instances);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        #endregion


        #region Properties

        public int ThreadId { get; }
        public int Disposals { get; protected set; }
        public bool IsDisposed 
        { 
            get => 0 < Disposals;
            private set
            {
                if (value)
                    Disposals += 1;
                else
                    Disposals = 0;
            }
        }

        #endregion


        #region IDisposable

        public void Dispose() => IsDisposed = true;

        #endregion
    }


    public interface IOtherService { }

    public class OtherService : IService, IOtherService, IDisposable
    {
        [InjectionConstructor]
        public OtherService()
        {

        }

        public OtherService(IUnityContainer container)
        {

        }


        public bool Disposed;
        public void Dispose()
        {
            Disposed = true;
        }
    }
}


