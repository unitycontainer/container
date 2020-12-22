using System;

namespace Unity.Extension
{
    /// <summary>
    /// Extension methods on <see cref="IPolicySet"/> to provide convenience overloads
    /// </summary>

    public static class PolicySetExtensions
    {
        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to remove the policy from</param>
        public static void Clear<TPolicyInterface>(this IPolicySet policies)
            where TPolicyInterface : class => policies.Clear(typeof(TPolicyInterface));


        /// <summary>
        /// Gets an individual policy
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to search</param>
        /// <returns>The policy in the set, if present; returns null otherwise</returns>
        public static TPolicyInterface? Get<TPolicyInterface>(this IPolicySet policies)
            where TPolicyInterface : class => (TPolicyInterface?)policies.Get(typeof(TPolicyInterface));


        /// <summary>
        /// Sets an individual policy
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface to register the policy under.</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to add the policy to.</param>
        /// <param name="policy">The policy to be registered.</param>
        public static void Set<TPolicyInterface>(this IPolicySet policies, Type type, TPolicyInterface policy)
            where TPolicyInterface : class => policies.Set(typeof(TPolicyInterface), policy);
    }
}
