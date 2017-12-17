// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// Extension methods on <see cref="IPolicyList"/> to provide convenience
    /// overloads (generic versions, mostly).
    /// </summary>
    public static class PolicyListExtensions
    {
        #region Clear

        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <param name="policies"></param>
        /// <param name="policyInterface">The type of policy to remove.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        [Obsolete("Use IPolicyList.Clear(...)")]
        public static void Clear(this IPolicyList policies, Type policyInterface, object buildKey)
        {
            var key = ParseBuildKey(buildKey);
            (policies ?? throw new ArgumentNullException(nameof(policies))).Clear(key.Item1, key.Item2, policyInterface);
        }

        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        [Obsolete("Use IPolicyList.Clear(...)")]
        public static void Clear<TPolicyInterface>(this IPolicyList policies, object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            var key = ParseBuildKey(buildKey);
            (policies ?? throw new ArgumentNullException(nameof(policies))).Clear(key.Item1, key.Item2, typeof(TPolicyInterface));
        }

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The type the policy was registered as.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to remove the policy from.</param>
        [Obsolete("Use IPolicyList.Clear(null, null, ...)")]
        public static void ClearDefault<TPolicyInterface>(this IPolicyList policies)
            where TPolicyInterface : IBuilderPolicy
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Clear(null, null, typeof(TPolicyInterface));
        }


        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <param name="policies"></param>
        /// <param name="policyInterface">The type the policy was registered as.</param>
        [Obsolete("Use IPolicyList.Clear(null, null, ...)")]
        public static void ClearDefault(this IPolicyList policies, Type policyInterface)
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Clear(null, null, policyInterface);
        }

        #endregion


        #region Set

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policies"></param>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        [Obsolete("Use IPolicyList.Set(...)")]
        public static void Set(this IPolicyList policies, Type policyInterface, IBuilderPolicy policy, object buildKey = null)
        {
            var key = ParseBuildKey(buildKey);
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(key.Item1, key.Item2, policyInterface, policy);
        }


        /// <summary>
        /// Sets a generic individual policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public static void Set<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy, object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            var key = ParseBuildKey(buildKey);
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(key.Item1, key.Item2, typeof(TPolicyInterface), policy);
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
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(null, null, policyInterface, policy);
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
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(null, null, typeof(TPolicyInterface), policy);
        }

        #endregion


        #region Get

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        [Obsolete("Use IPolicyList.Get(...)")]
        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface, object buildKey)
        {
            return (policies ?? throw new ArgumentNullException(nameof(policies))).GetOrDefault(policyInterface, buildKey, out IPolicyList _);
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
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).GetOrDefault(typeof(TPolicyInterface), buildKey, out IPolicyList _);
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
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).GetOrDefault(typeof(TPolicyInterface), buildKey, out containingPolicyList);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="containingPolicyList">The policy list that actually contains the returned policy.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy Get(this IPolicyList policies, Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            var key = ParseBuildKey(buildKey);
            return (policies ?? throw new ArgumentNullException(nameof(policies))).Get(key.Item1, key.Item2, policyInterface, out containingPolicyList);
        }

        /// <summary>
        /// GetOrDefault the non default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).GetNoDefault(typeof(TPolicyInterface), buildKey);
        }

        /// <summary>
        /// GetOrDefault the non default policy.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface the policy is registered under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list that actually contains the returned policy.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)(policies ?? throw new ArgumentNullException(nameof(policies))).Get(typeof(TPolicyInterface), buildKey, out containingPolicyList);
        }

        /// <summary>
        /// GetOrDefault the non default policy.
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy GetNoDefault(this IPolicyList policies, Type policyInterface, object buildKey)
        {
            return (policies ?? throw new ArgumentNullException(nameof(policies))).Get(policyInterface, buildKey, out IPolicyList _);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static IBuilderPolicy GetOrDefault(this IPolicyList list, Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            Type buildType;

            if (buildKey is NamedTypeBuildKey basedBuildKey)
                buildType = basedBuildKey.Type;
            else
                buildType = buildKey as Type;

            return list.GetPolicyForKey(policyInterface, buildKey, out containingPolicyList) ??
                   list.GetPolicyForOpenGenericKey(policyInterface, buildKey, buildType, out containingPolicyList) ??
                   list.GetPolicyForType(policyInterface, buildType, out containingPolicyList) ??
                   list.GetPolicyForOpenGenericType(policyInterface, buildType, out containingPolicyList) ??
                   list.GetDefaultForPolicy(policyInterface, out containingPolicyList);
        }


        #endregion


        #region Implementation

        private static IBuilderPolicy GetPolicyForKey(this IPolicyList list, Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            if (buildKey != null)
            {
                return list.Get(policyInterface, buildKey, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private static IBuilderPolicy GetPolicyForOpenGenericKey(this IPolicyList list, Type policyInterface, object buildKey, Type buildType, out IPolicyList containingPolicyList)
        {
            if (buildType != null && buildType.GetTypeInfo().IsGenericType)
            {
                return list.Get(policyInterface, ReplaceType(buildKey, buildType.GetGenericTypeDefinition()), out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private static IBuilderPolicy GetPolicyForType(this IPolicyList list, Type policyInterface, Type buildType, out IPolicyList containingPolicyList)
        {
            if (buildType != null)
            {
                return list.Get(policyInterface, buildType, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private static IBuilderPolicy GetPolicyForOpenGenericType(this IPolicyList list, Type policyInterface, Type buildType, out IPolicyList containingPolicyList)
        {
            if (buildType != null && buildType.GetTypeInfo().IsGenericType)
            {
                return list.Get(policyInterface, buildType.GetGenericTypeDefinition(), out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private static IBuilderPolicy GetDefaultForPolicy(this IPolicyList list, Type policyInterface, out IPolicyList containingPolicyList)
        {
            return list.Get(policyInterface, null, out containingPolicyList);
        }

        private static object ReplaceType(object buildKey, Type newType)
        {
            if (buildKey is Type)
                return newType;

            if (buildKey is NamedTypeBuildKey originalKey)
                return new NamedTypeBuildKey(newType, originalKey.Name);

            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                Constants.CannotExtractTypeFromBuildKey,
                buildKey), nameof(buildKey));
        }


        private static Tuple<Type, string> ParseBuildKey(object buildKey)
        {
            switch (buildKey)
            {
                case NamedTypeBuildKey namedBuildKey:
                    return new Tuple<Type, string>(namedBuildKey.Type, namedBuildKey.Name);

                case Type typeBuildKey:
                    return new Tuple<Type, string>(typeBuildKey, string.Empty);

                case string name:
                    return new Tuple<Type, string>(null, name);

                case null:
                    return new Tuple<Type, string>(null, null);

                default:
                    return new Tuple<Type, string>(buildKey.GetType(), null);
            }
        }

        #endregion



        public static TPolicyInterface GetPolicy<TPolicyInterface>(this IPolicyList list, NamedTypeBuildKey buildKey, out IPolicyList containingPolicyList)
        {
            return (TPolicyInterface)(list.GetPolicyForKey(typeof(TPolicyInterface), buildKey, out containingPolicyList) ??
                                      list.GetPolicyForOpenType(typeof(TPolicyInterface), buildKey, buildKey.Type, out containingPolicyList) ??
                                      list.GetDefaultForPolicy(typeof(TPolicyInterface), out containingPolicyList));
        }

        private static IBuilderPolicy GetPolicyForOpenType(this IPolicyList list, Type policyInterface, NamedTypeBuildKey buildKey, Type buildType, out IPolicyList containingPolicyList)
        {
            containingPolicyList = null;
            if (null == buildType) return null;

            if (buildType.GetTypeInfo().IsGenericType)
            {
                var newType = buildType.GetGenericTypeDefinition();
                return list.Get(newType, buildKey.Name, policyInterface, out containingPolicyList) ??
                       list.Get(newType, string.Empty, policyInterface, out containingPolicyList);
            }

            if (buildType.IsArray && buildType.GetArrayRank() == 1)
            {
                return list.Get(typeof(Array), buildKey.Name, policyInterface, out containingPolicyList) ??
                       list.Get(typeof(Array), string.Empty, policyInterface, out containingPolicyList);
            }

            return null;
        }

    }
}
