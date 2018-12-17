using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;

namespace Unity.Builder
{
    // TODO: 5.9.0 Consolidate with other implementations
    public static class AttributeResolverFactory
    {
        public static object CreateResolver(FieldInfo info) => null;

        public static object CreateResolver(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false)
                                    .OfType<DependencyResolutionAttribute>()
                                    .First();

            return attribute is OptionalDependencyAttribute dependencyAttribute
                ? (IResolve)new OptionalDependencyResolvePolicy(property.PropertyType, dependencyAttribute.Name)
                : null != attribute.Name
                    ? new NamedTypeDependencyResolvePolicy(property.PropertyType, attribute.Name)
                    : null;
        }

        public static object CreateResolver(ParameterInfo info) => null;
    }
}
