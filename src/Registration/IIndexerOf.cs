
namespace Unity.Registration
{
    public interface IIndexerOf<TKey, TValue>
    {
        TValue this[TKey index] { get; set; }
    }

}
