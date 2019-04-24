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
        /// <param name="option">Tells Unity if this field is optional.</param>
        public InjectionProperty(string name, ResolutionOption option = ResolutionOption.Required)
            : base(name, ResolutionOption.Optional == option ? OptionalDependencyAttribute.Instance
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
            return DeclaredMembers(type).FirstOrDefault(p => p.Name == Selection.Name);
        }

        public override IEnumerable<PropertyInfo> DeclaredMembers(Type type)
        {
            foreach (var member in type.GetDeclaredProperties())
            {
                if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                    continue;

                var setter = member.GetSetMethod(true);
                if (setter.IsPrivate || setter.IsFamily)
                    continue;

                yield return member;
            }
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
