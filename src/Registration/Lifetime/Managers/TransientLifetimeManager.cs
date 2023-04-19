using Unity.Injection;

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
        #region Constructors

        /// <inheritdoc/>
        public TransientLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? TryGetValue(ILifetimeContainer scope) 
            => UnityContainer.NoValue;

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy 
            => CreationPolicy.OnceInWhile;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new TransientLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:Transient";

        #endregion
    }
}
