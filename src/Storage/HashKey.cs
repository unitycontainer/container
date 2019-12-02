using System;
using Unity.Resolution;

namespace Unity.Storage
{
    public readonly struct HashKey : IEquatable<HashKey>
    {
        #region Private Fields

        private readonly int _typeHash;
        private readonly int _nameHash;
        
        #endregion


        #region Public Fields

        public readonly Type?   Type;
        public readonly string? Name;
        public readonly int     HashCode;

        #endregion


        #region Constructors

        public HashKey(int _ = 0)
        {
            _typeHash = 0;
            _nameHash = 0;
            HashCode = 0;
            Type = null;
            Name = null;
        }

        public HashKey(Type type)
        {
            _typeHash = type?.GetHashCode() ?? 0; 
            _nameHash = -1;
            HashCode = NamedType.GetHashCode(_typeHash, _nameHash) & UnityContainer.HashMask;
            Type = type;
            Name = null;
        }

        public HashKey(string? name)
        {
            _typeHash = name?.Length ?? 0;
            _nameHash = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(_typeHash, _nameHash) & UnityContainer.HashMask;
            Type = null;
            Name = name;
        }

        public HashKey(Type? type, string? name)
        {
            _typeHash = type?.GetHashCode() ?? 0;
            _nameHash = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(_typeHash, _nameHash) & UnityContainer.HashMask;
            Type = type;
            Name = name;
        }

        #endregion


        #region Public Members

        public static int GetHashCode(Type type)  
            => NamedType.GetHashCode(type.GetHashCode(), -1) & UnityContainer.HashMask;

        public static int GetHashCode(Type? type, string? name)
            => NamedType.GetHashCode(type, name) & UnityContainer.HashMask;

        #endregion


        #region Overrides

        public bool Equals(HashKey other)
        {
            return other.HashCode == HashCode &&
                   other._typeHash == _typeHash &&
                   other._nameHash == _nameHash;
        }

        public override bool Equals(object obj)
        {
            return obj is HashKey other &&
                   other.HashCode == HashCode &&
                   other._typeHash == _typeHash &&
                   other._nameHash == _nameHash;
        }

        public override int GetHashCode() => HashCode;

        public static bool operator ==(HashKey x, HashKey y)
        {
            return x.HashCode == y.HashCode &&
                   x._typeHash == y._typeHash &&
                   x._nameHash == y._nameHash;
        }
        public static bool operator !=(HashKey x, HashKey y)
        {
            return x.HashCode != y.HashCode ||
                   x._typeHash != y._typeHash ||
                   x._nameHash != y._nameHash;
        }

        #endregion
    }
}
