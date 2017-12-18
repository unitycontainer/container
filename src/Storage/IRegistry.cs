using System;
using System.Collections.Generic;

namespace Unity.Storage
{

    public interface IRegistry<TKey, TValue>
    {
        TValue this[TKey index] { get; set; }

        bool RequireToGrow { get; }

        IEnumerable<TKey> Keys { get; }

        IEnumerable<TValue> Values { get; }

        TValue GetOrAdd(TKey key, Func<TValue> factory);

        TValue SetOrReplace(TKey key, TValue value);
    }
}
