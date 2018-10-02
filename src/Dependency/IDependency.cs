using System;
using Unity.Builder;

namespace Unity.Dependency
{
    public interface IDependency : INamedType
    {
        Type Target { get; }
    }
}
