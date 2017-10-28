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
        /// GetOrDefault the non default policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies to.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list if present; returns null otherwise.</returns>
        public IBuilderPolicy Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            containingPolicyList = null;

            if (_policies.TryGetValue(new PolicyKey(policyInterface, buildKey), out var policy))
            {
                containingPolicyList = this;
                return policy;
            }

            return _innerPolicyList?.Get(policyInterface, buildKey, out containingPolicyList);
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
