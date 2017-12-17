namespace Unity.Container
{
    public interface IMap<in TKey, TValue>
    {
        TValue this[TKey index] { get; set; }
    }
}
