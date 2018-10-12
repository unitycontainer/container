using System.Reflection;

namespace Unity.Builder.Selection
{
    /// <summary>
    /// Objects of this type are returned from
    /// <see cref="IPropertySelectorPolicy.SelectProperties"/>.
    /// This class combines the <see cref="PropertyInfo"/> about
    /// the property with the string key used to look up the resolver
    /// for this property's value.
    /// </summary>
    public class SelectedProperty
    {
        /// <summary>
        /// Create an instance of <see cref="SelectedProperty"/>
        /// with the given <see cref="PropertyInfo"/> and key.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="resolver"></param>
        public SelectedProperty(PropertyInfo property, object resolver)
        {
            Property = property;
            Resolver = resolver;
        }

        /// <summary>
        /// PropertyInfo for this property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// IResolverPolicy for this property
        /// </summary>
        public object Resolver { get; }
    }
}
