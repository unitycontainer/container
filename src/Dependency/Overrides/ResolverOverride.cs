using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// Base class for all override objects passed in the
    /// <see cref="IUnityContainer.Resolve"/> method.
    /// </summary>
    public abstract class ResolverOverride : IMatchContract<FieldInfo>,
                                             IMatchContract<PropertyInfo>,
                                             IMatchContract<ParameterInfo> 
    {
        #region Fields

        protected Type?              Target;
        protected readonly string?   Name;
        public    readonly object?   Value;
        public    readonly MatchRank RequireRank;

        #endregion


        #region Constructors

        /// <summary>
        /// This constructor is used when no target is required
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Value to pass to resolver</param>
        /// <param name="rank">Minimal required rank to override</param>
        protected ResolverOverride(string? name, object? value, MatchRank rank)
        {
            Name = name;
            Value = value;
            RequireRank = rank;
        }

        /// <summary>
        /// This constructor is used with targeted overrides
        /// </summary>
        /// <param name="target"><see cref="Type"/> of the target</param>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Value to pass to resolver</param>
        /// <param name="rank">Minimal required rank to override</param>
        protected ResolverOverride(Type? target, string? name, object? value, MatchRank rank)
        {
            Target = target;
            Name = name;
            Value = value;
            RequireRank = rank;
        }

        #endregion


        #region Match Contract

        public virtual MatchRank Match(FieldInfo other, in Contract contract) => MatchRank.NoMatch;

        public virtual MatchRank Match(PropertyInfo other, in Contract contract) => MatchRank.NoMatch;

        public virtual MatchRank Match(ParameterInfo other, in Contract contract) => MatchRank.NoMatch;

        #endregion


        #region Type Based Override

        /// <summary>
        /// This method adds target information to the override. Only targeted
        /// <see cref="Type"/> will be overridden even if other dependencies match
        /// the type of the name of the override.
        /// </summary>
        /// <typeparam name="T">Type to constrain the override to.</typeparam>
        /// <returns>The new override.</returns>
        public ResolverOverride OnType<T>()
        {
            Target = typeof(T);
            return this;
        }

        /// <summary>
        /// This method adds target information to the override. Only targeted
        /// <see cref="Type"/> will be overridden even if other dependencies match
        /// the type of the name of the override.
        /// </summary>
        /// <param name="targetType">Type to constrain the override to.</param>
        /// <returns>The new override.</returns>
        public ResolverOverride OnType(Type targetType)
        {
            Target = targetType;
            return this;
        }

        #endregion
    }
}

