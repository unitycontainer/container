using System;
using Unity.Resolution;

namespace Unity.Storage
{
    public readonly struct HashKey : IEquatable<HashKey>
    {
        #region Fields

        public readonly int HashCode;
        public readonly int HashType;
        public readonly int HashName;

        #endregion


        #region Constructors

        public HashKey(Type type)
        {
            HashType = type?.GetHashCode() ?? 0; 
            HashName = -1;
            HashCode = NamedType.GetHashCode(HashType, HashName) & UnityContainer.HashMask;
        }

        public HashKey(string? name)
        {
            HashType = -1;
            HashName = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(HashType, HashName) & UnityContainer.HashMask;
        }

        public HashKey(Type type, string? name)
        {
            HashType = type?.GetHashCode() ?? 0;
            HashName = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(HashType, HashName) & UnityContainer.HashMask;
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
                   other.HashType == HashType &&
                   other.HashName == HashName;
        }

        public override bool Equals(object obj)
        {
            return obj is HashKey other &&
                   other.HashCode == HashCode &&
                   other.HashType == HashType &&
                   other.HashName == HashName;
        }

        public override int GetHashCode() => HashCode;

        public static bool operator ==(HashKey x, HashKey y)
        {
            return x.HashCode == y.HashCode &&
                   x.HashType == y.HashType &&
                   x.HashName == y.HashName;
        }
        public static bool operator !=(HashKey x, HashKey y)
        {
            return x.HashCode != y.HashCode ||
                   x.HashType != y.HashType ||
                   x.HashName != y.HashName;
        }

        #endregion
    }
}
