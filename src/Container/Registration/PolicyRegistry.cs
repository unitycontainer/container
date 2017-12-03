using System;
using Unity.Container.Storage;
using Unity.Policy;

namespace Unity.Container.Registration
{
    public class PolicyRegistry : LinkedNode<Type, object>, 
                                  IMap<Type, IBuilderPolicy>, 
                                  IPolicyList
    {
        #region Constructors

        public PolicyRegistry()
        {
        }


        public PolicyRegistry(PolicyRegistry parent)
        {
            Next = parent;
        }


        #endregion


        #region IMap

        public virtual IBuilderPolicy this[Type policy]
        {
            get
            {
                for (var node = Next; node != null; node = node.Next)
                {
                    if (node.Key == policy)
                        return (IBuilderPolicy)node.Value;
                }

                return null;
            }
            set
            {
                for (var node = Next; node != null; node = node.Next)
                {
                    if (node.Key == policy)
                    {
                        // Found it
                        node.Value = value;
                        return;
                    }
                }

                Next = new LinkedNode<Type, object>
                {
                    Key = policy,
                    Next = Next,
                    Value = value
                };
            }
        }

        #endregion


        #region IPolicyList

        public virtual void Clear(Type policyInterface, object buildKey)
        {
            throw new NotImplementedException();
        }

        public virtual void ClearAll()
        {
            Next = null;
        }

        public virtual IBuilderPolicy Get(Type policy, object buildKey, out IPolicyList containingPolicyList)
        {
            throw new NotImplementedException();
        }

        public virtual void Set(Type policy, IBuilderPolicy value, object buildKey = null)
        {
            for (var node = Next; node != null; node = node.Next)
            {
                if (node.Key == policy)
                {
                    // TODO: Check if buildKey is Equal
                    node.Value = value;
                    return;
                }
            }

            Next = new LinkedNode<Type, object>
            {
                Key = policy,
                Next = Next,
                Value = value
            };
        }

        #endregion
    }
}
