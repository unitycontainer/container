using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Registration
{
    [DebuggerDisplay("InternalRegistration:  Type={Type?.Name},    Name={Name}")]
    public class InternalRegistration : LinkedNode<Type, IBuilderPolicy>, 
                                        IPolicySet, 
                                        INamedType
    {
        #region Fields

        private readonly int _hash;

        #endregion


        #region Constructors

        public InternalRegistration(Type type, string name)
        {
            Name = name;
            Type = type;

            _hash = (Type?.GetHashCode() ?? 0 + 37) ^ (Name?.GetHashCode() ?? 0 + 17);
        }

        public InternalRegistration(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            Name = name;
            Type = type;
            Key = policyInterface;
            Value = policy;

            _hash = (Type?.GetHashCode() ?? 0 + 37) ^ (Name?.GetHashCode() ?? 0 + 17);
        }

        #endregion


        #region Public Members

        public virtual IList<BuilderStrategy> BuildChain { get; set; }

        public bool EnableOptimization { get; set; } = true;

        #endregion


        #region IPolicySet

        public virtual IBuilderPolicy Get(Type policyInterface)
        {
            for (var node = (LinkedNode<Type, IBuilderPolicy>)this; node != null; node = node.Next)
            {
                if (ReferenceEquals(node.Key, policyInterface))
                    return node.Value;
            }

            return null;
        }

        public virtual void Set(Type policyInterface, IBuilderPolicy policy)
        {
            if (null == Value && null == Key)
            {
                Key = policyInterface;
                Value = policy;
            }
            else
            {
                Next = new LinkedNode<Type, IBuilderPolicy>
                {
                    Key = policyInterface,
                    Value = policy,
                    Next = Next
                };
            }
        }

        public virtual void Clear(Type policyInterface)
        {
            LinkedNode<Type, IBuilderPolicy> node;
            LinkedNode<Type, IBuilderPolicy> last = null;

            for (node = this; node != null; node = node.Next)
            {
                if (ReferenceEquals(node.Key, policyInterface))
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

        public virtual void ClearAll()
        {
            Key = null;
            Value = null;
            Next  = null;
        }

        #endregion


        #region INamedType

        public Type Type { get; }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is INamedType registration &&
                   ReferenceEquals(Type, registration.Type) &&
                   Name == registration.Name;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public static implicit operator NamedTypeBuildKey(InternalRegistration namedType)
        {
            return new NamedTypeBuildKey(namedType.Type, namedType.Name);
        }

        #endregion
    }
}
