// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Lifetime
{
    /// <summary>
    /// An <see cref="LifetimeManager"/> implementation that does nothing,
    /// thus ensuring that instances are created new every time.
    /// </summary>
    public class TransientLifetimeManager : LifetimeManager
    {
        public override bool InUse
        {
            get => false;
            set { }
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return null;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">Instance of container which owns the value</param>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// <param name="container">Instance of container</param>
        /// </summary>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
        }
    }
}
