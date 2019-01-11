using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Configure the container to inject the given property name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="optional">Tells Unity if this field is optional.</param>
        public InjectionProperty(string name, bool optional = false)
            : base(name, optional ? OptionalDependencyAttribute.Instance
                                  : (object)DependencyAttribute.Instance)
        {
        }

        /// <summary>
        /// Configure the container to inject the given property name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="value">InjectionParameterValue for property.</param>
        public InjectionProperty(string name, object value)
            : base(name, value)
        {
        }

        #endregion


        #region Overrides

        protected override PropertyInfo DeclaredMember(Type type, string name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            return type.GetTypeInfo().GetDeclaredProperty(Selection.Name);
#else
            return type.GetProperty(Selection.Name);
#endif
        }

        public override IEnumerable<PropertyInfo> DeclaredMembers(Type type)
        {
#if NETCOREAPP1_0 || NETSTANDARD1_0
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            var info = type.GetTypeInfo();
            if (type == typeof(object))
            {
                return info.DeclaredProperties; 
            }

            return info.DeclaredProperties
                       .Concat(DeclaredMembers(type.GetTypeInfo().BaseType));
#else
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
#endif
        }

        protected override Type MemberType => Selection.PropertyType;

        public override string ToString()
        {
            return Data is DependencyResolutionAttribute 
                ? $"Resolve.Property('{Name}')"
                : $"Inject.Property('{Name}', {Data})";
        }

        #endregion
    }
}
