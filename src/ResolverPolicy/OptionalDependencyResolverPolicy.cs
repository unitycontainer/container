using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// A <see cref="IResolverPolicy"/> that will attempt to
    /// resolve a value, and return null if it cannot rather than throwing.
    /// </summary>
    public class OptionalDependencyResolverPolicy : IResolverPolicy
    {
        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolverPolicy"/> object
        /// that will attempt to resolve the given name and type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        /// <param name="name">Name to resolve with.</param>
        public OptionalDependencyResolverPolicy(Type type, string name)
        {
            if ((type ?? throw new ArgumentNullException(nameof(type))).GetTypeInfo().IsValueType)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Constants.OptionalDependenciesMustBeReferenceTypes,
                        type.GetTypeInfo().Name));
            }

            DependencyType = type;
            Name = name;
        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolverPolicy"/> object
        /// that will attempt to resolve the given type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        public OptionalDependencyResolverPolicy(Type type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Type this resolver will resolve.
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// Name this resolver will resolve.
        /// </summary>
        public string Name { get; }

        #region IResolverPolicy Members

        /// <summary>
        /// GetOrDefault the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            try
            {
                return context.NewBuildUp(DependencyType, Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
