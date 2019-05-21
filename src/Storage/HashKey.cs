using System;
using Unity.Resolution;

namespace Unity.Storage
{
    public readonly struct HashKey : IEquatable<HashKey>
    {
        #region Fields

        public readonly int HashCode;
        public readonly Type Type;
        private readonly int _name;

        #endregion


        #region Constructors

        public HashKey(int _ = 0)
        {
            _name = 0;
            HashCode = 0;
            Type = typeof(NoType);
        }

        public HashKey(Type type)
        {
            _name = -1;
            HashCode = NamedType.GetHashCode(type?.GetHashCode() ?? 0, _name) & UnityContainer.HashMask;
            Type = type;
        }

        public HashKey(string? name)
        {
            _name = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(typeof(NoType), name) & UnityContainer.HashMask;
            Type = typeof(NoType);
        }

        public HashKey(Type type, string? name)
        {
            _name = name?.GetHashCode() ?? 0;
            HashCode = NamedType.GetHashCode(type, name) & UnityContainer.HashMask;
            Type = type;
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
                   other.Type == Type &&
                   other._name == _name;
        }

        public override bool Equals(object obj)
        {
            return obj is HashKey other &&
                   other.HashCode == HashCode &&
                   other.Type == Type &&
                   other._name == _name;
        }

        public override int GetHashCode() => HashCode;

        public static bool operator ==(HashKey x, HashKey y)
        {
            return x.HashCode == y.HashCode &&
                   x.Type == y.Type &&
                   x._name == y._name;
        }

        public static bool operator !=(HashKey x, HashKey y)
        {
            return x.HashCode != y.HashCode ||
                   x.Type != y.Type ||
                   x._name != y._name;
        }

        #endregion


        #region NoType

        private class NoType { }

        #endregion
    }
}
