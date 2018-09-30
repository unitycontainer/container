using System.Linq;
using System.Reflection;
using Unity.Attributes;
using Unity.Builder.Selection;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.Policy;
using Unity.ResolverPolicy;

namespace Unity.ObjectBuilder.Policies
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that is aware of
    /// the build keys used by the unity container.
    /// </summary>
    public class DefaultUnityPropertySelectorPolicy : PropertySelectorBase<DependencyResolutionAttribute>
    {
        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IResolverPolicy CreateResolver(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false)
                                    .OfType<DependencyResolutionAttribute>()
                                    .First();

            return attribute is OptionalDependencyAttribute dependencyAttribute
                ? (IResolverPolicy)new OptionalDependencyResolverPolicy(property.PropertyType, dependencyAttribute.Name)
                : new NamedTypeDependencyResolverPolicy(property.PropertyType, attribute.Name);
        }
    }
}
