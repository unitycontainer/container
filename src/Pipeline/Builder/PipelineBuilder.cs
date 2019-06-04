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
            BuildType = null;
            TypeConverter = null;
            BuildRequired = false;
            LifetimeManager = null;
            InjectionMembers = null;

            Policies = null;
            ContainerContext = container.Context;

            Seed = null;
            _enumerator = pipelines.GetEnumerator();
        }

        // Pipeline From Registration
        public PipelineBuilder(ExplicitRegistration registration)
        {
            Debug.Assert(null != registration.Type);

            Type = registration.Type;
            BuildType = null;
            TypeConverter = null;
            BuildRequired = registration.BuildRequired;
            LifetimeManager = registration.LifetimeManager;
            InjectionMembers = registration.InjectionMembers;

            Policies = registration;
            ContainerContext = registration.Owner.Context;

            Seed = registration.Pipeline;

            Debug.Assert(null != registration?.Processors);
            _enumerator = registration.Processors.GetEnumerator();
        }

        public PipelineBuilder(ref BuilderContext context)
        {
            Type = context.Type;
            BuildType = null;
            TypeConverter = context.Registration?.BuildType;
            BuildRequired = context.Registration?.BuildRequired ?? false;
            LifetimeManager = context.Registration?.LifetimeManager;
            InjectionMembers = context.Registration?.InjectionMembers;

            Policies = context.Registration;
            ContainerContext = context.ContainerContext;

            Seed = context.Registration?.Pipeline;

            Debug.Assert(null != context.Registration?.Processors);
            _enumerator = context.Registration.Processors.GetEnumerator();
        }

        // Pipeline from factory
        public PipelineBuilder(Type type, ExplicitRegistration factory, UnityContainer owner)
        {
            Type = type;
            BuildType = factory.Type;
            Seed = factory.Pipeline;
            TypeConverter = factory.BuildType;
            // TODO: Move to registration
            BuildRequired = null != factory.InjectionMembers && factory.InjectionMembers.Any(m => m.BuildRequired);
            LifetimeManager = factory.LifetimeManager?.CreateLifetimePolicy();
            InjectionMembers = factory.InjectionMembers;

            Policies = null;

            ContainerContext = owner.Context;

            Debug.Assert(null != factory.Processors);
            _enumerator = factory.Processors.GetEnumerator();
        }

        #endregion


        #region Public Members

        public Type Type;

        public readonly Type? BuildType;

        public LifetimeManager? LifetimeManager { get; }

        public InjectionMember[]? InjectionMembers { get; set; }

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
