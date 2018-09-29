using System;
using System.Reflection;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride
    {
        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">Value to use for the property.</param>
        public PropertyOverride(string propertyName, object propertyValue)
            : base(propertyName, propertyValue ?? throw new ArgumentNullException(nameof(propertyValue)))
        {
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IResolverPolicy GetResolver<TBuilderContext>(ref TBuilderContext context, Type dependencyType)
        {
            if (context.CurrentOperation is PropertyInfo info && info.Name == Name)
            {
                return Value.GetResolverPolicy(dependencyType);
            }
            return null;
        }
    }
}
