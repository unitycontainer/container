using System.Diagnostics;

namespace Unity.Storage
{
    [DebuggerDisplay("Node:  Key={Key},    InjectionParameterValue={Value}")]
    public class LinkedNode<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public LinkedNode<TKey, TValue> Next;
    }
}
