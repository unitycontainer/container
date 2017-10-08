// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Base class for Lifetime managers - classes that control how
    /// and when instances are created by the Unity container.
    /// </summary>
    public abstract class LifetimeManager : ILifetimePolicy
    {
        // Get or set the InUse flag. Internal because it should only be touched from
        // the Register methods in the container.
        public bool InUse { get; set; }

        #region ILifetimePolicy Members

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public abstract object GetValue();

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public abstract void SetValue(object newValue);

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public abstract void RemoveValue();

        #endregion
    }
}
