namespace Unity.Container.Storage
{
    public class LinkedMap<TKey, TValue> : LinkedNode<TKey, TValue>,
                                           IMap<TKey, TValue>
    {
        #region Constructors

        protected LinkedMap()
        {
        }

        public LinkedMap(TKey key, TValue value, LinkedNode<TKey, TValue> next = null)
        {
            Key = key;
            Value = value;
            Next = next;
        }

        #endregion


        #region IMap

        public virtual TValue this[TKey key]
        {
            get
            {
                for (var node = (LinkedNode<TKey, TValue>)this; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
                        return node.Value;
                }

                return default(TValue);
            }
            set
            {
                for (var node = (LinkedNode<TKey, TValue>)this; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
                    {
                        // Found it
                        node.Value = value;
                        return;
                    }
                }

                Next = new LinkedNode<TKey, TValue>
                {
                    Key = key,
                    Next = Next,
                    Value = value
                };
            }
        }

        #endregion
    }
}
