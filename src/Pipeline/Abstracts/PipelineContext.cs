using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;
using static Unity.UnityContainer;

namespace Unity.Pipeline
{

    [DebuggerDisplay("Type: {Type?.Name} Name: {Registration?.Name}    Stage: {_enumerator.Current?.GetType().Name}")]
    public ref struct PipelineContext
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerator<PipelineBuilder> _enumerator;

        #endregion


        public PipelineContext(ref BuilderContext context)
        {
            Type = context.RegistrationType;
            Registration = context.Registration;
            ContainerContext = context.ContainerContext;

            Seed = null;
            _enumerator = context.Registration.Processors?.GetEnumerator() 
                        ?? throw new InvalidOperationException("Processors must be initialized");
        }

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
