using System;

namespace Unity.Resolution
{
    public struct NamedType
    {
        public Type Type;
        public string Name;

        public override int GetHashCode() => ((Type?.GetHashCode() ?? 0) + 37) ^ ((Name?.GetHashCode() ?? 0) + 17);
    }
}
