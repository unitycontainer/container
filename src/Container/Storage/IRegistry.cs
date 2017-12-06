using System;
using System.Collections.Generic;

namespace Unity.Container.Storage
{

    public interface IRegistry<TKey, TValue> : IMap<TKey, TValue>
    {
        bool RequireToGrow { get; }

        IEnumerable<TKey> Keys { get; }

        IEnumerable<TValue> Values { get; }

        TValue GetOrAdd(TKey key, Func<TValue> factory);

        TValue SetOrReplace(TKey key, TValue value);
    }
}
