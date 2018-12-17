using System;
using Unity.Policy;

namespace Unity.Builder
{
    public class NamedTypeBase : INamedType
    {
        #region Fields

        private readonly int _hash;

        #endregion


        #region Constructors

        protected NamedTypeBase(Type type, string name)
        {
            Type = type;
            Name = name;
            _hash = (Type?.GetHashCode() ?? 0 + 37) ^ (Name?.GetHashCode() ?? 0 + 17);
        }

        #endregion


        #region INamedType

        public Type Type { get; }

        public string Name { get; }

        #endregion


        #region Object

        public override bool Equals(object obj)
        {
            return obj is INamedType namedType &&
                   ReferenceEquals(Type, namedType.Type) &&
                   Name == namedType.Name;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        #endregion


        #region NamedTypeBuildKey

        public static implicit operator NamedTypeBuildKey(NamedTypeBase namedType)
        {
            return new NamedTypeBuildKey(namedType.Type, namedType.Name);
        }


        #endregion
    }
}
