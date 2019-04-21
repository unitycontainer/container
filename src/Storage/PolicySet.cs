using System;
using Unity.Policy;

namespace Unity.Storage
{
    public class PolicySet : LinkedNode<Type, object>,
                             IPolicySet
    {
        #region IPolicySet

        public virtual object Get(Type policyInterface)
        {
            for (var node = (LinkedNode<Type, object>)this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                    return node.Value;
            }

            return null;
        }

        public virtual void Set(Type policyInterface, object policy)
        {
            for (var node = (LinkedNode<Type, object>)this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                {
                    node.Value = policy;
                    return;
                }
            }

            Next = new LinkedNode<Type, object>
            {
                Key = policyInterface,
                Value = policy,
                Next = Next
            };
        }

        public virtual void Clear(Type policyInterface)
        {
            LinkedNode<Type, object> node;
            LinkedNode<Type, object> last = null;

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
    }
}
