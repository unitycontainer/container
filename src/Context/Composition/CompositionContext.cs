using System;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Composition
{
    public ref struct CompositionContext
    {
        public Type Type;
        public string? Name;
        public ResolverOverride[]? Overrides;
        public ImplicitRegistration Registration;
        public UnityContainer Container;
    }
}
