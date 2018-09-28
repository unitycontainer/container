using System;

namespace Unity.Policy
{
    /// <summary>
    /// A custom collection over <see cref="Object"/> objects.
    /// </summary>
    public interface IPolicyList
    {
        /// <summary>
        /// Get policy
        /// </summary>
        /// <param name="type">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="policyInterface">Type of policy to retrieve</param>
        /// <param name="list">Reference to owner IList holding reference to the policy</param>
        /// <returns>Instance of the policy or null if none found</returns>
        object Get(Type type, string name, Type policyInterface);

        /// <summary>
        /// Set policy
        /// </summary>
        /// <param name="type">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="policyInterface">Type of policy to be set</param>
        /// <param name="policy">Policy instance to be set</param>
        void Set(Type type, string name, Type policyInterface, object policy);

        /// <summary>
        /// Remove specific policy from the list
        /// </summary>
        /// <param name="type">Type of the registration</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="policyInterface">Type of policy to be removed</param>
        void Clear(Type type, string name, Type policyInterface);
    }
}
