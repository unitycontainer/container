// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.Policy
{
    /// <summary>
    /// Extension methods on <see cref="IPolicyList"/> to provide convenience
    /// overloads (generic versions, mostly).
    /// </summary>
    public static class PolicyListExtensions
    {
        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public static void Clear<TPolicyInterface>(this IPolicyList policies, object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Clear(typeof(TPolicyInterface), buildKey);
        }

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from.</param>
        public static void ClearDefault<TPolicyInterface>(this IPolicyList policies)
            where TPolicyInterface : IBuilderPolicy
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).ClearDefault(typeof(TPolicyInterface));
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)policies.Get(typeof(TPolicyInterface), buildKey, false);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="containingPolicyList">The policy list that actually contains the returned policy.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, out IPolicyList containingPolicyList)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).Get(typeof(TPolicyInterface), buildKey, false, out containingPolicyList);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface,
            object buildKey)
        {
            return (policies ?? throw new ArgumentNullException(nameof(policies))).Get(policyInterface, buildKey, false);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="containingPolicyList">The policy list that actually contains the returned policy.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface,
            object buildKey, out IPolicyList containingPolicyList)
        {
            return (policies ?? throw new ArgumentNullException(nameof(policies))).Get(policyInterface, buildKey, false, out containingPolicyList);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey,
            bool localOnly)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).Get(typeof(TPolicyInterface), buildKey, localOnly);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list that actually contains the returned policy.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey,
            bool localOnly, out IPolicyList containingPolicyList)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).Get(typeof(TPolicyInterface), buildKey, localOnly, out containingPolicyList);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface,
                                  object buildKey,
                                  bool localOnly)
        {

            return (policies ?? throw new ArgumentNullException(nameof(policies))).Get(policyInterface, buildKey, localOnly, out IPolicyList _);
        }

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey,
            bool localOnly)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).GetNoDefault(typeof(TPolicyInterface), buildKey, localOnly);
        }

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list that actually contains the returned policy.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey,
            bool localOnly, out IPolicyList containingPolicyList)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).GetNoDefault(typeof(TPolicyInterface), buildKey, localOnly, out containingPolicyList);
        }

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy GetNoDefault(this IPolicyList policies, Type policyInterface,
                                           object buildKey,
                                           bool localOnly)
        {
            return (policies ?? throw new ArgumentNullException(nameof(policies))).GetNoDefault(policyInterface, buildKey, localOnly, out IPolicyList _);
        }

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public static void Set<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy,
            object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(typeof(TPolicyInterface), policy, buildKey);
        }


        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policies"></param>
        /// <param name="policyInterface">The interface to register the policy under.</param>
        /// <param name="policy">The default policy to be registered.</param>
        public static void SetDefault(this IPolicyList policies, Type policyInterface, IBuilderPolicy policy)
        {
            policies.Set(policyInterface, policy);
        }

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface to register the policy under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The default policy to be registered.</param>
        public static void SetDefault<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy)
            where TPolicyInterface : IBuilderPolicy
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(typeof(TPolicyInterface), policy);
        }
    }
}