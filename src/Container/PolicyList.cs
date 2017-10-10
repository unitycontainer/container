// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Container
{
    /// <summary>
    /// A custom collection wrapper over <see cref="IBuilderPolicy"/> objects.
    /// </summary>
    public class PolicyList : IPolicyList
    {
        private readonly IPolicyList _innerPolicyList;
        private readonly IDictionary<PolicyKey, IBuilderPolicy> _policies = 
            new ConcurrentDictionary<PolicyKey, IBuilderPolicy>(PolicyKeyEqualityComparer.Default);

        /// <summary>
        /// Initialize a new instance of a <see cref="PolicyList"/> class.
        /// </summary>
        public PolicyList()
            : this(null) { }

        /// <summary>
        /// Initialize a new instance of a <see cref="PolicyList"/> class with another policy list.
        /// </summary>
        /// <param name="innerPolicyList">An inner policy list to search.</param>
        public PolicyList(IPolicyList innerPolicyList)
        {
            _innerPolicyList = innerPolicyList;
        }

        /// <summary>
        /// Gets the number of items in the locator.
        /// </summary>
        /// <value>
        /// The number of items in the locator.
        /// </value>
        public int Count => _policies.Count;

        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <param name="policyInterface">The type of policy to remove.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Clear(Type policyInterface, object buildKey)
        {
            _policies.Remove(new PolicyKey(policyInterface, buildKey));
        }

        /// <summary>
        /// Removes all policies from the list.
        /// </summary>
        public void ClearAll()
        {
            _policies.Clear();
        }

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <param name="policyInterface">The type the policy was registered as.</param>
        public void ClearDefault(Type policyInterface)
        {
            Clear(policyInterface, null);
        }

        /// <summary>
        /// Gets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <param name="localOnly">true if the policy searches local only; otherwise false to search up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public IBuilderPolicy Get(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            TryGetType(buildKey, out var buildType);

            return GetPolicyForKey(policyInterface, buildKey, localOnly, out containingPolicyList) ??
                GetPolicyForOpenGenericKey(policyInterface, buildKey, buildType, localOnly, out containingPolicyList) ??
                GetPolicyForType(policyInterface, buildType, localOnly, out containingPolicyList) ??
                GetPolicyForOpenGenericType(policyInterface, buildType, localOnly, out containingPolicyList) ??
                GetDefaultForPolicy(policyInterface, localOnly, out containingPolicyList);
        }

        private IBuilderPolicy GetPolicyForKey(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            if (buildKey != null)
            {
                return GetNoDefault(policyInterface, buildKey, localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForOpenGenericKey(Type policyInterface, object buildKey, Type buildType, bool localOnly, out IPolicyList containingPolicyList)
        {
            if (buildType != null && buildType.GetTypeInfo().IsGenericType)
            {
                return GetNoDefault(policyInterface, ReplaceType(buildKey, buildType.GetGenericTypeDefinition()),
                    localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForType(Type policyInterface, Type buildType, bool localOnly, out IPolicyList containingPolicyList)
        {
            if (buildType != null)
            {
                return GetNoDefault(policyInterface, buildType, localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForOpenGenericType(Type policyInterface, Type buildType, bool localOnly, out IPolicyList containingPolicyList)
        {
            if (buildType != null && buildType.GetTypeInfo().IsGenericType)
            {
                return GetNoDefault(policyInterface, buildType.GetGenericTypeDefinition(), localOnly, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetDefaultForPolicy(Type policyInterface, bool localOnly, out IPolicyList containingPolicyList)
        {
            return GetNoDefault(policyInterface, null, localOnly, out containingPolicyList);
        }

        /// <summary>
        /// Get the non default policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies to.</param>
        /// <param name="localOnly">True if the search should be in the local policy list only; otherwise false to search up the parent chain.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list if present; returns null otherwise.</returns>
        public IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            containingPolicyList = null;

            IBuilderPolicy policy;
            if (_policies.TryGetValue(new PolicyKey(policyInterface, buildKey), out policy))
            {
                containingPolicyList = this;
                return policy;
            }

            if (localOnly)
            {
                return null;
            }

            return _innerPolicyList?.GetNoDefault(policyInterface, buildKey, false, out containingPolicyList);
        }

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey)
        {
            _policies[new PolicyKey(policyInterface, buildKey)] = policy;
        }

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policyInterface">The interface to register the policy under.</param>
        /// <param name="policy">The default policy to be registered.</param>
        public void SetDefault(Type policyInterface,
                               IBuilderPolicy policy)
        {
            Set(policyInterface, policy, null);
        }

        private static bool TryGetType(object buildKey, out Type type)
        {
            type = buildKey as Type;

            if (type == null)
            {
                var basedBuildKey = buildKey as NamedTypeBuildKey;
                if (basedBuildKey != null)
                {
                    type = basedBuildKey.Type;
                }
            }

            return type != null;
        }

        private static object ReplaceType(object buildKey, Type newType)
        {
            if (buildKey is Type)
            {
                return newType;
            }

            var originalKey = buildKey as NamedTypeBuildKey;
            if (originalKey != null)
            {
                return new NamedTypeBuildKey(newType, originalKey.Name);
            }

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Constants.CannotExtractTypeFromBuildKey,
                    buildKey),
                    nameof(buildKey));
        }

        private struct PolicyKey
        {
#pragma warning disable 219
            public readonly object BuildKey;
            public readonly Type PolicyType;
#pragma warning restore 219

            public PolicyKey(Type policyType,
                             object buildKey)
            {
                PolicyType = policyType;
                BuildKey = buildKey;
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is PolicyKey)
                {
                    return this == (PolicyKey)obj;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return ((SafeGetHashCode(PolicyType)) * 37) +
                         SafeGetHashCode(BuildKey);
            }

            public static bool operator ==(PolicyKey left, PolicyKey right)
            {
                return left.PolicyType == right.PolicyType &&
                    Equals(left.BuildKey, right.BuildKey);
            }

            public static bool operator !=(PolicyKey left, PolicyKey right)
            {
                return !(left == right);
            }

            private static int SafeGetHashCode(object obj)
            {
                return obj != null ? obj.GetHashCode() : 0;
            }
        }

        private class PolicyKeyEqualityComparer : IEqualityComparer<PolicyKey>
        {
            public static readonly PolicyKeyEqualityComparer Default = new PolicyKeyEqualityComparer();

            public bool Equals(PolicyKey x, PolicyKey y)
            {
                return x.PolicyType == y.PolicyType &&
                    Equals(x.BuildKey, y.BuildKey);
            }

            public int GetHashCode(PolicyKey obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
