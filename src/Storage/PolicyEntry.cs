using System;

namespace Unity.Storage
{
    public class PolicyEntry
    {
        public Type? Key;
        public object? Value;
        public PolicyEntry? Next;
    }
}
