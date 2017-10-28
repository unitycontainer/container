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
        #region Fields

        private readonly IPolicyList _innerPolicyList;
        private readonly IDictionary<PolicyKey, IBuilderPolicy> _policies = 
            new ConcurrentDictionary<PolicyKey, IBuilderPolicy>(PolicyKeyEqualityComparer.Default);

        #endregion


        #region Constructors

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

        #endregion


        #region IPolicyList

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
        public IBuilderPolicy Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            Type buildType;

            if (buildKey is NamedTypeBuildKey basedBuildKey)
                buildType = basedBuildKey.Type;
            else
                buildType = buildKey as Type;

            return GetPolicyForKey(policyInterface, buildKey, out containingPolicyList) ??
                GetPolicyForOpenGenericKey(policyInterface, buildKey, buildType, out containingPolicyList) ??
                GetPolicyForType(policyInterface, buildType, out containingPolicyList) ??
                GetPolicyForOpenGenericType(policyInterface, buildType, out containingPolicyList) ??
                GetDefaultForPolicy(policyInterface, out containingPolicyList);
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
        public IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            containingPolicyList = null;

            if (_policies.TryGetValue(new PolicyKey(policyInterface, buildKey), out var policy))
            {
                containingPolicyList = this;
                return policy;
            }

            return _innerPolicyList?.GetNoDefault(policyInterface, buildKey, out containingPolicyList);
        }

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey = null)
        {
            _policies[new PolicyKey(policyInterface, buildKey)] = policy;
        }

        #endregion


        #region Implementation

        private IBuilderPolicy GetPolicyForKey(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            if (buildKey != null)
            {
                return GetNoDefault(policyInterface, buildKey, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForOpenGenericKey(Type policyInterface, object buildKey, Type buildType, out IPolicyList containingPolicyList)
        {
            if (buildType != null && buildType.GetTypeInfo().IsGenericType)
            {
                return GetNoDefault(policyInterface, ReplaceType(buildKey, buildType.GetGenericTypeDefinition()), out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForType(Type policyInterface, Type buildType, out IPolicyList containingPolicyList)
        {
            if (buildType != null)
            {
                return GetNoDefault(policyInterface, buildType, out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetPolicyForOpenGenericType(Type policyInterface, Type buildType, out IPolicyList containingPolicyList)
        {
            if (buildType != null && buildType.GetTypeInfo().IsGenericType)
            {
                return GetNoDefault(policyInterface, buildType.GetGenericTypeDefinition(), out containingPolicyList);
            }
            containingPolicyList = null;
            return null;
        }

        private IBuilderPolicy GetDefaultForPolicy(Type policyInterface, out IPolicyList containingPolicyList)
        {
            return GetNoDefault(policyInterface, null, out containingPolicyList);
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

        #endregion


        #region Nested Types

        private class PolicyKey
        {
            private readonly int _hash;

            public PolicyKey(Type policyType, object buildKey)
            {
                PolicyType = policyType;
                BuildKey = buildKey;
                _hash = ((PolicyType?.GetHashCode() ?? 0) * 37) + (BuildKey?.GetHashCode() ?? 0);
            }

            public object BuildKey { get; }

            public Type PolicyType { get; }


            public override bool Equals(object obj)
            {
                if (obj is PolicyKey key)
                {
                    return this == key;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return _hash;
            }

            public static bool operator ==(PolicyKey left, PolicyKey right)
            {
                return left?.PolicyType == right?.PolicyType && Equals(left?.BuildKey, right?.BuildKey);
            }

            public static bool operator !=(PolicyKey left, PolicyKey right)
            {
                return !(left == right);
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

        #endregion
    }
}
