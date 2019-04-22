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
    [DebuggerDisplay("Registration (Implicit)")]
    public class ImplicitRegistration : PolicySet
    {
        #region Fields

        private int _refCount;

        #endregion

        #region Constructors

        public ImplicitRegistration()
            : base(typeof(LifetimeManager))
        {
        }

        public ImplicitRegistration(IPolicySet set)
            : base(typeof(LifetimeManager))
        {
            switch(set)
            {
                case ImplicitRegistration factory:
                    Map = factory.Map;
                    Next = factory.Next;
                    InjectionMembers = factory.InjectionMembers;
                    LifetimeManager = factory.LifetimeManager?
                                              .CreateLifetimePolicy();
                    break;

                case LinkedNode<Type, object> node:
                    Next = node;
                    break;
            }
        }

        public ImplicitRegistration(Type policyInterface, object policy)
            : base(typeof(LifetimeManager))
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

        public virtual void Add(IPolicySet set)
        {
            if (set is LinkedNode<Type, object> policies)
            {
                var node = (LinkedNode<Type, object>)this;
                while (null != node.Next) node = node.Next;
                node.Next = policies;
            }
        }

        public virtual int AddRef() => Interlocked.Increment(ref _refCount);

        public virtual int Release() => Interlocked.Decrement(ref _refCount);

        #endregion


        #region IPolicySet

        public override void Set(Type policyInterface, object policy)
        {
            Next = new LinkedNode<Type, object>
            {
                Key = policyInterface,
                Value = policy,
                Next = Next
            };
        }

        #endregion
    }
}
