using System;
using Unity.Resolution;

namespace Unity.Storage
{
    public readonly struct HashKey : IEquatable<HashKey>
    {
        #region Fields

        public readonly int HashCode;
        private readonly int _type;
        private readonly int _name;

        #endregion


        #region Constructors

        public HashKey(int _ = 0)
        {
            _type = 0;
            _name = 0;
            HashCode = 0;
        }

        public HashKey(Type type)
        {
            _type = type?.GetHashCode() ?? 0; 
            _name = -1;
            HashCode = NamedType.GetHashCode(_type, _name) & UnityContainer.HashMask;
        }

        public HashKey(string? name)
        {
            _type = name?.Length ?? 0;
            _name = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(_type, _name) & UnityContainer.HashMask;
        }

        public HashKey(Type type, string? name)
        {
            _type = type?.GetHashCode() ?? 0;
            _name = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(_type, _name) & UnityContainer.HashMask;
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
                   other._type == _type &&
                   other._name == _name;
        }

        public override bool Equals(object obj)
        {
            return obj is HashKey other &&
                   other.HashCode == HashCode &&
                   other._type == _type &&
                   other._name == _name;
        }

        public override int GetHashCode() => HashCode;

        public static bool operator ==(HashKey x, HashKey y)
        {
            return x.HashCode == y.HashCode &&
                   x._type == y._type &&
                   x._name == y._name;
        }
        public static bool operator !=(HashKey x, HashKey y)
        {
            return x.HashCode != y.HashCode ||
                   x._type != y._type ||
                   x._name != y._name;
        }

        #endregion
    }
}
