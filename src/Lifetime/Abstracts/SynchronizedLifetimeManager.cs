using System;
using System.Threading;

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

        private readonly object _lock = new object();

        /// <summary>
        /// This field controls how long the monitor will wait to 
        /// enter the lock. It is <see cref="Timeout.Infinite"/> by default or number of 
        /// milliseconds from 0 to 2147483647.
        /// </summary>
        public static int ResolveTimeout = Timeout.Infinite;

        #endregion

        /// <inheritdoc/>
        public override object TryGetValue(ILifetimeContainer container = null)
        {
            if (Monitor.TryEnter(_lock))
            {
                var result = SynchronizedGetValue(container);
                Monitor.Exit(_lock);
                return result;
            }
            else
                return NoValue;
        }


        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            if (Monitor.TryEnter(_lock, ResolveTimeout))
            {
                var result = SynchronizedGetValue(container);
                if (NoValue != result)
                {
                    Monitor.Exit(_lock);
                }
                return result;
            }
            else
                throw new TimeoutException($"Failed to enter a monitor");
        }

        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of the lifetime's container</param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        protected abstract object SynchronizedGetValue(ILifetimeContainer container);


        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            SynchronizedSetValue(newValue, container);
            TryExit();
        }

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

        protected virtual void TryExit()
        {
#if !NET40
            // Prevent first chance exception when abandoning a lock that has not been entered
            if (!Monitor.IsEntered(_lock)) return;
#endif
            try
            {
                Monitor.Exit(_lock);
            }
            catch (SynchronizationLockException)
            {
                // Noop here - we don't hold the lock and that's ok.
            }
        }


        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
