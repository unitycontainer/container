using System;
using Unity.Builder;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Registration
{
    public class InternalRegistration : LinkedNode<Type, IBuilderPolicy>, 
                                        IPolicyStore, INamedType
    {
        #region Fields

        private readonly int _hash;

        #endregion


        #region Constructors

        public InternalRegistration(Type type, string name)
        {
            Name = name;
            Type = type;
            _hash = Type?.GetHashCode() ?? 0 + Name?.GetHashCode() ?? 0;
        }

        #endregion


        #region  INamedType

        public Type Type { get; }

        public string Name { get; }

        #endregion


        #region IPolicyMap

        public virtual IBuilderPolicy Get(Type policyInterface)
        {
            for (var node = (LinkedNode<Type, IBuilderPolicy>)this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                    return node.Value;
            }

            return null;
        }

        public virtual void Set(Type policyInterface, IBuilderPolicy policy)
        {
            LinkedNode<Type, IBuilderPolicy> node;
            LinkedNode<Type, IBuilderPolicy> last = null;

            for (node = this; node != null; node = node.Next)
            {
                if (node.Key == policyInterface)
                {
                    // Found it
                    node.Value = policy;
                    return;
                }

                last = node;
            }

            // Not found, so add a new one
            last.Next = new LinkedNode<Type, IBuilderPolicy>
            {
                Key = policyInterface,
                Value = policy
            };
        }

        public virtual void Clear(Type policyInterface)
        {
            LinkedNode<Type, IBuilderPolicy> node;
            LinkedNode<Type, IBuilderPolicy> last = null;

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
                    return;
                }
                
                last = node;
            }
        }

        public virtual void ClearAll()
        {
            Key = null;
            Value = null;
            Next  = null;
        }

        #endregion


        #region Object

        public override bool Equals(object obj)
        {
            return obj is INamedType registration &&
                   Type == registration.Type &&
                   Name == registration.Name;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        #endregion

    }
}
