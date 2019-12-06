using System.Diagnostics;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace Unity.Storage
{

    [DebuggerDisplay("Node:  Key={Key},  Value={Value}")]
    public class LinkedNode<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public LinkedNode<TKey, TValue>? Next;
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
