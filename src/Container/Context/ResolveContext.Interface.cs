using System;
using Unity.Resolution;

namespace Unity.Container
{
#if NETSTANDARD1_6 || NETCOREAPP1_0 || NETSTANDARD2_0 || NETSTANDARD2_1

    public partial class ResolveContext : IResolveContext
    {
        public Type Type => Contract.Type;

        public string? Name => Contract.Name;

        public object? Resolve(Type type, string? name = null)
        {
            return null;
        }
    }

#else

    public partial struct ResolveContext : IResolveContext
    {
        public readonly Type Type => Contract.Type;

        public readonly string? Name => Contract.Name;

        public readonly object? Resolve(Type type, string? name = null)
        {
            return null;
        }
    }

#endif
}
