using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity
{
    public static partial class Override
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ResolverOverride Property(string name, object value) => new PropertyOverride(name, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ResolverOverride Parameter(string name, object value) => new ParameterOverride(name, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ResolverOverride Dependency() => throw new NotImplementedException();
    }
}
