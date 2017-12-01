using System;

namespace Unity.Container.Storage
{

    public interface IRegistry<in TKey, TValue> : IMap<TKey, TValue>
    {
        bool RequireToGrow { get; }

        TValue GetOrAdd(TKey key, Func<TValue> factory);

        TValue SetOrReplace(TKey key, TValue value);
    }
}
