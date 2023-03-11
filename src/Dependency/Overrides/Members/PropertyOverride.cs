using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride, 
                                    IMatch<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">InjectionParameterValue to use for the property.</param>
        /// <param name="rank">Indicates if override has to match exactly</param>
        public PropertyOverride(string name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name ?? throw new ArgumentNullException(nameof(name), "Must provide a name of the property to override"), 
                   value, rank)
        {
        }

        #endregion


        #region Match

        public MatchRank Matches(PropertyInfo other)
        {
            return other.Name == Name && (Target is null || other.DeclaringType == Target)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion
    }
}
