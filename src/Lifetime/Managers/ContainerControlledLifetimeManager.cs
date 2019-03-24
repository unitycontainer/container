using System;

namespace Unity.Lifetime
{
    /// <summary>
    /// <para>
    /// Unity returns the same instance each time the Resolve(...) method is called or when the
    /// dependency mechanism injects the instance into other classes.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per Container lifetime allows a registration of an existing or resolved object as 
    /// a scoped singleton in the container it was created or registered. In other words 
    /// this instance is unique within the container it war registered with. Child or parent 
    /// containers could have their own instances registered for the same contract.
    /// </para>
    /// <para>
    /// When the <see cref="ContainerControlledLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </para>
    /// </remarks>
    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager, 
                                                      IInstanceLifetimeManager, 
                                                      IFactoryLifetimeManager,
                                                      ITypeLifetimeManager
    {
        #region Fields

        /// <summary>
        /// An instance of the object this manager is associated with.
        /// </summary>
        /// <value>This field holds a strong reference to the associated object.</value>
        protected object Value;

        private Func<ILifetimeContainer, object> _currentGetValue;
        private Action<object, ILifetimeContainer> _currentSetValue;

        #endregion


        #region Constructor

        public ContainerControlledLifetimeManager()
        {
            _currentGetValue = base.GetValue;
            _currentSetValue = base.SetValue;
        }

        #endregion


        #region SynchronizedLifetimeManager

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return _currentGetValue(container);
        }

        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            _currentSetValue(newValue, container);
            _currentSetValue = (o, c) => throw new InvalidOperationException("InjectionParameterValue of ContainerControlledLifetimeManager can only be set once");
            _currentGetValue = SynchronizedGetValue;
        }

        /// <inheritdoc/>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return Value;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            Value = newValue;
        }

        /// <inheritdoc/>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            Dispose();
        }

        #endregion


        #region IFactoryLifetimeManager

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ContainerControlledLifetimeManager();
        }

        #endregion


        #region IDisposable

        /// <inheritdoc/>
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

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerContainer"; 

        #endregion
    }
}
