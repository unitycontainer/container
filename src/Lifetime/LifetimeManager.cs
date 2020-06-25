using System;
using System.Diagnostics;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base class for all lifetime managers - classes that control how
    /// and when instances are created by the Unity container.
    /// </summary>
    public abstract class LifetimeManager
    {
        #region Fields

        private object? _scope;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Registration Registration;

        #endregion


        /// <summary>
        /// This value represents Invalid Value. Lifetime manager must return this
        /// unless value is set with a valid object. Null is a value and is not equal 
        /// to NoValue 
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        /// <summary>
        /// A <see cref="Boolean"/> indicating if this manager is being used in 
        /// one of the registrations.
        /// </summary>
        /// <remarks>
        /// The Unity container requires that each registration used its own, unique
        /// lifetime manager. This property is being used to track that condition.
        /// </remarks>
        /// <value>True is this instance already in use, False otherwise.</value>
        public virtual bool InUse { get; set; }

        
        #region Constructors

        public LifetimeManager()
        {
            Set    = SetValue;
            Get    = GetValue;
            TryGet = TryGetValue;
        }

        #endregion


        #region Scope

        /// <summary>
        /// This is a reference to the container this manager is registered with.
        /// </summary>
        public object? Scope
        {
            get => _scope; 
            set
            {
                System.Diagnostics.Debug.Assert(null == _scope, $"Manager {this} is already registered with {_scope} scope");
                
                _scope = value;
            }
        }

        #endregion


        #region  Optimizers

        /// <summary>
        /// The property holding a method that attempts to get value. 
        /// Synchronized lifetime managers will not set a lock by calling the method.
        /// </summary>
        public Func<ILifetimeContainer?, object?> TryGet { get; protected set; }

        /// <summary>
        /// The property holding a method that gets the value. 
        /// Synchronized lifetime managers will set a lock by calling the method.
        /// </summary>
        public Func<ILifetimeContainer?, object?> Get { get; protected set; }

        /// <summary>
        /// The property holding a method that sets the value. 
        /// </summary>
        public Action<object?, ILifetimeContainer?> Set { get; protected set; }

        #endregion

// TODO: Verify if container could ever be null

        #region   LifetimeManager Members

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on synchronization 
        /// primitives.
        /// </remarks>
        /// <param name="container">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? TryGetValue(ILifetimeContainer? container = null) => GetValue(container);

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? GetValue(ILifetimeContainer? container = null) => NoValue;

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">The container this lifetime is associated with</param>
        public virtual void SetValue(object? newValue, ILifetimeContainer? container = null) { }

        #endregion


        #region ICloneable

        /// <summary>
        /// Creates a new lifetime manager of the same type as this Lifetime Manager
        /// </summary>
        /// <returns>A new instance of the appropriate lifetime manager</returns>
        public LifetimeManager Clone() => OnCreateLifetimeManager();

        #endregion


        #region Implementation

        /// <summary>
        /// Implementation of <see cref="Clone"/> policy.
        /// </summary>
        /// <returns>A new instance of the same lifetime manager of appropriate type</returns>
        protected abstract LifetimeManager OnCreateLifetimeManager();

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
