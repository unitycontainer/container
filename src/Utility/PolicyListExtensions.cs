using System;
using Unity.Storage;

// ReSharper disable once CheckNamespace

namespace Unity.Policy
{
    /// <summary>
    /// Extension methods on <see cref="IPolicyList"/> to provide convenience
    /// overloads (generic versions, mostly).
    /// </summary>
    public static class PolicyListUtilityExtensions
    {
        /// <summary>
        /// Sets a generic individual policy.
        /// </summary>
        /// <typeparam name="T">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="type">Type of registration to add this policy to</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="policy">Policy to be set</param>
        public static void Set<T>(this IPolicyList policies, Type type, string name, Type type1, object policy)
        {
            policies.Set(type, name, typeof(T), policy);
        }

        public static T Get<T>(this IPolicyList policies)
        {
            return (T)policies.Get(null, null, typeof(T));
        }


        public static T Get<T>(this IPolicyList policies, Type type, string name)
        {
            return (T)policies.Get(type, name, typeof(T));
        }
    }
}
