using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        public InjectionMember<TMemberInfo, TData>? OfType<TMemberInfo, TData>()
            where TMemberInfo : MemberInfo where TData : class
            => (Registration as ISequenceSegment<InjectionMember<TMemberInfo, TData>>)?.Next;

        public IEnumerable<T> OfType<T>()
            => Registration?.OfType<T>() ?? Enumerable.Empty<T>();


        public object? Get(Type type)
            => Registration?.Get(type);

        public void Set(Type type, object policy)
            => Registration?.Set(type, policy);

        public void Clear(Type type)
            => Registration?.Clear(type);

    }
}
