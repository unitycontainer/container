using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity
{
    public static partial class Override
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Property(string name, object value) => new PropertyOverride(name, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Parameter(string name, object value) => new ParameterOverride(name, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ResolverOverride Dependency() => throw new NotImplementedException();
    }
}
