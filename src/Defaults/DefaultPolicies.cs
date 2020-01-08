using System;
using Unity.Storage;

namespace Unity.Policy
{
    public partial class DefaultPolicies : IPolicySet
    {
        #region Properties

        protected PolicyEntry Policies { get; private set; }

        #endregion


        #region PolicySet

        public virtual object? Get(Type policyInterface)
        {
            for (PolicyEntry? node = Policies; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                    return node.Value;
            }

            return null;
        }

        public virtual void Set(Type policyInterface, object policy)
        {
            if (null == policy) throw new ArgumentNullException(nameof(policy));

            for (PolicyEntry? node = Policies; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                {
                    node.Value = policy;
                    return;
                }
            }

            Policies = new PolicyEntry
            {
                Key = policyInterface,
                Value = policy,
                Next = Policies
            };
        }

        public virtual void Clear(Type policyInterface) => throw new InvalidOperationException("Default policies can only be added or replaced");

        #endregion
    }
}
