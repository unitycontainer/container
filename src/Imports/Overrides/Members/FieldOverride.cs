using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified field.
    /// </summary>
    public class FieldOverride : ResolverOverride
    {
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="FieldOverride"/>.
        /// </summary>
        /// <param name="name">The Field name.</param>
        /// <param name="value">InjectionParameterValue to use for the Field.</param>
        /// <param name="rank">Indicates if override has to match exactly</param>
        public FieldOverride(string name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name ?? throw new ArgumentNullException(nameof(name)), value, rank)
        {
        }

        #endregion


        #region  Match Target

        public override MatchRank Match(FieldInfo other, in Contract _)
        {
            return other.Name == Name && (null == Target || other.DeclaringType == Target)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion
    }
}
