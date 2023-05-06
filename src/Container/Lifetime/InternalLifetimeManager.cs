using Unity.Lifetime;

namespace Unity.Container
{
    /// <summary>
    /// Internal container lifetime manager. 
    /// </summary>
    internal class InternalLifetimeManager : LifetimeManager
    {
        #region Fields

        private readonly object? _value;

        #endregion


        #region Constructors

        internal InternalLifetimeManager(object? data, RegistrationCategory type = RegistrationCategory.Instance)
        {
            _value = data;
            Category = type;
        }

        internal InternalLifetimeManager(RegistrationCategory type)
        {
            _value = UnityContainer.NoValue;
            Category = type;
        }

        #endregion

        
        #region Value

        /// <inheritdoc/>
        public override object? TryGetValue(ILifetimeContainer _) 
            => _value;

        /// <inheritdoc/>
        public override object? GetValue(ILifetimeContainer _) 
            => _value;

        #endregion


        #region Implementation

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
            => throw new NotSupportedException();

        public override bool IsLocal => true;

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy => CreationPolicy.Once;

        /// <inheritdoc/>
        public override string ToString() => "Lifetime: Internal";
        
        #endregion
    }
}
