using System;

namespace Unity.Resolution
{
    public struct NamedType
    {
        public Type Type;
        public string Name;

        public override int GetHashCode() => GetHashCode(Type, Name);

        public static int GetHashCode(Type type, string name) => ((type?.GetHashCode() ?? 0) + 37) ^ ((name?.GetHashCode() ?? 0) + 17);

        public static int GetHashCode(int typeHash, int nameHash) => (typeHash + 37) ^ (nameHash + 17);

        public override bool Equals(object obj)
        {
            if (obj is NamedType other && Type == other.Type && Name == other.Name)
                return true;

            return false;
        }

        public static bool operator ==(NamedType obj1, NamedType obj2)
        {
            return obj1.Type == obj2.Type && obj1.Name == obj2.Name;
        }

        public static bool operator !=(NamedType obj1, NamedType obj2)
        {
            return obj1.Type != obj2.Type || obj1.Name != obj2.Name;
        }

        public override string ToString()
        {
            return $"Type: {Type?.Name},  Name: {Name}";
        }
    }
}
