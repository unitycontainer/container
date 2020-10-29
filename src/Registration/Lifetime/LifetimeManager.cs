using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base class for all lifetime managers - classes that control how
    /// and when instances are created by the Unity container.
    /// </summary>
    public abstract partial class LifetimeManager : RegistrationManager
    {
        #region Constructors

        public LifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region LifetimeManager Members

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="scope">The container this lifetime is associated with</param>
        public virtual void SetValue(object? newValue, ICollection<IDisposable> scope) { }

        #endregion


        #region ICloneable

        /// <summary>
        /// Creates a new lifetime manager of the same type as this Lifetime Manager
        /// </summary>
        /// <returns>A new instance of the appropriate lifetime manager</returns>
        public LifetimeManager Clone()
        {
            var manager = OnCreateLifetimeManager();
            manager.CloneData(this);
            return manager;
        }

        public LifetimeManager Clone(params InjectionMember[] members)
        {
            var manager = OnCreateLifetimeManager();
            manager.CloneData(this, members);
            return manager;
        }

        #endregion


        #region Implementation

        /// <summary>
        /// Implementation of <see cref="Clone"/> policy.
        /// </summary>
        /// <returns>A new instance of the same lifetime manager of appropriate type</returns>
        protected abstract LifetimeManager OnCreateLifetimeManager();

        #endregion
    }
}
