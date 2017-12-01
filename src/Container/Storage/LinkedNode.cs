namespace Unity.Container.Storage
{
    public class LinkedNode<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public LinkedNode<TKey, TValue> Next;
    }
}
