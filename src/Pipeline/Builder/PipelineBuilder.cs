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

        public PipelineBuilder(ExplicitRegistration registration, UnityContainer container, IEnumerable<Pipeline> pipelines)
        {
            Type = registration.Type ?? typeof(object);
            Name = registration.Name;
            BuildType = registration.BuildType;
            BuildRequired = registration.BuildRequired;
            LifetimeManager = registration.LifetimeManager;
            InjectionMembers = registration.InjectionMembers;

            Registration = registration;
            ContainerContext = container.Context;

            Seed = registration.Pipeline;
            _enumerator = pipelines.GetEnumerator();
            Debug.Assert(null != _enumerator);
        }

        public PipelineBuilder(Type type, string? name, UnityContainer container, IEnumerable<Pipeline> pipelines)
        {
            Type = type;
            Name = name;
            BuildType = null;
            BuildRequired = false;
            LifetimeManager = null;
            InjectionMembers = null;

            Registration = null;
            ContainerContext = container.Context;

            Seed = null;
            _enumerator = pipelines.GetEnumerator();
        }

        public PipelineBuilder(Type type, string? name, UnityContainer container, IRegistration registration)
        {
            Type = type;
            Name = name;
            BuildType = registration.BuildType;
            BuildRequired = registration.BuildRequired;
            LifetimeManager = registration.LifetimeManager;
            InjectionMembers = registration.InjectionMembers;

            Registration = registration;
            ContainerContext = container.Context;

            Seed = registration.Pipeline;
            _enumerator = (registration?.Processors ??
                           Enumerable.Empty<Pipeline>()).GetEnumerator();
        }

        public PipelineBuilder(ref BuilderContext context)
        {
            Type = context.Type;
            Name = context.Name;
            BuildType = context.Registration?.BuildType;
            BuildRequired = context.Registration?.BuildRequired ?? false;
            LifetimeManager = context.Registration?.LifetimeManager;
            InjectionMembers = context.Registration?.InjectionMembers;

            Registration = context.Registration;
            ContainerContext = context.ContainerContext;

            Seed = context.Registration?.Pipeline;
            _enumerator = (context.Registration?.Processors ?? 
                           Enumerable.Empty<Pipeline>()).GetEnumerator();
        }

        #endregion


        #region Public Members

        public Type Type;

        public string? Name { get; }

        public LifetimeManager? LifetimeManager { get; }

        public InjectionMember[]? InjectionMembers { get; set; }

        public bool BuildRequired { get; }

        public Converter<Type, Type>? BuildType { get; }



        public ResolveDelegate<BuilderContext>? Seed { get; private set; }

        public readonly ContainerContext ContainerContext;

        public IRegistration? Registration { get; }

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
