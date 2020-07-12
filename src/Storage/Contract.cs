using System;

namespace Unity
{
    /// <summary>
    /// Structure holding contract information
    /// </summary>
    public readonly struct Contract
    {
        #region Public Members

        /// <summary>
        /// <see cref="Type"/> of the contract
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Name of the contract
        /// </summary>
        public readonly string? Name;

        #endregion


        #region Constructors

        public Contract(Type type, string? name = null)
        {
            Type = type;
            Name = name;
        }

        #endregion

        // TODO: switch to HashCode implementation
        #region Implementation

        public override int GetHashCode() => GetHashCode(Type, Name);

        public static int GetHashCode(Type? type, string? name) => (type?.GetHashCode() ?? 0) + 37 ^ (name?.GetHashCode() ?? 0) + 17;

        public static int GetHashCode(int typeHash, int nameHash) => typeHash + 37 ^ nameHash + 17;

        public override bool Equals(object? obj)
        {
            if (obj is Contract other && Type == other.Type && Name == other.Name)
                return true;

            return false;
        }

        public static bool operator ==(Contract obj1, Contract obj2)
        {
            return obj1.Type == obj2.Type && obj1.Name == obj2.Name;
        }

        public static bool operator !=(Contract obj1, Contract obj2)
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
