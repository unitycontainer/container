using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base class for all lifetime managers - classes that control how
    /// and when instances are created by the Unity container.
    /// </summary>
    public abstract partial class LifetimeManager : RegistrationManager
    {
        /// <summary>
        /// This value represents Invalid Value. Lifetime manager must return this
        /// unless value is set with a valid object. Null is a value and is not equal 
        /// to NoValue 
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        
        #region Constructors

        public LifetimeManager(params InjectionMember[] members)
            : base(members)
        {
            Set    = SetValue;
            Get    = GetValue;
            TryGet = TryGetValue;
        }

        #endregion


        #region  Optimizers

        /// <summary>
        /// The property holding a method that attempts to get value. 
        /// Synchronized lifetime managers will not set a lock by calling the method.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<ICollection<IDisposable>, object?> TryGet { get; protected set; }

        /// <summary>
        /// The property holding a method that gets the value. 
        /// Synchronized lifetime managers will set a lock by calling the method.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<ICollection<IDisposable>, object?> Get { get; protected set; }

        /// <summary>
        /// The property holding a method that sets the value. 
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Action<object?, ICollection<IDisposable>> Set { get; protected set; }

        #endregion


        #region   LifetimeManager Members

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on synchronization 
        /// primitives.
        /// </remarks>
        /// <param name="lifetime">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? TryGetValue(ICollection<IDisposable> lifetime) => GetValue(lifetime);

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="lifetime">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? GetValue(ICollection<IDisposable> lifetime) => NoValue;

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="lifetime">The container this lifetime is associated with</param>
        public virtual void SetValue(object? newValue, ICollection<IDisposable> lifetime) { }

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
