using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Unity.Builder;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

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
            BuildType = factory.BuildType;
            Next = factory.Next;
            LifetimeManager = factory.LifetimeManager?.CreateLifetimePolicy();
            InjectionMembers = factory.InjectionMembers;
            BuildRequired = null != InjectionMembers && InjectionMembers.Any(m => m.BuildRequired);
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

        public virtual PipelineDelegate? PipelineDelegate { get; set; }

        public ResolveDelegate<BuilderContext>? Pipeline { get; set; }

        public IEnumerable<Pipeline>? Processors { get; set; }

        public InjectionMember[]? InjectionMembers { get; set; }

        public virtual bool BuildRequired { get; }

        public virtual Converter<Type, Type>? BuildType { get; }

        public LifetimeManager? LifetimeManager { get; protected set; }

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
                Type type when typeof(LifetimeManager) == type => base.Get(policyInterface) ?? LifetimeManager, 
                _ => base.Get(policyInterface)
            };
        }

        public override void Set(Type policyInterface, object policy)
        {
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

            public Converter<Type, Type?>? Map => _registration.BuildType;

            public LifetimeManager? LifetimeManager => null;

            public int RefCount => _registration._refCount;
        }

        #endregion
    }

#if NETSTANDARD1_0 || NETCOREAPP1_0
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
#endif
}
