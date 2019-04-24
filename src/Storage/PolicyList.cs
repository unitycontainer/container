using System;
using System.Collections.Generic;
using Unity.Policy;

namespace Unity.Storage
{
    /// <summary>
    /// A custom collection wrapper over <see cref="IBuilderPolicy"/> objects.
    /// </summary>
    public class PolicyList : IPolicyList
    {
        #region Fields

        private readonly object _sync = new object();
        private readonly IPolicyList _innerPolicyList;
        private IDictionary<PolicyKey, object> _policies = null;

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
        public int Count => _policies?.Count ?? 0;


        public void Clear(Type type, string name, Type policyInterface)
        {
            _policies?.Remove(new PolicyKey(type, name, policyInterface));
        }

        /// <summary>
        /// Removes a default policy.
        /// </summary>
        /// <param name="policyInterface">The type the policy was registered as.</param>
        public void ClearDefault(Type policyInterface)
        {
            Clear(null, null, policyInterface);
        }


        public object Get(Type type, string name, Type policyInterface)
        {
            object policy = null;

            if (_policies?.TryGetValue(new PolicyKey(type, name, policyInterface), out policy) ?? false)
            {
                return policy;
            }

            return _innerPolicyList?.Get(type, name, policyInterface);
        }

        public object Get(Type type, Type policyInterface)
        {
            object policy = null;

            if (_policies?.TryGetValue(new PolicyKey(type, UnityContainer.All, policyInterface), out policy) ?? false)
            {
                return policy;
            }

            return _innerPolicyList?.Get(type, UnityContainer.All, policyInterface);
        }

        public void Set(Type type, Type policyInterface, object policy)
        {
            if (null == _policies)
                _policies = new Dictionary<PolicyKey, object>(PolicyKeyEqualityComparer.Default);

            _policies[new PolicyKey(type, UnityContainer.All, policyInterface)] = policy;
        }

        public void Set(Type type, string name, Type policyInterface, object policy)
        {
            if (null == _policies)
                _policies = new Dictionary<PolicyKey, object>(PolicyKeyEqualityComparer.Default);

            _policies[new PolicyKey(type, name, policyInterface)] = policy;
        }

        #endregion


        #region Nested Types

        private struct PolicyKey 
        {
            #region Fields

            private readonly int _hash;
            private readonly Type _type;
            private readonly string _name;
            private readonly Type _policy;

            #endregion

            public PolicyKey(Type type, string name, Type policyType)
            {
                _policy = policyType;
                _type = type;
                _name = !string.IsNullOrEmpty(name) ? name : null;
                _hash = (policyType?.GetHashCode() ?? 0) * 37 + 
                    (ReferenceEquals(UnityContainer.All, name) ? type?.GetHashCode() ?? 0 
                                                               : ((type?.GetHashCode() ?? 0 + 37) ^ (name?.GetHashCode() ?? 0 + 17)));
            }

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
                return left._policy == right._policy && 
                       left._type == right._type &&
                       Equals(left._name, right._name);
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
                return x == y;
            }

            public int GetHashCode(PolicyKey obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
    }
}
