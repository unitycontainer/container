using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using Unity.Exceptions;
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

            SeedMethod = null;
            SeedExpression = null;
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

            SeedMethod = registration.Pipeline;
            SeedExpression = null;

            ContainerContext = registration.Owner.Context;

            Debug.Assert(null != registration?.Processors);
            _enumerator = registration.Processors.GetEnumerator();
        }

        // Pipeline from factory
        public PipelineBuilder(Type type, ExplicitRegistration factory, LifetimeManager manager, UnityContainer owner)
        {
            Type = type;
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

            SeedMethod = factory.Pipeline;
            SeedExpression = null;

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

        public ResolveDelegate<PipelineContext>? SeedMethod { get; private set; }

        public IEnumerable<Expression>? SeedExpression { get; private set; }

        #endregion


        #region Public Methods


        public ResolveDelegate<PipelineContext> Compile()
        {
            try
            {
                ref var context = ref this;
                var expressions = _enumerator.MoveNext()
                                ? _enumerator.Current.Express(ref context)
                                                     .ToList()
                                : new List<Expression>();

                expressions.Add(Expression.Label(Unity.Pipeline.ReturnTarget, PipelineContextExpression.Existing));

                var lambda = Expression.Lambda<ResolveDelegate<PipelineContext>>(
                    Expression.Block(expressions), PipelineContextExpression.Context);

                return lambda.Compile();
            }
            catch (Exception ex)
            {
                return (ref PipelineContext context) => throw new InvalidRegistrationException(ex.Message);
            }
        }

        public IEnumerable<Expression> Express()
        {
            ref var context = ref this;
            return _enumerator.MoveNext()
                 ? _enumerator.Current.Express(ref context)
                 : SeedExpression ?? Enumerable.Empty<Expression>();
        }

        public IEnumerable<Expression> Express(ResolveDelegate<PipelineContext> resolver)
        {
            var expression = Expression.Assign(
                    PipelineContextExpression.Existing,
                    Expression.Invoke(Expression.Constant(resolver), PipelineContextExpression.Context));

            SeedExpression = new[] { expression };

            ref var context = ref this;
            return _enumerator.MoveNext()
                 ? _enumerator.Current.Express(ref context)
                 : SeedExpression ?? Enumerable.Empty<Expression>();
        }

        public ResolveDelegate<PipelineContext>? Pipeline()
        {
            ref var context = ref this;
            return _enumerator.MoveNext()
                 ? _enumerator.Current?.Build(ref context) ?? SeedMethod
                 : SeedMethod;
        }

        public ResolveDelegate<PipelineContext>? Pipeline(ResolveDelegate<PipelineContext>? method)
        {
            SeedMethod = method;

            ref var context = ref this;
            return _enumerator.MoveNext()
                 ? _enumerator.Current.Build(ref context) ?? SeedMethod
                 : SeedMethod;
        }

        #endregion
    }
}
