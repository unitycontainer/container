using System.Runtime.CompilerServices;

namespace Unity.Policy
{
    /// <summary>
    /// Extension methods to provide convenience overloads
    /// </summary>
    public static class PoliciesExtensions
    {
        #region Clear

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TTarget">Target <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TTarget, TPolicy>(this IPolicies policies)
            where TPolicy : class => policies.Clear(typeof(TTarget), typeof(TPolicy));

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to remove the policy from</param>
        /// <param name="target">The target <see cref="Type"/> the policy was registered under</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TPolicy>(this IPolicies policies, Type target)
            where TPolicy : class => policies.Clear(target, typeof(TPolicy));

        /// <summary>
        /// Removes a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The type the policy was registered as</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearDefault<TPolicy>(this IPolicies policies)
            where TPolicy : class => policies.Clear(typeof(TPolicy));

        #endregion


        #region Get

        /// <summary>
        /// Gets a policy set on a type
        /// </summary>
        /// <typeparam name="TTarget">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to get the policy from</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TTarget, TPolicy>(this IPolicies policies)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TTarget), typeof(TPolicy));

        /// <summary>
        /// Gets a policy set on a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to get the policy from</param>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicies policies, Type target)
            where TPolicy : class => (TPolicy?)policies.Get(target, typeof(TPolicy));

        /// <summary>
        /// Gets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicies policies)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TPolicy));

        /// <summary>
        /// Gets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? GetDefault<TPolicy>(this IPolicies policies)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TPolicy));


        /// <summary>
        /// Gets a policy set on a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TTarget">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to get the policy from</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TTarget, TPolicy>(this IPolicies policies, PolicyChangeHandler handler)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TTarget), typeof(TPolicy), handler);

        /// <summary>
        /// Gets a policy set on a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to get the policy from</param>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicies policies, Type target, PolicyChangeHandler handler)
            where TPolicy : class => (TPolicy?)policies.Get(target, typeof(TPolicy), handler);

        /// <summary>
        /// Gets a default policy and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to get the policy from</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicies policies, PolicyChangeHandler handler)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TPolicy), handler);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this IPolicies defaults, Type target, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(target, typeof(TPolicy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this IPolicies defaults, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(typeof(TPolicy)));

        #endregion


        #region Set


        /// <summary>
        /// Sets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicies policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TPolicy), policy);

        /// <summary>
        /// Sets a policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to</param>
        /// <param name="target">The <see cref="Type"/> to register policy under</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicies policies, Type target, TPolicy policy)
            where TPolicy : class => policies.Set(target, typeof(TPolicy), policy);

        /// <summary>
        /// Sets a policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <typeparam name="TTarget">The <see cref="Type"/> to register policy under</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TTarget, TPolicy>(this IPolicies policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TTarget), typeof(TPolicy), policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <param name="type">The <see cref="Type"/> the policy implements</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault(this IPolicies policies, Type type, object policy)
            => policies.Set(type, policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault<TPolicy>(this IPolicies policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TPolicy), policy);


        /// <summary>
        /// Sets a policy for a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TTarget">The <see cref="Type"/> to register policy under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TTarget, TPolicy>(this IPolicies policies, TPolicy policy, PolicyChangeHandler handler)
            => policies.Set(typeof(TTarget), typeof(TPolicy), policy, handler);

        /// <summary>
        /// Sets a policy for a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to</param>
        /// <param name="target">The <see cref="Type"/> to registered policy under</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicies policies, Type target, TPolicy policy, PolicyChangeHandler handler)
            where TPolicy : class => policies.Set(target, typeof(TPolicy), policy, handler);

        /// <summary>
        /// Sets a default policy and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicies policies, TPolicy policy, PolicyChangeHandler handler)
            where TPolicy : class => policies.Set(typeof(TPolicy), policy, handler);

        #endregion


        #region  Compare Exchange


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? CompareExchange<TPolicy>(this IPolicies policies, TPolicy policy, TPolicy? comparand)
            where TPolicy : class => policies.CompareExchange(null, policy, comparand);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? CompareExchange<TPolicy>(this IPolicies policies, TPolicy policy, TPolicy? comparand, PolicyChangeHandler handler)
            where TPolicy : class => policies.CompareExchange(null, policy, comparand, handler);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? CompareExchange<TPolicy>(this IPolicies policies, Type target, TPolicy policy, TPolicy? comparand)
            where TPolicy : class => policies.CompareExchange(target, policy, comparand);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? CompareExchange<TTarget, TPolicy>(this IPolicies policies, TPolicy policy, TPolicy? comparand)
            where TPolicy : class => policies.CompareExchange(typeof(TTarget), policy, comparand);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? CompareExchange<TPolicy>(this IPolicies policies, Type target, TPolicy policy, TPolicy? comparand, PolicyChangeHandler handler)
            where TPolicy : class => policies.CompareExchange(target, policy, comparand, handler);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? CompareExchange<TTarget, TPolicy>(this IPolicies policies, TPolicy policy, TPolicy? comparand, PolicyChangeHandler handler)
            where TPolicy : class => policies.CompareExchange(typeof(TTarget), policy, comparand, handler);

        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy GetOrAdd<TPolicy>(this IPolicies policies, TPolicy policy, PolicyChangeHandler handler)
            where TPolicy : class => policies.CompareExchange(null, policy, null, handler) ?? policy;

    }
}
