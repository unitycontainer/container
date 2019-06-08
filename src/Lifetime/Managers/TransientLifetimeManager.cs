namespace Unity.Lifetime
{
    /// <summary>
    /// An <see cref="LifetimeManager"/> implementation that does nothing,
    /// thus ensuring that instances are created new every time.
    /// </summary>
    /// <remarks>
    /// Transient lifetime is a default lifetime of the Unity container. As 
    /// the name implies it lasts very short period of time, actually, no 
    /// time at all. In the Unity container terms, having transient lifetime 
    /// is the same as having no lifetime manager at all.
    /// </remarks>
    public class TransientLifetimeManager : LifetimeManager,
                                            IFactoryLifetimeManager,
                                            ITypeLifetimeManager
    {
        #region Fields

        /// <summary>
        /// Globally unique transient lifetime manager singleton
        /// </summary>
        /// <remarks>
        /// This instance is used for all transient lifetimes
        /// </remarks>
        /// <value>An instance of a <see cref="TransientLifetimeManager"/> object.</value>
        public static readonly TransientLifetimeManager Instance = new TransientLifetimeManager();

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override bool InUse { get => false; set { } }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() => new TransientLifetimeManager();

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:Transient";

        #endregion
    }
}
