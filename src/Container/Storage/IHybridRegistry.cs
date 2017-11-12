using Unity.Registration;

namespace Unity.Container.Storage
{

    public interface IHybridRegistry<TKey, TValue> : IIndexerOf<TKey, TValue>
    {
        bool RequireToGrow { get; }
    }
}
