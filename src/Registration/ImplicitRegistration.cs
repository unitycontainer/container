using System;
using System.Diagnostics;
using System.Threading;
using Unity.Builder;
using Unity.Composition;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Processors;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Registration
{
    [DebuggerDisplay("Registration.Implicit({Count})")]
    [DebuggerTypeProxy(typeof(ImplicitRegistrationDebugProxy))]
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

        public ImplicitRegistration(IPolicySet? set)
            : base(typeof(LifetimeManager), null, (PolicyEntry?)set)
        {
        }

        public ImplicitRegistration(ImplicitRegistration factory)
            : base(typeof(LifetimeManager), 
                   factory.LifetimeManager?.CreateLifetimePolicy(), 
                   factory.Next)
        {
            Map = factory.Map;
            InjectionMembers = factory.InjectionMembers;
        }

        public ImplicitRegistration(ResolveDelegate<BuilderContext> pipeline)
            : base(typeof(LifetimeManager))
        {
            Pipeline = pipeline;
        }

        #endregion


        #region Public Members

        public virtual ResolveDelegate<BuilderContext> Pipeline { get; set; }

        public virtual PipelineProcessor[]? Processors { get; set; }

        public virtual BuilderStrategy[]? BuildChain { get; set; }

        public InjectionMember[]? InjectionMembers { get; set; }

        public bool BuildRequired { get; set; }

        public Converter<Type, Type?>? Map { get; set; }

        public LifetimeManager? LifetimeManager
        {
            get => Value as LifetimeManager; 
            set => Value = value;
        }

        public virtual void Add(IPolicySet set)
        {
            var node = (PolicyEntry)this;
            while (null != node.Next) node = node.Next;
            node.Next = (PolicyEntry)set;
        }

        public virtual int AddRef() => Interlocked.Increment(ref _refCount);

        public virtual int Release() => Interlocked.Decrement(ref _refCount);

        #endregion


        #region IPolicySet

        public override object? Get(Type policyInterface)
        {
            if (typeof(ResolveDelegate<BuilderContext>) == policyInterface)
                return Pipeline;
            else
                return base.Get(policyInterface);
        }

        public override void Set(Type policyInterface, object policy)
        {
            if (typeof(ResolveDelegate<BuilderContext>) == policyInterface)
                Pipeline = (ResolveDelegate<BuilderContext>)policy;
            else
                Next = new PolicyEntry
                {
                    Key = policyInterface,
                    Value = policy,
                    Next = Next
                };
        }

        #endregion


        #region Debug Support

        protected class ImplicitRegistrationDebugProxy : PolicySetDebugProxy
        {
            private readonly ImplicitRegistration _registration;

            public ImplicitRegistrationDebugProxy(ImplicitRegistration set)
                : base(set)
            {
                _registration = set;
            }

            public InjectionMember[]? InjectionMembers => _registration.InjectionMembers;

            public bool BuildRequired => _registration.BuildRequired;

            public Converter<Type, Type?>? Map => _registration.Map;

            public LifetimeManager? LifetimeManager => null;

            public int RefCount => _registration._refCount;
        }

        #endregion
    }
}
