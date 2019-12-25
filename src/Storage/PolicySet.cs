using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Policy;

namespace Unity.Storage
{
    [DebuggerDisplay(nameof(PolicySet) + "({Count})")]
    [DebuggerTypeProxy(typeof(PolicySetDebugProxy))]
    public class PolicySet : PolicyEntry,
                             IEnumerable<PolicyEntry>,
                             IPolicySet
    {
        #region Constructors

        public PolicySet(Type type, object? value)
        {
            var node = (PolicyEntry)this;
            node.Key = type;
            node.Value = value;
        }

        #endregion


        #region Public Properties

        public UnityContainer Owner => (UnityContainer)(Value ?? 
            throw new InvalidOperationException("Invalid Policy Set"));
        
        #endregion


        #region IPolicySet

        public virtual object? Get(Type policyInterface)
        {
            for (PolicyEntry? node = this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                    return node.Value;
            }

            return null;
        }

        public virtual void Set(Type policyInterface, object policy)
        {
            for (PolicyEntry? node = this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                {
                    node.Value = policy;
                    return;
                }
            }

            Next = new PolicyEntry
            {
                Key = policyInterface,
                Value = policy,
                Next = Next
            };
        }

        public virtual void Clear(Type policyInterface)
        {
            PolicyEntry? node;
            PolicyEntry? last = null;

            for (node = this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                {
                    if (null == last)
                    {
                        Key = node.Next?.Key;
                        Value = node.Next?.Value;
                        Next = node.Next?.Next;
                    }
                    else
                    {
                        last.Key = node.Next?.Key;
                        last.Value = node.Next?.Value;
                        last.Next = node.Next?.Next;
                    }
                }

                last = node;
            }
        }

        #endregion


        #region IEnumerable

        public IEnumerator<PolicyEntry> GetEnumerator()
        {
            for (PolicyEntry? node = this; node != null; node = node.Next)
                yield return node;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion


        #region Debug Support

        protected int Count
        {
            get
            {
                var count = 0;
                for (PolicyEntry? node = this; node != null; node = node.Next)
                    count += 1;

                return count;
            }
        }

        protected class PolicySetDebugProxy
        {
            private readonly PolicySet _set;

            public PolicySetDebugProxy(PolicySet set)
            {
                _set = set;
                Policies = set.Select(p => new Policy(p))
                              .ToArray();
            }

            public Policy[] Policies { get; }
        }

        [DebuggerDisplay("{Value}", Name = "{Interface.Name}", Type = nameof(PolicyEntry))]
        public class Policy
        {
            PolicyEntry _node;
            public Policy(PolicyEntry node) => _node = node;
            public Type? Interface => _node.Key;
            public object? Value => _node.Value;
        }

        #endregion
    }

}
