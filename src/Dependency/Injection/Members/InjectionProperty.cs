using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public class InjectionProperty : InjectionMember<PropertyInfo, object>
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

        public override PropertyInfo? MemberInfo(Type type)
        {
            if (null != Info && Info.DeclaringType == type) 
                return Info;

            return type.GetProperty(Name);
        }

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return base.ToString(debug);
            }
            else
            {
                return Data is DependencyResolutionAttribute
                    ? null == Info
                            ? $"Resolve.Property('{Name}')"        
                            : $"Resolve: '{Info.DeclaringType}.{Name}'"
                    : null == Info
                            ? $"Inject.Property('{Name}', {Data})" 
                            : $"Inject: '{Info.DeclaringType}.{Name}' with {Data})";
            }
        }

        #endregion
    }
}
