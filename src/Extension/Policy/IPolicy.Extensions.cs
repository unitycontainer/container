using System;
using System.Runtime.CompilerServices;

namespace Unity.Extension
{
    /// <summary>
    /// Extension methods to provide convenience overloads
    /// </summary>
    public static class PolicyExtensions
    {
        #region Constants

        const string ERROR_NO_LOCAL = "This method is deprecated. Local policies are no longer supported";
        const string ERROR_NO_CASCADING = "This method is deprecated. Cascading policies are no longer supported";
        const string ERROR_NO_LOCAL_CASCADING = "This method is deprecated. Local, cascading policies are no longer supported";

        #endregion


        #region Clear


        #region PolicyList

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TTarget">Target <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TTarget, TPolicy>(this IPolicyList policies)
            where TPolicy : class => policies.Clear(typeof(TTarget), typeof(TPolicy));

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        /// <param name="target">The target <see cref="Type"/> the policy was registered under</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TPolicy>(this IPolicyList policies, Type target)
            where TPolicy : class => policies.Clear(target, typeof(TPolicy));

        /// <summary>
        /// Removes a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The type the policy was registered as</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearDefault<TPolicy>(this IPolicyList policies)
            where TPolicy : class => policies.Clear(null, typeof(TPolicy));

        #endregion


        #endregion


        #region Get


        #region PolicyList

        /// <summary>
        /// Gets a policy set on a type
        /// </summary>
        /// <typeparam name="TTarget">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to get the policy from</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TTarget, TPolicy>(this IPolicyList policies)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TTarget), typeof(TPolicy));

        /// <summary>
        /// Gets a policy set on a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to get the policy from</param>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicyList policies, Type target)
            where TPolicy : class => (TPolicy?)policies.Get(target, typeof(TPolicy));

        /// <summary>
        /// Gets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The interface to register the policy under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? GetDefault<TPolicy>(this IPolicyList policies)
            where TPolicy : class => (TPolicy?)policies.Get(null, typeof(TPolicy));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this IPolicyList defaults, Type? target, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(target, typeof(TPolicy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this IPolicyList defaults, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(null, typeof(TPolicy)));


        #endregion


        #region Observable

        /// <summary>
        /// Gets a policy set on a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TTarget">The <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyObservable"/> to get the policy from</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TTarget, TPolicy>(this IPolicyObservable policies, PolicyChangeHandler handler)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TTarget), typeof(TPolicy), handler);

        /// <summary>
        /// Gets a policy set on a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyObservable"/> to get the policy from</param>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicyObservable policies, Type target, PolicyChangeHandler handler)
            where TPolicy : class => (TPolicy?)policies.Get(target, typeof(TPolicy), handler);

        /// <summary>
        /// Gets a default policy and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyObservable"/> to get the policy from</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicyObservable policies, PolicyChangeHandler handler)
            where TPolicy : class => (TPolicy?)policies.Get(null, typeof(TPolicy), handler);

        #endregion


        #endregion


        #region Set


        #region PolicyList

        /// <summary>
        /// Sets a default policy
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to</param>
        /// <param name="type">The <see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(this IPolicyList policies, Type type, object policy)
            => policies.Set(null, type, policy);

        /// <summary>
        /// Sets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicyList policies, TPolicy policy)
            where TPolicy : class => policies.Set(null, typeof(TPolicy), policy);

        /// <summary>
        /// Sets a policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to</param>
        /// <param name="target">The <see cref="Type"/> to register policy under</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicyList policies, Type target, TPolicy policy)
            where TPolicy : class => policies.Set(target, typeof(TPolicy), policy);

        /// <summary>
        /// Sets a policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <typeparam name="TTarget">The <see cref="Type"/> to register policy under</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TTarget, TPolicy>(this IPolicyList policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TTarget), typeof(TPolicy), policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="type">The <see cref="Type"/> the policy implements</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault(this IPolicyList policies, Type type, object policy)
            => policies.Set(null, type, policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault<TPolicy>(this IPolicyList policies, TPolicy policy)
            where TPolicy : class => policies.Set(null, typeof(TPolicy), policy);

        #endregion


        #region Observable

        /// <summary>
        /// Sets a default policy and subscribes to notifications
        /// </summary>
        /// <param name="policies"><see cref="IPolicyObservable"/> to add the policy to</param>
        /// <param name="type">The <see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(this IPolicyObservable policies, Type type, object policy, PolicyChangeHandler handler)
            => policies.Set(null, type, policy, handler);

        /// <summary>
        /// Sets a default policy and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicyObservable"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicyObservable policies, TPolicy policy, PolicyChangeHandler handler)
            where TPolicy : class => policies.Set(null, typeof(TPolicy), policy, handler);

        /// <summary>
        /// Sets a policy for a type and subscribes to notifications
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicyObservable"/> to add the policy to</param>
        /// <param name="target">The <see cref="Type"/> to registered policy under</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicyObservable policies, Type target, TPolicy policy, PolicyChangeHandler handler)
            where TPolicy : class => policies.Set(target, typeof(TPolicy), policy, handler);

        #endregion


        #endregion


        #region Deprecated

        [Obsolete("This method is deprecated. Use 'Clear<TPolicyInterface>(Type policy)' instead", true)]
        public static void Clear<TPolicyInterface>(this IPolicyList policies, object buildKey)
            => throw new NotImplementedException();

        [Obsolete("This method is deprecated. Use 'Get<TPolicyInterface>(Type policy)' instead", true)]
        public static TPolicyInterface? Get<TPolicyInterface>(this IPolicyList policies, object buildKey)
            => throw new NotImplementedException();

        [Obsolete("This method is deprecated. Use 'Set<TPolicyInterface>(Type policy, TPolicyInterface instance)' instead", true)]
        public static void Set<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy, object buildKey)
            => throw new NotImplementedException();

        [Obsolete("This method is deprecated. Use 'Get(Type? type, Type policy)' instead", true)]
        public static object Get(this IPolicyList policies, Type policyInterface, object buildKey)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_CASCADING, true)]
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_CASCADING, true)]
        public static object? Get(this IPolicyList policies, Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static object? Get(this IPolicyList policies, Type policyInterface, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static object? GetNoDefault(this IPolicyList policies, Type policyInterface, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL_CASCADING, true)]
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL_CASCADING, true)]
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        #endregion
    }
}
