using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that creates a new instance of 
    /// the registered <see cref="Type"/> once per each thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per thread lifetime means a new instance of the registered <see cref="Type"/>
    /// will be created once per each thread. In other words, if a Resolve{T}() method 
    /// is called on a thread the first time, it will return a new object. Each
    /// subsequent call to Resolve{T}(), or when the dependency mechanism injects 
    /// instances of the type into other classes on the same thread, the container 
    /// will return the same object.
    /// </para>
    /// <para>
    /// This LifetimeManager does not dispose the instances it holds.
    /// </para>
    /// </remarks>
    public class PerThreadLifetimeManager : LifetimeManager,
                                            IFactoryLifetimeManager,
                                            ITypeLifetimeManager
    {
        #region Fields

        private ThreadLocal<object?> _value = new ThreadLocal<object?>(() => UnityContainer.NoValue);

        #endregion


        #region Constructors

        public PerThreadLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? TryGetValue(ILifetimeContainer scope)
            => _value.Value;

        /// <inheritdoc/>
        public override object? GetValue(ILifetimeContainer scope) 
            => _value.Value;

        /// <inheritdoc/>
        public override void SetValue(object? newValue, ILifetimeContainer scope) 
            => _value.Value = newValue;

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy 
            => CreationPolicy.Always;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new PerThreadLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerThread";

        #endregion
    }
}
