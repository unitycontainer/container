using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public class InjectionProperty : MemberInfoBase<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="optional">Tells Unity if this field is optional.</param>
        public InjectionProperty(string name, bool optional = false)
            : base(name, optional ? OptionalDependencyAttribute.Instance
                                  : (object)DependencyAttribute.Instance)
        {
        }

        /// <summary>
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="info"><see cref="PropertyInfo"/> of the property</param>
        /// <param name="optional">Tells Unity if this field is optional.</param>
        public InjectionProperty(PropertyInfo info, bool optional = false)
            : base(info, optional ? OptionalDependencyAttribute.Instance
                                  : (object)DependencyAttribute.Instance)
        {
        }

        /// <summary>
        /// Configures the container to inject the given property with the provided value
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="value">Value to be injected into the property.</param>
        public InjectionProperty(string name, object value)
            : base(name, value)
        {
        }

        /// <summary>
        /// Configures the container to inject the given property with the provided value
        /// </summary>
        /// <param name="info"><see cref="PropertyInfo"/> of the property</param>
        /// <param name="value">Value to be injected into the property.</param>
        public InjectionProperty(PropertyInfo info, object value)
            : base(info, value)
        {
        }

        #endregion


        #region Overrides

        protected override PropertyInfo? DeclaredMember(Type type, string name) => 
            type.GetProperty(Selection!.Name);

        public override IEnumerable<PropertyInfo> DeclaredMembers(Type type) => 
            type.SupportedProperties();

        protected override Type? MemberType => Selection?.PropertyType;

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return base.ToString(debug);
            }
            else
            {
                return Data is DependencyResolutionAttribute
                    ? null == Selection 
                            ? $"Resolve.Property('{Name}')"        
                            : $"Resolve: '{Selection.DeclaringType}.{Name}'"
                    : null == Selection 
                            ? $"Inject.Property('{Name}', {Data})" 
                            : $"Inject: '{Selection.DeclaringType}.{Name}' with {Data})";
            }
        }

        #endregion
    }
}
