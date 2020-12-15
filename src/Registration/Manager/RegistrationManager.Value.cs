using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager : IEnumerable
    {
        #region Invalid Value object

        /// <summary>
        /// This value represents Invalid Value. Lifetime manager must return this
        /// unless value is set with a valid object. Null is a value and is not equal 
        /// to NoValue 
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        #endregion


        #region Try/Get/Set Value

        /// <summary>
        /// Attempts to retrieve a value from the backing lifetime manager
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on lifetime 
        /// synchronization objects primitives.
        /// </remarks>
        /// <param name="scope">The lifetime container this manager is associated with</param>
        /// <returns>The object stored with the manager or <see cref="NoValue"/></returns>
        public virtual object? TryGetValue(ICollection<IDisposable> scope) => NoValue;

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="scope">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? GetValue(ICollection<IDisposable> scope) => NoValue;

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="scope">The container this lifetime is associated with</param>
        public virtual void SetValue(object? newValue, ICollection<IDisposable> scope) { }

        #endregion


        #region Nested Types

        public sealed class InvalidValue
        {
            internal InvalidValue()
            {
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object? obj) 
                => ReferenceEquals(this, obj);

            public override int GetHashCode() 
                => base.GetHashCode();
        }

        #endregion
    }
}
