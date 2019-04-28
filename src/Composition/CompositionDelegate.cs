using Unity.Resolution;

namespace Unity.Composition
{
    public delegate object? CompositionDelegate(UnityContainer container, object? existing, ResolverOverride[] overrides);
}
