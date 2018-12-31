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
    public class InjectionProperty : MemberInfoMember<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Configure the container to inject the given property name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        public InjectionProperty(string name)
            : base(name, ResolvedValue)
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

        protected InjectionProperty(PropertyInfo info, object value = null)
            : base(info, value)
        {
        }


        #endregion


        #region Overrides

        public override (PropertyInfo, object) FromType(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            var declaringType = MemberInfo.DeclaringType.GetTypeInfo();

            if (!declaringType.IsGenericType && !declaringType.ContainsGenericParameters)
                return base.FromType(type);

            var info = type.GetTypeInfo().GetDeclaredProperty(MemberInfo.Name);
#else
            if (MemberInfo.DeclaringType != null &&
                !MemberInfo.DeclaringType.IsGenericType &&
                !MemberInfo.DeclaringType.ContainsGenericParameters)
                return base.FromType(type);

            var info = type.GetProperty(MemberInfo.Name);
#endif
            return ReferenceEquals(Data, ResolvedValue)
                ? (info, info)
                : (info, Data);
        }

        protected override IEnumerable<PropertyInfo> DeclaredMembers(Type type)
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

        #endregion


        #region Guards

        protected override void ValidateInjectionMember(Type type)
        {
            base.ValidateInjectionMember(type);

            if (!MemberInfo.CanWrite)
            {
                throw new InvalidOperationException(
                    $"The property {MemberInfo.Name} on type {MemberInfo.DeclaringType} is not settable.");
            }

            if (MemberInfo.GetIndexParameters().Length > 0)
            {
                throw new InvalidOperationException(
                    $"The property {MemberInfo.Name} on type {MemberInfo.DeclaringType} is an indexer. Indexed properties cannot be injected.");
            }

            // TODO: Implement
            //if (!value.MatchesType(property.PropertyType))
            //{
            //    throw new InvalidOperationException(
            //        ExceptionMessage(Constants.PropertyTypeMismatch,
            //            property.Name,
            //            property.DeclaringType,
            //            property.PropertyType,
            //            value.ParameterTypeName));
            //}
        }

        #endregion
    }
}
