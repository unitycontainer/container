using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public class InjectionField : MemberInfoMember<FieldInfo>
    {
        #region Constructors

        /// <summary>
        /// Configure the container to inject the given field name.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="optional">Tells Unity if this field is optional.</param>
        public InjectionField(string name, bool optional = false)
            : base(name, optional ? OptionalDependencyAttribute.Instance 
                                  : (object)DependencyAttribute.Instance)
        {
        }

        /// <summary>
        /// Configure the container to inject the given field name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="value">InjectionParameterValue for property.</param>
        public InjectionField(string name, object value)
            : base(name, value)
        {
        }

        #endregion


        #region Overrides

        protected override FieldInfo DeclaredMember(Type type, string name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            return type.GetTypeInfo().GetDeclaredField(MemberInfo.Name);
#else
            return type.GetField(MemberInfo.Name);
#endif
        }

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
#if NETCOREAPP1_0 || NETSTANDARD1_0
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            var info = type.GetTypeInfo();
            if (type == typeof(object))
            {
                return info.DeclaredFields;
            }

            return info.DeclaredFields
                       .Concat(DeclaredMembers(type.GetTypeInfo().BaseType));
#else
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
#endif
        }

        protected override Type MemberType => MemberInfo.FieldType;

        protected override void ValidateInjectionMember(Type type)
        {
            base.ValidateInjectionMember(type);

            if (MemberInfo.IsInitOnly)
            {
                throw new InvalidOperationException(
                    $"The field {MemberInfo.Name} on type {MemberInfo.DeclaringType} is not settable.");
            }
        }

        #endregion
    }
}
