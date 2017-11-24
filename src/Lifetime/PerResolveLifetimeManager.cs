// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Policy;

namespace Unity.Lifetime
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerResolveLifetimeManager : LifetimeManager, IRequireBuildUpPolicy
    {
        protected object value;

        /// <summary>
        /// Construct a new <see cref="PerResolveLifetimeManager"/> object that does not
        /// itself manage an instance.
        /// </summary>
        public PerResolveLifetimeManager()
        {
            value = null;
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return value;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later. In this class,
        /// this is a noop, since it has special hooks down in the guts.
        /// </summary>
        /// <param name="container">Instance of container which owns the value</param>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
        }

        /// <summary>
        /// Remove the given object from backing store. Noop in this class.
        /// </summary>
        /// <param name="container">Instance of container</param>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
        }
    }
}
