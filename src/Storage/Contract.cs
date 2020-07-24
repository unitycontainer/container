using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity
{
    /// <summary>
    /// Structure holding contract information
    /// </summary>
    [DebuggerDisplay("Type = { Type?.Name }, Name = { Name }")]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Contract
    {
        #region Public Members

        /// <summary>
        /// Hash code of the contract
        /// </summary>
        public readonly int HashCode;

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

        internal Contract(int hash, Type type, string? name = null)
        {
            Type = type;
            Name = name;
            HashCode = hash;
        }

        public Contract(Type type, string? name = null)
        {
            Type = type;
            Name = name;
            HashCode = type.GetHashCode() ^ (name?.GetHashCode() ?? 0);
        }

        #endregion


        #region Implementation

        public override int GetHashCode() => HashCode;

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
