using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<Pipeline> _pipelines;

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
            _pipelines  = pipelines;
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
            _pipelines  = registration.Processors;
        }

        // Pipeline from factory
        public PipelineBuilder(Type type, ExplicitRegistration factory, LifetimeManager manager, UnityContainer owner)
        {
            Type = type;
            Seed = factory.Pipeline;
            TypeConverter = factory.BuildType;
            LifetimeManager = manager;
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
            _pipelines  = factory.Processors;
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

        public ResolveDelegate<PipelineContext>? Seed { get; private set; }

        #endregion


        #region Public Methods


        public ResolveDelegate<PipelineContext> Compile()
        {
            ref var context = ref this;
            var expressions = _enumerator.MoveNext()
                            ? _enumerator.Current.Express(ref context)
                                                 .ToList()
                            : new List<Expression>();

            expressions.Add(PipelineContextExpression.Existing);

            var lambda = Expression.Lambda<ResolveDelegate<PipelineContext>>(
                Expression.Block(expressions), PipelineContextExpression.Context);

            return lambda.Compile();
        }

        public ResolveDelegate<PipelineContext>? Pipeline()
        {
            ref var context = ref this;
            return _enumerator.MoveNext()
                 ? _enumerator.Current?.Build(ref context) ?? Seed
                 : Seed;
        }

        public ResolveDelegate<PipelineContext>? Pipeline(ResolveDelegate<PipelineContext>? method = null)
        {
            Seed = method;

            ref var context = ref this;
            return _enumerator.MoveNext()
                 ? _enumerator.Current.Build(ref context) ?? Seed
                 : Seed;
        }

        #endregion
    }
}
