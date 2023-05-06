using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base class for Lifetime managers which need to synchronize calls to
    /// <see cref="GetValue"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The purpose of this class is to provide a basic implementation of the lifetime manager synchronization pattern.
    /// </para>
    /// <para>
    /// Calls to the <see cref="GetValue"/> method of a <see cref="SynchronizedLifetimeManager"/> 
    /// instance acquire a lock, and if the instance has not been initialized with a value yet the lock will only be released 
    /// when such an initialization takes place by calling the <see cref="SetValue"/> method or if 
    /// the build request which resulted in the call to the GetValue method fails.
    /// </para>
    /// </remarks>
    /// <see cref="LifetimeManager"/>
    public abstract class SynchronizedLifetimeManager : LifetimeManager

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


        #region Constructors

        public SynchronizedLifetimeManager(params InjectionMember[] members)
            : base(members)
        {

        }

        #endregion


        /// <inheritdoc/>
        public override object? TryGetValue(ILifetimeContainer scope) 
            => SynchronizedGetValue(scope);

        /// <inheritdoc/>
        public override object? GetValue(ILifetimeContainer scope)
        {
            if (Monitor.TryEnter(_lock, ResolveTimeout))
            {
                var result = SynchronizedGetValue(scope);
                if (UnityContainer.NoValue != result)
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
        /// <param name="scope">Instance of the lifetime's container</param>
        /// <remarks>This method is invoked by <see cref="GetValue"/>
        /// after it has acquired its lock.</remarks>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        protected abstract object? SynchronizedGetValue(ILifetimeContainer scope);


        /// <inheritdoc/>
        public override void SetValue(object? newValue, ILifetimeContainer scope)
        {
            SynchronizedSetValue(newValue, scope);
            TryExit();
        }

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="scope">Instance of the lifetime's container</param>
        /// <remarks>This method is invoked by <see cref="SetValue"/>
        /// before releasing its lock.</remarks>
        protected abstract void SynchronizedSetValue(object? newValue, ILifetimeContainer scope);

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
            if (!Monitor.IsEntered(_lock)) return;

            Monitor.Exit(_lock);
        }
    }
}
