using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unity.Builder;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Policy;
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

        public ImplicitRegistration(UnityContainer owner, string? name)
            : base(owner)
        {
            Name = name;
        }

        public ImplicitRegistration(UnityContainer owner, string? name, IPolicySet? set)
            : base(owner)
        {
            Name = name;
            Next = (PolicyEntry?)set;
        }

        public ImplicitRegistration(UnityContainer owner, string? name, ImplicitRegistration factory)
            : base(owner)
        {
            Name = name;
            Map = factory.Map;
            Next = factory.Next;
            LifetimeManager = factory.LifetimeManager?.CreateLifetimePolicy();
            InjectionMembers = factory.InjectionMembers;
        }

        public ImplicitRegistration(UnityContainer owner, string? name, ResolveDelegate<BuilderContext> pipeline)
            : base(owner)
        {
            Name = name;
            Pipeline = pipeline;
        }

        #endregion


        #region Public Members

        public string? Name { get; }

        public ResolveDelegate<BuilderContext>? Pipeline { get; set; }

        public IEnumerable<PipelineBuilder>? Processors { get; set; }

        public BuilderStrategy[]? BuildChain { get; set; } // TODO: Remove

        public InjectionMember[]? InjectionMembers { get; set; }

        public bool BuildRequired { get; set; }

        public Converter<Type, Type>? Map { get; set; }

        public LifetimeManager? LifetimeManager { get; set; }

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
            return policyInterface switch
            {
                Type type when typeof(LifetimeManager) == type => LifetimeManager, 
                _ => base.Get(policyInterface)
            };
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
