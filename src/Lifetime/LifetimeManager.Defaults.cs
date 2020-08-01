using System;

namespace Unity.Lifetime
{
    public abstract partial class LifetimeManager
    {
        #region Fields

        internal static ITypeLifetimeManager _typeManager = new TransientLifetimeManager();
        internal static IFactoryLifetimeManager _factoryManager = new TransientLifetimeManager();
        internal static IInstanceLifetimeManager _instanceManager = new ContainerControlledLifetimeManager();

        #endregion


        #region Invalid Value object

        /// <summary>
        /// This value represents Invalid Value. Lifetime manager must return this
        /// unless value is set with a valid object. Null is a value and is not equal 
        /// to NoValue 
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        #endregion


        #region Default Registration Lifetimes

        public static ITypeLifetimeManager DefaultTypeLifetime 
        {
            get => _typeManager;
            set
            { 
                _typeManager = value ?? throw new ArgumentNullException(nameof(DefaultTypeLifetime));
            }
        }

        public static IFactoryLifetimeManager DefaultFactoryLifetime
        {
            get => _factoryManager;
            set
            {
                _factoryManager = value ?? throw new ArgumentNullException(nameof(DefaultFactoryLifetime));
            }
        }

        public static IInstanceLifetimeManager DefaultInstanceLifetime
        {
            get => _instanceManager;
            set
            {
                _instanceManager = value ?? throw new ArgumentNullException(nameof(DefaultInstanceLifetime));
            }
        }

        #endregion


        #region Nested Types

        public sealed class InvalidValue
        {
            internal InvalidValue()
            {
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion
    }
}
