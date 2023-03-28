using System;
using System.Runtime.CompilerServices;

namespace Unity.Extension
{
    /// <summary>
    /// Extension methods to provide convenience overloads
    /// </summary>
    public static class PolicySetExtensions
    {
        #region Clear


        /// <summary>
        /// Removes an individual policy for a type
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<TPolicy>(this IPolicySet set)
            where TPolicy : class => set.Clear(typeof(TPolicy));

        /// <summary>
        /// Removes a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The type the policy was registered as</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to remove the policy from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearDefault<TPolicy>(this IPolicySet set)
            where TPolicy : class => set.Clear(typeof(TPolicy));

        #endregion


        #region Get

        /// <summary>
        /// Gets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to add the policy to.</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this IPolicySet set)
            where TPolicy : class => (TPolicy?)set.Get(typeof(TPolicy));

        /// <summary>
        /// Gets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</typeparam>
        /// <param name="policies"><see cref="IPolicySet"/> to add the policy to.</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? GetDefault<TPolicy>(this IPolicySet set)
            where TPolicy : class => (TPolicy?)set.Get(typeof(TPolicy));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this IPolicySet defaults, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(typeof(TPolicy)));

        #endregion


        #region Set

        /// <summary>
        /// Sets a default policy
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicySet"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this IPolicySet set, TPolicy policy)
            where TPolicy : class => set.Set(typeof(TPolicy), policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policies"><see cref="IPolicySet"/> to add the policy to.</param>
        /// <param name="type">The <see cref="Type"/> the policy implements</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault(this IPolicySet set, Type type, object policy)
            => set.Set(type, policy);

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicy">The <see cref="Type"/> of the policy</param>
        /// <param name="policies"><see cref="IPolicySet"/> to add the policy to.</param>
        /// <param name="policy">The policy to be set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDefault<TPolicy>(this IPolicySet set, TPolicy policy)
            where TPolicy : class => set.Set(typeof(TPolicy), policy);

        #endregion
    }
}
