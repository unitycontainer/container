using System;

namespace Unity.Extension
{
    /// <summary>
    /// Extension methods on <see cref="IPolicyList"/> to provide convenience overloads
    /// </summary>
    public static class PolicyListExtensions
    {
        #region Clear

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TInterface">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        public static void Clear<TPolicy, TInterface>(this IPolicyList policies)
            where TInterface : class => policies.Clear(typeof(TPolicy), typeof(TInterface));

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        /// <param name="policy">The policy</param>
        public static void Clear<TPolicy>(this IPolicyList policies, Type policy)
            where TPolicy : class => policies.Clear(typeof(TPolicy), policy);

        /// <summary>
        /// Removes a default policy
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        public static void ClearDefault<TPolicyInterface>(this IPolicyList policies)
            where TPolicyInterface : class => policies.Clear(null, typeof(TPolicyInterface));

        #endregion


        #region Get


        /// <summary>
        /// Gets an individual policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TInterface">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        public static TInterface? Get<TPolicy, TInterface>(this IPolicyList policies)
            where TPolicy : class => (TInterface?)policies.Get(typeof(TPolicy), typeof(TInterface));

        /// <summary>
        /// Gets an individual policy
        /// </summary>
        /// <typeparam name="TInterface">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search</param>
        /// <param name="type">The <see cref="Type"/> the policy was registered under</param>
        /// <returns>The policy, if present; returns null otherwise</returns>
        public static TInterface? Get<TInterface>(this IPolicyList policies, Type type)
            where TInterface : class => (TInterface?)policies.Get(type, typeof(TInterface));

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface to register the policy under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="instance">The default policy to be registered.</param>
        public static TPolicyInterface? GetDefault<TPolicyInterface>(this IPolicyList policies)
            where TPolicyInterface : class => (TPolicyInterface?)policies.Get(null, typeof(TPolicyInterface));

        #endregion


        #region Set

        /// <summary>
        /// Sets an individual policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> the policy implements</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to</param>
        /// <param name="target">The <see cref="Type"/> to register the policy under</param>
        /// <param name="instance">The default policy to be registered</param>
        public static void Set<TPolicy>(this IPolicyList policies, Type target, TPolicy instance)
            where TPolicy : class => policies.Set(target, typeof(TPolicy), instance);

        /// <summary>
        /// Sets an default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> to register the policy under</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to</param>
        /// <param name="policy">The <see cref="Type"/> the policy implements</param>
        /// <param name="instance">The default policy to be registered</param>
        public static void Set(this IPolicyList policies, Type policy, object instance)
            => policies.Set(null, policy, instance);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> the policy implements</param>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="instance">The default policy to be registered.</param>
        public static void SetDefault<TPolicy>(this IPolicyList policies, TPolicy instance)
            where TPolicy : class => policies.Set(null, typeof(TPolicy), instance);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The <see cref="Type"/> the policy implements</param>
        /// <param name="instance">The default policy to be registered.</param>
        public static void SetDefault(this IPolicyList policies, Type policy, object instance)
            => policies.Set(null, policy, instance);

        #endregion
    }
}
