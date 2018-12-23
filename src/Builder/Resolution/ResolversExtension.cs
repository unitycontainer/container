using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;

namespace Unity.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public static class ResolversExtension
    {
        public static object GetResolver(this FieldInfo info) => null;

        public static object GetResolver(this PropertyInfo property)
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

        public static object GetResolver(this ParameterInfo info) => null;
    }
}
