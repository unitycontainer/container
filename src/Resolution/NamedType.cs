using System;

namespace Unity.Resolution
{
    public struct NamedType
    {
        public Type Type;
        public string Name;

        public override int GetHashCode() => GetHashCode(Type, Name);

        public static int GetHashCode(Type type, string name) => ((type?.GetHashCode() ?? 0) + 37) ^ ((name?.GetHashCode() ?? 0) + 17);

    }
}
