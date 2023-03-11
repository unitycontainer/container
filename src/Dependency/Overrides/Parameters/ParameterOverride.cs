using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that lets you
    /// override a named parameter passed to a constructor.
    /// </summary>
    public class ParameterOverride : ResolverOverride, 
                                     IMatch<ParameterInfo>
    {
        #region Fields

        protected readonly Type? Type;

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="name">Name of the constructor parameter.</param>
        /// <param name="value">InjectionParameterValue to pass for the constructor.</param>
        /// <param name="exactMatch">Is exact match required for the override</param>
        public ParameterOverride(string name, object? value, bool exactMatch = true)
            : base(name, value, exactMatch)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="value">Value to pass for the MethodBase.</param>
        /// <param name="exactMatch">Is exact match required for the override</param>
        public ParameterOverride(Type type, object? value, bool exactMatch = true)
            : base(null, value, exactMatch) => Type = type;

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value to pass for the MethodBase.</param>
        /// <param name="exactMatch">Is exact match required for the override</param>
        public ParameterOverride(string? name, Type type, object? value, bool exactMatch = true)
            : base(name, value, exactMatch) => Type = type;

        #endregion


        #region Match

        public MatchRank Matches(ParameterInfo other)
        {
            return (Target is null || other.Member.DeclaringType == Target) &&
                   (Type   is null || other.ParameterType == Type) &&
                   (Name   is null || other.Name == Name)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion
    }
}
