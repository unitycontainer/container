using System;
using System.Diagnostics;
using System.Threading;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Registration
{
    [DebuggerDisplay("InternalRegistration")]
    public class InternalRegistration : LinkedNode<Type, object>,
                                        IPolicySet
    {
        #region Fields

        private int _refCount;

        #endregion

        #region Constructors

        public InternalRegistration()
        {
            Key = typeof(LifetimeManager);
        }

        public InternalRegistration(InternalRegistration factory)
        {
            Key = typeof(LifetimeManager);

            if (null != factory)
            {
                InjectionMembers = factory.InjectionMembers;
                Map = factory.Map;

                var manager = factory.LifetimeManager;
                if (null != manager)
                {
                    LifetimeManager = manager.CreateLifetimePolicy();
                }
            }
        }

        public InternalRegistration(Type policyInterface, object policy)
        {
            Key = typeof(LifetimeManager);
            Next = new LinkedNode<Type, object>
            {
                Key = policyInterface,
                Value = policy,
                Next = Next
            };
        }

        #endregion


        #region Public Members

        public virtual BuilderStrategy[] BuildChain { get; set; }

        public InjectionMember[] InjectionMembers { get; set; }

        public bool BuildRequired { get; set; }

        public Converter<Type, Type> Map { get; set; }

        // TODO: Streamline LifetimeManager usage
        public LifetimeManager LifetimeManager
        {
            get => (LifetimeManager)Value;
            set => Value = value;
        }

        public virtual int AddRef() => Interlocked.Increment(ref _refCount);

        public virtual int Release() => Interlocked.Decrement(ref _refCount);

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
