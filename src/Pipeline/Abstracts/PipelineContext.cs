using System;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public ref struct PipelineContext
    {
        public PipelineContext(ref BuilderContext context)
        {
            Container = (UnityContainer)context.Container;
            Registration = context.Registration;
            Type = context.RegistrationType;
            Seed = null;
        }

        public readonly ImplicitRegistration Registration;
        public readonly UnityContainer Container;
        ResolveDelegate<BuilderContext>? Seed;
        Type Type;
    }
}
