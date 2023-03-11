using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified field.
    /// </summary>
    public class FieldOverride : ResolverOverride, 
                                 IMatch<FieldInfo>
    {
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="FieldOverride"/>.
        /// </summary>
        /// <param name="name">The Field name.</param>
        /// <param name="value">InjectionParameterValue to use for the Field.</param>
        /// <param name="exactMatch">Is exact match required for the override</param>
        public FieldOverride(string name, object? value, bool exactMatch = true)
            : base(name ?? throw new ArgumentNullException(nameof(name), "Must provide a name of the field to override"), 
                   value, exactMatch)
        {
        }

        #endregion


        #region  Match Target

        public MatchRank Matches(FieldInfo other)
        {
            return other.Name == Name && (Target is null || other.DeclaringType == Target)
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion
    }
}
