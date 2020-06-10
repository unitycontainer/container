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
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="option">Tells Unity if this field is optional.</param>
        public InjectionProperty(string name, ResolutionOption option = ResolutionOption.Required)
            : base(name, ResolutionOption.Optional == option ? OptionalDependencyAttribute.Instance
                                                             : (object)DependencyAttribute.Instance)
        {
        }


        /// <summary>
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="info"><see cref="PropertyInfo"/> of the property</param>
        /// <param name="option">Tells Unity if this field is optional.</param>
        public InjectionProperty(PropertyInfo info, ResolutionOption option = ResolutionOption.Required)
            : base(info, ResolutionOption.Optional == option ? OptionalDependencyAttribute.Instance
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

        protected override PropertyInfo DeclaredMember(Type type, string name)
        {
            return DeclaredMembers(type).FirstOrDefault(p => p.Name == Selection!.Name);
        }

        public override IEnumerable<PropertyInfo> DeclaredMembers(Type type)
        {
            foreach (var member in type.GetDeclaredProperties())
            {
                if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                    continue;

                var setter = member.GetSetMethod(true);
                if (null == setter || setter.IsPrivate || setter.IsFamily)
                    continue;

                yield return member;
            }
        }

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
