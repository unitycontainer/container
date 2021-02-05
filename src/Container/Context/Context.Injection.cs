using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TSource> OfType<TSource>()
            where TSource : ISequenceSegment
            => throw new NotImplementedException();
            //=> Registration?.OfType<TSource>() ?? Enumerable.Empty<TSource>();


        public TSource? FirstOrDefault<TSource>(Func<TSource, bool>? predicate = null)
             where TSource : ISequenceSegment
        {
            // TODO: Sequence
            //if (predicate is null)
            //{
            //    for (var next = Registration?.Policies; next is not null; next = next.Next)
            //    {
            //        if (next is TSource source)
            //            return source;
            //    }
            //}
            //else
            //{ 
            //    for (var next = Registration?.Policies; next is not null; next = next.Next)
            //    {
            //        if (next is TSource source && predicate(source)) 
            //            return source;
            //    }
            //}

            return default;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type type)
        {
            return Registration is null
                ? _policies 
                : Registration?.Get(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type type, object policy)
        {
            if (Registration is null)
            {
                _policies = (InjectionMember)policy;
                return;
            }
            Registration?.Set(type, policy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type type)
            => Registration?.Clear(type);

    }
}
