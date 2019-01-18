using System;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds onto the instance given to it.
    /// When the <see cref="ContainerControlledLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </summary>
    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager, 
                                                      IInstanceLifetimeManager, 
                                                      IFactoryLifetimeManager,
                                                      ITypeLifetimeManager
    {
        #region Fields

        protected object Value;
        private Func<ILifetimeContainer, object> _currentGetValue;
        private Action<object, ILifetimeContainer> _currentSetValue;

        #endregion

        public ContainerControlledLifetimeManager()
        {
            _currentGetValue = base.GetValue;
            _currentSetValue = base.SetValue;
        }

        public override object GetValue(ILifetimeContainer container = null)
        {
            return _currentGetValue(container);
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            _currentSetValue(newValue, container);
            _currentSetValue = (o, c) => throw new InvalidOperationException("InjectionParameterValue of ContainerControlledLifetimeManager can only be set once");
            _currentGetValue = SynchronizedGetValue;
        }
        
        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this WithLifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return Value;
        }

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container"></param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.SetValue"/>
        /// before releasing its lock.</remarks>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            Value = newValue;
        }


        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        /// <param name="container">Instance of container</param>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            Dispose();
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ContainerControlledLifetimeManager();
        }

        #region IDisposable

        /// <summary>		
        /// Standard Dispose pattern implementation.		
        /// </summary>		
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>		
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (Value == null) return;
                if (Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                Value = null;
            }
            finally 
            {
                base.Dispose(disposing);
            }
        }

        #endregion


        #region Overrides

        public override string ToString() => "Lifetime:PerContainer"; 

        #endregion
    }
}
