using System;

namespace Unity.Resolution
{
    /// <summary>
    /// Structure holding <see cref="Type"/> and Name values
    /// </summary>
    public readonly struct NamedType
    {
        #region Public Members

        /// <summary>
        /// HashCode calculated with held <see cref="Type"/> and Name
        /// </summary>
        public readonly int HashCode;

        /// <summary>
        /// <see cref="Type"/> of the structure
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Name of the structure
        /// </summary>
        public readonly string? Name;

        #endregion


        #region Constructors

        public NamedType(Type type, string? name = null)
        {
            Type = type;
            Name = name;
            HashCode = (type.GetHashCode() + 37) ^ ((name?.GetHashCode() ?? 0) + 17);
        }

        #endregion


        #region Implementation

        public override int GetHashCode() => GetHashCode(Type, Name);

        public static int GetHashCode(Type? type, string? name) => ((type?.GetHashCode() ?? 0) + 37) ^ ((name?.GetHashCode() ?? 0) + 17);

        public static int GetHashCode(int typeHash, int nameHash) => (typeHash + 37) ^ (nameHash + 17);

        public override bool Equals(object? obj)
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

        #endregion
    }
}
