using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;
using static Unity.UnityContainer;

namespace Unity
{

    [DebuggerDisplay("Type: {Type?.Name} Name: {Registration?.Name}    Stage: {_enumerator.Current?.GetType().Name}")]
    public ref struct PipelineBuilder
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerator<Pipeline> _enumerator;

        #endregion

        
        #region Constructors

        public PipelineBuilder(Type type, ImplicitRegistration registration, UnityContainer container)
        {
            Type = type;
            Registration = registration;
            ContainerContext = container.Context;

            Seed = null;
            _enumerator = registration.Processors?.GetEnumerator()
                        ?? throw new InvalidOperationException("Processors must be initialized");
        }

        public PipelineBuilder(ref BuilderContext context)
        {
            Type = context.Type;
            Registration = context.Registration;
            ContainerContext = context.ContainerContext;

            Seed = null;
            _enumerator = context.Registration.Processors?.GetEnumerator() 
                        ?? throw new InvalidOperationException("Processors must be initialized");
        }

        #endregion


        #region Public Members

        public Type Type;
        public string? Name => Registration.Name;

        public ResolveDelegate<BuilderContext>? Seed { get; private set; }

        public readonly ContainerContext ContainerContext;

        public readonly ImplicitRegistration Registration;

        #endregion


        #region Public Methods

        public ResolveDelegate<BuilderContext>? Pipeline()
        {
            ref var context = ref this;
            return _enumerator?.MoveNext() ?? false 
                 ? _enumerator.Current.Build(ref context) 
                 : Seed;
        }

        public ResolveDelegate<BuilderContext>? Pipeline(ResolveDelegate<BuilderContext>? method = null)
        {
            Seed = method;

            ref var context = ref this;
            return _enumerator?.MoveNext() ?? false 
                 ? _enumerator.Current.Build(ref context)
                 : Seed;
        }

        #endregion
    }
}
