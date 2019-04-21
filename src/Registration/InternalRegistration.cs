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
    public class InternalRegistration : PolicySet
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
                Map = factory.Map;
                Next = factory.Next;
                InjectionMembers = factory.InjectionMembers;
                LifetimeManager  = factory.LifetimeManager?
                                          .CreateLifetimePolicy();
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
