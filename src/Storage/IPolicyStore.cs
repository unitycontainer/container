using System;
using Unity.Policy;

namespace Unity.Storage
{
    public interface IPolicyStore
    {
        /// <summary>
        /// Get policy
        /// </summary>
        /// <param name="policyInterface">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        IBuilderPolicy Get(Type policyInterface);

        /// <summary>
        /// Set policy
        /// </summary>
        /// <param name="policyInterface">Type of policy to be set</param>
        /// <param name="policy">Policy instance to be set</param>
        void Set(Type policyInterface, IBuilderPolicy policy);

        /// <summary>
        /// Remove specific policy from the list
        /// </summary>
        /// <param name="policyInterface">Type of policy to be removed</param>
        void Clear(Type policyInterface);

        /// <summary>
        /// Removes all policies from the list.
        /// </summary>
        void ClearAll();
    }
}
