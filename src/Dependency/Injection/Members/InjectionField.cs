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
        public InjectionField(string name)
            : base(name, ResolvedValue)
        {
        }

        /// <summary>
        /// Configure the container to inject the given field name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="value">InjectionParameterValue for property.</param>
        public InjectionField(string name, object value = null)
            : base(name, value)
        {
        }

        protected InjectionField(FieldInfo info, object value = null)
            : base(info, value)
        {
        }

        #endregion


        #region Overrides

        public override (FieldInfo, object) FromType(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            var declaringType = MemberInfo.DeclaringType.GetTypeInfo();

            if (!declaringType.IsGenericType && !declaringType.ContainsGenericParameters)
                return base.FromType(type);

            var info = type.GetTypeInfo().GetDeclaredField(MemberInfo.Name);
#else
            if (MemberInfo.DeclaringType != null &&
                !MemberInfo.DeclaringType.IsGenericType &&
                !MemberInfo.DeclaringType.ContainsGenericParameters)
                return base.FromType(type);

            var info = type.GetField(MemberInfo.Name);
#endif
            return ReferenceEquals(Data, ResolvedValue)
                ? (info, info)
                : (info, Data);
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

        #endregion


        #region Guards

        protected override void ValidateInjectionMember(Type type)
        {
            base.ValidateInjectionMember(type);

            if (MemberInfo.IsInitOnly)
            {
                throw new InvalidOperationException(
                    $"The field {MemberInfo.Name} on type {MemberInfo.DeclaringType} is not settable.");
            }

            // TODO: Implement
            //if (null != Data && !ReferenceEquals(Data, ResolvedValue) && !Data.MatchesType(MemberInfo.FieldType))
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
