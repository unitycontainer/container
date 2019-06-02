using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Unity.Builder;
using Unity.Lifetime;
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
            Registration = registration;
            ContainerContext = container.Context;

            Seed = Registration.Pipeline;
            _enumerator = pipelines.GetEnumerator();
            Debug.Assert(null != _enumerator);
        }

        public PipelineBuilder(Type type, string? name, UnityContainer container, IEnumerable<Pipeline> pipelines)
        {
            Seed = null;
            Type = type;
            Name = name;
            Registration = null;
            ContainerContext = container.Context;

            _enumerator = pipelines.GetEnumerator();
        }

        public PipelineBuilder(Type type, string? name, UnityContainer container, IRegistration registration)
        {
            Seed = registration?.Pipeline;
            Type = type;
            Name = name;
            Registration = registration;
            ContainerContext = container.Context;

            _enumerator = (registration?.Processors ??
                           Enumerable.Empty<Pipeline>()).GetEnumerator();
        }

        public PipelineBuilder(ref BuilderContext context)
        {
            Type = context.Type;
            Name = context.Name;
            Registration = context.Registration;
            ContainerContext = context.ContainerContext;

            Seed = Registration?.Pipeline;
            _enumerator = (context.Registration?.Processors ?? 
                           Enumerable.Empty<Pipeline>()).GetEnumerator();
        }

        #endregion


        #region Public Members

        public Type Type;

        public string? Name { get; }

        public ResolveDelegate<BuilderContext>? Seed { get; private set; }

        public readonly ContainerContext ContainerContext;

        public readonly IRegistration? Registration;

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

        public PipelineDelegate PipelineDelegate()
        {
            var manager = Registration?.LifetimeManager;

            return manager switch
            {
                null                        => TransientLifetime(),
                TransientLifetimeManager  _ => TransientLifetime(),
                PerResolveLifetimeManager _ => PerResolveLifetime(),
                PerThreadLifetimeManager  _ => PerThreadLifetime(),
                                          _ => DefaultLifetime()
            };
        }

        public BuildPipelineAsync BuildPipelineAsync()
        {
            var manager = Registration?.LifetimeManager;

            return manager switch
            {
                null                        => TransientLifetimeAsync(),
                TransientLifetimeManager  _ => TransientLifetimeAsync(),
                PerResolveLifetimeManager _ => PerResolveLifetimeAsync(),
                PerThreadLifetimeManager  _ => PerThreadLifetimeAsync(),
                                          _ => DefaultLifetimeAsync()
            };
        }


        #endregion
    }
}
