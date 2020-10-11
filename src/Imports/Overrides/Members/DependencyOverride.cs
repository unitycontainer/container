using System;
using System.Reflection;

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
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(Type type, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(null, value, rank)
        {
            Type = type;
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given name
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Value to override with</param>
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(string name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name, value, rank)
        {
        }


        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(Type type, string? name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(name, value, rank)
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
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(Type? target, Type type, string? name, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(target, name, value, rank)
        {
            Type = type;
        }

        #endregion


        #region  Match

        public override MatchRank Match(FieldInfo other, in Contract contract)
        {
            if ((null != Target && other.DeclaringType != Target) || (contract.Name != Name))
                return MatchRank.NoMatch;

            // If Type is 'null', all types are compatible
            if (null == Type) return MatchRank.Compatible;

            // Matches exactly
            if (contract.Type == Type) return MatchRank.ExactMatch;

            // Can be assigned to
            if (contract.Type.IsAssignableFrom(Type)) return MatchRank.HigherProspect;

            return MatchRank.NoMatch;
        }

        public override MatchRank Match(PropertyInfo other, in Contract contract)
        {
            if ((null != Target && other.DeclaringType != Target) || (contract.Name != Name))
                return MatchRank.NoMatch;

            // If Type is 'null', all types are compatible
            if (null == Type) return MatchRank.Compatible;

            // Matches exactly
            if (contract.Type == Type) return MatchRank.ExactMatch;

            // Can be assigned to
            if (contract.Type.IsAssignableFrom(Type)) return MatchRank.HigherProspect;

            return MatchRank.NoMatch;
        }

        public override MatchRank Match(ParameterInfo other, in Contract contract)
        {
            if ((null != Target && other.Member.DeclaringType != Target) || (contract.Name != Name))
                return MatchRank.NoMatch;

            // If Type is 'null', all types are compatible
            if (null == Type) return MatchRank.Compatible;

            // Matches exactly
            if (contract.Type == Type) return MatchRank.ExactMatch;

            // Can be assigned to
            if (contract.Type.IsAssignableFrom(Type)) return MatchRank.HigherProspect;

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
