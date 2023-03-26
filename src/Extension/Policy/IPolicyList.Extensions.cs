using System;
using System.Runtime.CompilerServices;

namespace Unity.Extension
{
    /// <summary>
    /// Extension methods to provide convenience overloads
    /// </summary>
    public static class PolicyListExtensions
    {
        #region Clear

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TTarget">Target <see cref="Type"/> the policy was registered under</typeparam>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TTarget, TPolicy>(this IPolicyList policies)
            where TPolicy : class => policies.Clear(typeof(TTarget), typeof(TPolicy));

        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to remove the policy from</param>
        /// <param name="target">The target <see cref="Type"/> the policy was registered under</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TPolicy>(this IPolicyList policies, Type target)
            where TPolicy : class => policies.Clear(target, typeof(TPolicy));

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
        public static TPolicy? Get<TTarget, TPolicy>(this IPolicyList policies)
            where TPolicy : class => (TPolicy?)policies.Get(typeof(TTarget), typeof(TPolicy));

        /// <summary>
        /// Gets a policy set on a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to get the policy from</param>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicyList policies, Type target)
            where TPolicy : class => (TPolicy?)policies.Get(target, typeof(TPolicy));

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
        public static void Set<TPolicy>(this IPolicyList policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TPolicy), policy);

        /// <summary>
        /// Sets a policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to</param>
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
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TTarget, TPolicy>(this IPolicyList policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TTarget), typeof(TPolicy), policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicies"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault<TPolicy>(this IPolicyList policies, TPolicy policy)
            where TPolicy : class => policies.Set(typeof(TPolicy), policy);

        #endregion
    }
}
