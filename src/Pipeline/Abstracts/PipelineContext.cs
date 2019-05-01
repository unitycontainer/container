using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public ref struct PipelineContext
    {
        #region Fields

        private IEnumerator<PipelineBuilder> _enumerator;

        #endregion


        public PipelineContext(ref BuilderContext context)
        {
            Container = (UnityContainer)context.Container;
            Registration = context.Registration;
            Type = context.RegistrationType;

            Seed = null;
            _enumerator = context.Registration.Processors?.GetEnumerator() 
                        ?? throw new InvalidOperationException("Processors must be initialized");
        }

        #region Public Fields

        public readonly ImplicitRegistration Registration;
        public readonly UnityContainer Container;

        public Type Type;

        public ResolveDelegate<BuilderContext>? Seed { get; private set; }

        #endregion

        #region Public Methods

        public ResolveDelegate<BuilderContext>? Pipeline()
        {
            ref var context = ref this;
            return _enumerator?.MoveNext() ?? false 
                 ? _enumerator.Current.Build(ref context) 
                 : Seed;
        }

        public ResolveDelegate<BuilderContext>? Pipeline(ResolveDelegate<BuilderContext>? method)
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
