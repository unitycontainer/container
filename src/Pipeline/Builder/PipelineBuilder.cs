using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Unity.Builder;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using static Unity.UnityContainer;

namespace Unity
{
    [SecuritySafeCritical]
    [DebuggerDisplay("Type: {Type?.Name} Name: {Registration?.Name}    Stage: {_enumerator.Current?.GetType().Name}")]
    public ref partial struct PipelineBuilder
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string error = "\n\nFor more detailed information run Unity in debug mode: new UnityContainer(ModeFlags.Diagnostic)";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerator<Pipeline> _enumerator;

        #endregion


        #region Constructors

        // Pipeline From Type
        public PipelineBuilder(Type type, UnityContainer container, IEnumerable<Pipeline> pipelines)
        {
            Type = type;
            TypeConverter = null;
            LifetimeManager = null;
            InjectionMembers = null;

            Factory = null;
            Policies = null;
            Registration = null;

            IsMapping = false;
            BuildRequired = false;

            ContainerContext = container.Context;

            Seed = null;
            _enumerator = pipelines.GetEnumerator();
        }

        // Pipeline From Registration
        public PipelineBuilder(Type type, ExplicitRegistration registration)
        {
            Debug.Assert(null != registration.Type);

            Type = type;
            TypeConverter = null;
            LifetimeManager = registration.LifetimeManager;
            InjectionMembers = registration.InjectionMembers;

            Factory = null;
            Policies = registration;
            Registration = registration;

            IsMapping = null != registration.Type && type != registration.Type;
            BuildRequired = registration.BuildRequired;

            Seed = registration.Pipeline;

            ContainerContext = registration.Owner.Context;

            Debug.Assert(null != registration?.Processors);
            _enumerator = registration.Processors.GetEnumerator();
        }

        // Pipeline from context
        public PipelineBuilder(ref BuilderContext context)
        {
            var type = (context.Registration as ExplicitRegistration)?.Type ?? context.Type;
            Type = context.Type;
            TypeConverter = context.Registration?.BuildType;
            LifetimeManager = context.Registration?.LifetimeManager;
            InjectionMembers = context.Registration?.InjectionMembers;

            Registration = context.Registration as ExplicitRegistration;
            Factory = null != Registration ? null : context.Registration as ImplicitRegistration;
            Policies = Registration ?? Factory;

            IsMapping = (null != Registration?.Type && Registration?.Type != Type) || null != Factory?.BuildType;
            BuildRequired = context.Registration?.BuildRequired ?? false;

            ContainerContext = context.ContainerContext;

            Seed = context.Registration?.Pipeline;

            Debug.Assert(null != context.Registration?.Processors);
            _enumerator = context.Registration.Processors.GetEnumerator();
        }

        // Pipeline from factory
        public PipelineBuilder(Type type, ExplicitRegistration factory, UnityContainer owner)
        {
            Type = type;
            Seed = factory.Pipeline;
            TypeConverter = factory.BuildType;
            LifetimeManager = factory.LifetimeManager?.CreateLifetimePolicy();
            InjectionMembers = factory.InjectionMembers;

            Factory = factory;
            Policies = factory;
            Registration = null;

            IsMapping     = null != factory.BuildType;
            // TODO: Move to registration
            BuildRequired = null != factory.InjectionMembers && factory.InjectionMembers.Any(m => m.BuildRequired);

            ContainerContext = owner.Context;

            Debug.Assert(null != factory.Processors);
            _enumerator = factory.Processors.GetEnumerator();
        }

        #endregion


        #region Public Members

        public Type Type;

        public LifetimeManager? LifetimeManager { get; }

        public InjectionMember[]? InjectionMembers { get; set; }

        public ExplicitRegistration? Registration { get; }

        public ImplicitRegistration? Factory { get; }


        public bool IsMapping { get; }

        public bool BuildRequired { get; }

        public Converter<Type, Type>? TypeConverter { get; }

        public IPolicySet? Policies { get; }

        public readonly ContainerContext ContainerContext;

        public ResolveDelegate<BuilderContext>? Seed { get; private set; }

        #endregion


        #region Public Methods

        public ResolveDelegate<BuilderContext>? Pipeline()
        {
            ref var context = ref this;
            return _enumerator?.MoveNext() ?? false 
                 ? _enumerator?.Current?.Build(ref context) ?? Seed
                 : Seed;
        }

        public ResolveDelegate<BuilderContext>? Pipeline(ResolveDelegate<BuilderContext>? method = null)
        {
            Seed = method;

            ref var context = ref this;
            return _enumerator?.MoveNext() ?? false 
                 ? _enumerator?.Current.Build(ref context) ?? Seed
                 : Seed;
        }

        #endregion
    }
}
