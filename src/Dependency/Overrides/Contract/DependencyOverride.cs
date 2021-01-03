using System;
using Unity.Container;
using Unity.Extension;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride, IMatchImport
    {
        #region Fields

        protected readonly Type? Type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type
        /// </summary>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(Type contractType, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(Contract.AnyContractName, value, rank) => Type = contractType;

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given name
        /// </summary>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(string contractName, object? value, MatchRank rank = MatchRank.Compatible)
            : base(contractName, value, rank)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(Type contractType, string? contractName, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(contractName, value, rank) => Type = contractType;

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependency on specific type matching the given type and a name
        /// </summary>
        /// <param name="targetType">Target <see cref="Type"/> to override dependency on</param>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        /// <param name="rank">Minimal required rank to override</param>
        public DependencyOverride(Type? targetType, Type contractType, string? contractName, object? value, MatchRank rank = MatchRank.ExactMatch)
            : base(targetType, contractName, value, rank) => Type = contractType;

        #endregion


        #region  Match

        public MatchRank MatchImport<T>(in T other) where T : IImportInfo
        {
            if (null != Target && other.DeclaringType != Target)
                return MatchRank.NoMatch;

            if (!ReferenceEquals(Contract.AnyContractName, Name) && (other.ContractName != Name))
                return MatchRank.NoMatch;

            // If Type is 'null', all types are compatible
            if (Type is null) return Value.MatchTo(other.ContractType);

            // Matches exactly
            if (other.ContractType == Type) return MatchRank.ExactMatch;

            // Can be assigned to
            if (other.ContractType.IsAssignableFrom(Type)) return MatchRank.HigherProspect;

            return MatchRank.NoMatch;
        }

        #endregion
    }

    #region Generic

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

    #endregion
}
