using System;
using System.Diagnostics;

namespace Unity.Storage
{
    [DebuggerDisplay("Targeted Node: Type={Type}, Name={Name},  Key={Key},  Value={Value}")]
    public class LinkedNodeTargeted<TKey, TValue> : LinkedNode<TKey, TValue>
    {
        public Type Type { get; set; }

        public string Name { get; set; }
    }
}
