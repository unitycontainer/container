using System;
using Unity.Policy;

namespace Unity
{
    public static class LegacyPolicyListUtilityExtensions
    {
        #region Set

        /// <summary>
        /// Sets a policy.
        /// </summary>
        /// <typeparam name="TInterface">The interface to register the policy under.</typeparam>
        /// <param name="policies"></param>
        /// <param name="policy">The policy to be registered.</param>
        public static void Set<TInterface>(this IPolicyList policies, object policy)
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(null, null, typeof(TInterface), policy);
        }

        /// <summary>
        /// Sets a policy.
        /// </summary>
        /// <typeparam name="TInterface">The interface to register the policy under.</typeparam>
        /// <param name="policies"></param>
        /// <param name="type">Type of registration</param>
        /// <param name="name">Name of registration</param>
        /// <param name="policy">The policy to be registered.</param>
        public static void Set<TInterface>(this IPolicyList policies, Type type, string name, object policy)
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(type, name, typeof(TInterface), policy);
        }


        #endregion


        #region Get


        /// <summary>
        /// Retrieve default policy
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <typeparam name="TInterface">The interface of the policy to register.</typeparam>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TInterface Get<TInterface>(this IPolicyList policies)
        {
            return (TInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).Get(null, null, typeof(TInterface));
        }

        /// <summary>
        /// Retrieve requested policy for named type
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <typeparam name="TInterface">The interface of the policy to register.</typeparam>
        /// <param name="type">Type of registration</param>
        /// <param name="name">Name of registration</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TInterface Get<TInterface>(this IPolicyList policies, Type type, string name)
        {
            return (TInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).Get(type, name, typeof(TInterface));
        }



        #endregion
    }
}
