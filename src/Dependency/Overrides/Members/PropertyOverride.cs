using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride
    {
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">InjectionParameterValue to use for the property.</param>
        /// <param name="rank">Indicates if override has to match exactly</param>
        public PropertyOverride(string name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name ?? throw new ArgumentNullException(nameof(name)), value, rank)
        {
        }

        #endregion


        #region Match

        public override MatchRank Match(PropertyInfo other, in Contract _)
        {
            return other.Name == Name && (null == Target || other.DeclaringType == Target)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion
    }
}
