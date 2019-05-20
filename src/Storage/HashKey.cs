using System;
using Unity.Resolution;

namespace Unity.Storage
{
    public readonly struct HashKey : IEquatable<HashKey>
    {
        #region Fields

        public readonly int HashCode;
        public readonly int TypeHash;
        public readonly int NameHash;

        #endregion


        #region Constructors

        public HashKey(Type type)
        {
            TypeHash = type?.GetHashCode() ?? 0; 
            NameHash = -1;
            HashCode = NamedType.GetHashCode(TypeHash, NameHash) & UnityContainer.HashMask;
        }

        public HashKey(string? name)
        {
            TypeHash = name?.Length ?? 0;
            NameHash = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(TypeHash, NameHash) & UnityContainer.HashMask;
        }

        public HashKey(Type type, string? name)
        {
            TypeHash = type?.GetHashCode() ?? 0;
            NameHash = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(TypeHash, NameHash) & UnityContainer.HashMask;
        }

        #endregion


        #region Public Members

        public static int GetHashCode(Type type)  
            => NamedType.GetHashCode(type.GetHashCode(), -1) & UnityContainer.HashMask;

        public static int GetHashCode(Type type, string? name)
            => NamedType.GetHashCode(type, name) & UnityContainer.HashMask;

        #endregion


        #region Overrides

        public bool Equals(HashKey other)
        {
            return other.HashCode == HashCode &&
                   other.TypeHash == TypeHash &&
                   other.NameHash == NameHash;
        }

        public override bool Equals(object obj)
        {
            return obj is HashKey other &&
                   other.HashCode == HashCode &&
                   other.TypeHash == TypeHash &&
                   other.NameHash == NameHash;
        }

        public override int GetHashCode() => HashCode;

        public static bool operator ==(HashKey x, HashKey y)
        {
            return x.HashCode == y.HashCode &&
                   x.TypeHash == y.TypeHash &&
                   x.NameHash == y.NameHash;
        }
        public static bool operator !=(HashKey x, HashKey y)
        {
            return x.HashCode != y.HashCode ||
                   x.TypeHash != y.TypeHash ||
                   x.NameHash != y.NameHash;
        }

        #endregion
    }
}
