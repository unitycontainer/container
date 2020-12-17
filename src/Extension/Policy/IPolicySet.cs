using System;

namespace Unity.Extension
{
    /// <summary>
    /// This interface for manipulation of policies
    /// </summary>
    public interface IPolicySet
    {
        /// <summary>
        /// Get policy
        /// </summary>
        /// <param name="type"><see cref="Type"/> of policy to retrieve</param>
        /// <returns>Instance of the policy or null if none found</returns>
        object? Get(Type type);

        /// <summary>
        /// Set policy
        /// </summary>
        /// <param name="type"><see cref="Type"/> of policy to set</param>
        /// <param name="policy">Policy instance</param>
        void Set(Type type, object policy);

        /// <summary>
        /// Remove policy
        /// </summary>
        /// <param name="type"><see cref="Type"/> of policy to be removed</param>
        void Clear(Type type);
    }

    public static class PolicySetExtensions
    {
        public static T? Get<T>(this IPolicySet policySet) where T : class
        {
            return (T?)policySet.Get(typeof(T));
        }

        public static void Set<T>(this IPolicySet policySet, object policy)
        {
            policySet.Set(typeof(T), policy);
        }
    }
}
