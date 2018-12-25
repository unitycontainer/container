using System;
using System.Diagnostics;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Registration
{
    [DebuggerDisplay("InternalRegistration:  Type={Type?.Name},    Name={Name}")]
    public class InternalRegistration : LinkedNode<Type, object>,
                                        IPolicySet
    {
        #region Fields

        #endregion


        #region Constructors

        public InternalRegistration()
        {
        }

        public InternalRegistration(Type policyInterface, object policy)
        {
            Key = policyInterface;
            Value = policy;
        }

        #endregion


        #region Public Members

        public virtual BuilderStrategy[] BuildChain { get; set; }

        public InjectionMember[] InjectionMembers { get; set; }

        public bool BuildRequired { get; set; }

        public Converter<Type, Type> Map { get; set; }

        #endregion


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
            if (null == Value && null == Key)
            {
                Key = policyInterface;
                Value = policy;
            }
            else
            {
                Next = new LinkedNode<Type, object>
                {
                    Key = policyInterface,
                    Value = policy,
                    Next = Next
                };
            }
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
