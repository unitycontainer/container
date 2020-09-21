using System;
using Unity.Policy;

namespace Unity.Resolution
{
    public partial struct ResolutionContext : IPolicySet
    {
        public void Clear(Type type)
        {
        }

        public object? Get(Type type)
        {
            return null;
        }

        public void Set(Type type, object policy)
        {
        }
    }
}
