namespace Unity.Lifetime
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerResolveLifetimeManager : LifetimeManager, 
                                             IInstanceLifetimeManager, 
                                             IFactoryLifetimeManager,
                                             ITypeLifetimeManager
    {
        protected object value;

        /// <summary>
        /// Construct a new <see cref="PerResolveLifetimeManager"/> object that does not
        /// itself manage an instance.
        /// </summary>
        public PerResolveLifetimeManager()
        {
            value = null;
        }

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return value;
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerResolveLifetimeManager();
        }


        #region Overrides

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerResolve";

        #endregion
    }
}
