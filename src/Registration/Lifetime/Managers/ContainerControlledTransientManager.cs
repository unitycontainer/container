using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="TransientLifetimeManager"/>,
    /// except container remembers all Disposable objects it created. Once container
    /// is disposed all these objects are disposed as well.
    /// </summary>
    public class ContainerControlledTransientManager : LifetimeManager,
                                                       IFactoryLifetimeManager,
                                                       ITypeLifetimeManager
    {
        #region Constructors

        public ContainerControlledTransientManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        public override object? TryGetValue(ILifetimeContainer scope) 
            => UnityContainer.NoValue;

        public override object? GetValue(ILifetimeContainer scope)
            => UnityContainer.NoValue;

        /// <inheritdoc/>
        public override void SetValue(object? newValue, ILifetimeContainer scope)
        {
            if (newValue is IDisposable disposable) scope.Add(disposable);
        }

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy 
            => CreationPolicy.OnceInWhile;

        public override bool IsLocal => true;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new ContainerControlledTransientManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerContainerTransient";

        #endregion
    }
}
