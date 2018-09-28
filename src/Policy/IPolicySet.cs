using System;

namespace Unity.Policy
{
    public interface IPolicySet
    {
        /// <summary>
        /// Get policy
        /// </summary>
        /// <param name="policyInterface">Type of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        object Get(Type policyInterface);

        /// <summary>
        /// Set policy
        /// </summary>
        /// <param name="policyInterface">Type of policy to be set</param>
        /// <param name="policy">Policy instance to be set</param>
        void Set(Type policyInterface, object policy);

        /// <summary>
        /// Remove specific policy from the list
        /// </summary>
        /// <param name="policyInterface">Type of policy to be removed</param>
        void Clear(Type policyInterface);
    }

    public static class PolicySetExtensions
    {
        public static T Get<T>(this IPolicySet policySet)
        {
            return (T)policySet.Get(typeof(T));
        }

        public static void Set<T>(this IPolicySet policySet, object policy)
        {
            policySet.Set(typeof(T), policy);
        }
    }
}
