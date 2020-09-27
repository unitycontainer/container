using System;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride
    {
        #region Fields

        protected readonly Type? Type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        /// <param name="exact">Indicates if override has to match exactly</param>
        public DependencyOverride(Type type, object? value, bool exact = true)
            : base(null, value, exact)
        {
            Type = type;
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given name
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Value to override with</param>
        /// <param name="exact">Indicates if override has to match exactly</param>
        public DependencyOverride(string name, object? value, bool exact = true)
            : base(name, value, exact)
        {
        }


        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        /// <param name="exact">Indicates if override has to match exactly</param>
        public DependencyOverride(Type type, string? name, object? value, bool exact = true)
            : base(name, value, exact)
        {
            Type = type;
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependency on specific type matching the given type and a name
        /// </summary>
        /// <param name="target">Target type to override dependency on</param>
        /// <param name="name">Name of the dependency</param>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        /// <param name="exact">Indicates if override has to match exactly</param>
        public DependencyOverride(Type? target, Type type, string? name, object? value, bool exact = true)
            : base(target, name, value, exact)
        {
            Type = type;
        }

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? other)
        {
            switch (other)
            {
                case DependencyOverride dependency:
                    return (null == Target || dependency.Target == Target) &&
                           (null == Type   || dependency.Type == Type ) &&
                           (null == Name   || dependency.Name == Name);
                
                case Contract type:
                    return Equals(type);

                default:
                    return false;
            }
        }

        public override MatchRank MatchTo(in Contract other)
        {
            // If names are different - no match
            if (other.Name != Name) return MatchRank.NoMatch;

            // If Type is 'null', all types are compatible
            if (null == Type || null == other.Type) return MatchRank.Compatible;

            // Matches exactly
            if (other.Type == Type) return MatchRank.ExactMatch;

            // Can be assigned to
            if (other.Type.IsAssignableFrom(Type)) return MatchRank.HigherProspect;

            return MatchRank.NoMatch;
        }

        #endregion
    }

    /// <summary>
    /// A convenience version of <see cref="DependencyOverride"/> that lets you
    /// specify the dependency type using generic syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency to override.</typeparam>
    public class DependencyOverride<T> : DependencyOverride
    {
        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type and name match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="target">Target type to override dependency on</param>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Override value</param>
        public DependencyOverride(Type target, string name, object value)
            : base(target, typeof(T), name, value)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type and name match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Override value</param>
        public DependencyOverride(string name, object value)
            : base(null, typeof(T), name, value)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="value">Override value</param>
        public DependencyOverride(object value)
            : base(null, typeof(T), null, value)
        {
        }
    }
}
