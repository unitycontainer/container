using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base class for Lifetime managers which need to synchronize calls to
    /// <see cref="SynchronizedLifetimeManager.GetValue"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The purpose of this class is to provide a basic implementation of the lifetime manager synchronization pattern.
    /// </para>
    /// <para>
    /// Calls to the <see cref="SynchronizedLifetimeManager.GetValue"/> method of a <see cref="SynchronizedLifetimeManager"/> 
    /// instance acquire a lock, and if the instance has not been initialized with a value yet the lock will only be released 
    /// when such an initialization takes place by calling the <see cref="SynchronizedLifetimeManager.SetValue"/> method or if 
    /// the build request which resulted in the call to the GetValue method fails.
    /// </para>
    /// </remarks>
    /// <see cref="LifetimeManager"/>
    public abstract class SynchronizedLifetimeManager : LifetimeManager, IDisposable

    {
        #region Fields

        /// <summary>
        /// This field controls how long the monitor will wait to 
        /// enter the lock. It is <see cref="Timeout.Infinite"/> by default or number of 
        /// milliseconds from 0 to 2147483647.
        /// </summary>
        public static int ResolveTimeout = Timeout.Infinite;

        #endregion


        #region Constructors

        public SynchronizedLifetimeManager()
        {
            GetResult = GetValue;
            SetResult = SetValue;

            GetTask = (c) => throw new NotImplementedException();
            SetTask = (t, c) => throw new NotImplementedException();
        }

        #endregion


        #region ILifetimeManagerAsync

        public Func<ILifetimeContainer, object> GetResult { get; protected set; }

        public Action<object, ILifetimeContainer> SetResult { get; protected set; }

        public Func<ILifetimeContainer, Task<object>> GetTask { get; protected set; }

        public Func<Task<object>, ILifetimeContainer, Task<object>> SetTask { get; protected set; }

        #endregion


        #region LifetimeManager

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            if (Monitor.TryEnter(SyncRoot, ResolveTimeout))
            {
                var result = SynchronizedGetValue(container);
                if (NoValue != result)
                {
                    Monitor.Exit(SyncRoot);
                }
                return result;
            }
            else
                throw new TimeoutException($"Failed to enter a monitor");
        }

        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            SynchronizedSetValue(newValue, container);
            TryExit();
        }

        #endregion


        #region SynchronizedLifetimeManager

        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of the lifetime's container</param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        protected abstract object SynchronizedGetValue(ILifetimeContainer container);

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">Instance of the lifetime's container</param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.SetValue"/>
        /// before releasing its lock.</remarks>
        protected abstract void SynchronizedSetValue(object newValue, ILifetimeContainer container);

        /// <summary>
        /// A method that does whatever is needed to clean up
        /// as part of cleaning up after an exception.
        /// </summary>
        /// <remarks>
        /// Don't do anything that could throw in this method,
        /// it will cause later recover operations to get skipped
        /// and play real havoc with the stack trace.
        /// </remarks>
        public void Recover()
        {
            TryExit();
        }

        #endregion


        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Implementation

        protected virtual void TryExit()
        {
#if !NET40
            // Prevent first chance exception when abandoning a lock that has not been entered
            if (!Monitor.IsEntered(SyncRoot)) return;
#endif
            try
            {
                Monitor.Exit(SyncRoot);
            }
            catch (SynchronizationLockException)
            {
                // Ignore.
            }
        }

        /// <summary>		
        /// Standard Dispose pattern implementation.		
        /// </summary>		
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>		
        protected virtual void Dispose(bool disposing)
        {
            TryExit();
        }

        #endregion
    }
}
