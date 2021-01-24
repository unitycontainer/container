using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TSource> OfType<TSource>()
            => Registration?.OfType<TSource>() ?? Enumerable.Empty<TSource>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TSource? OfType<TSource>(Func<TSource, bool> predicate)
            => OfType<TSource>().FirstOrDefault(predicate);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type type)
            => Registration?.Get(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type type, object policy)
            => Registration?.Set(type, policy);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type type)
            => Registration?.Clear(type);

    }
}
