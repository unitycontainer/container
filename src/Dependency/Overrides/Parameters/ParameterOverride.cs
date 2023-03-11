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
        /// <param name="rank">Indicates if override type has to match exactly</param>
        public ParameterOverride(string name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name, value, rank)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="value">Value to pass for the MethodBase.</param>
        /// <param name="rank">Indicates if override type has to match exactly</param>
        public ParameterOverride(Type type, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(null, value, rank) => Type = type;

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value to pass for the MethodBase.</param>
        /// <param name="rank">Indicates if override has to match exactly</param>
        public ParameterOverride(string? name, Type type, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name, value, rank) => Type = type;

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
