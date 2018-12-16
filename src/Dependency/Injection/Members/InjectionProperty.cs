using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

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

        public override (PropertyInfo, object) Select(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            var declaringType = MemberInfo.DeclaringType.GetTypeInfo();

            if (!declaringType.IsGenericType && !declaringType.ContainsGenericParameters)
                return base.Select(type);

            var info = type.GetTypeInfo().GetDeclaredProperty(MemberInfo.Name);
#else
            if ( MemberInfo.DeclaringType != null && 
                !MemberInfo.DeclaringType.IsGenericType && 
                !MemberInfo.DeclaringType.ContainsGenericParameters)
                return base.Select(type);

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

        protected override bool MatchMemberInfo(PropertyInfo info, object data)
        {
#if NET40
            return info.Name == Name && !info.GetSetMethod(true).IsStatic;
#else
            return info.Name == Name && info.CanWrite && !info.SetMethod.IsStatic;
#endif
        }

#endregion


#region Guards

        protected override void ValidateInjectionMember(Type type)
        {
            base.ValidateInjectionMember(type);

            // TODO: Optimize
            GuardPropertyExists(MemberInfo, type, Name);
            GuardPropertyIsSettable(MemberInfo);
            GuardPropertyIsNotIndexer(MemberInfo);
            //InitializeParameterValue(propMemberInfo);
            //GuardPropertyValueIsCompatible(propMemberInfo, _dummy);
        }

        private static void GuardPropertyExists(PropertyInfo propInfo, Type typeToCreate, string propertyName)
        {
            if (propInfo == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Constants.NoSuchProperty,
                        typeToCreate.GetTypeInfo().Name,
                        propertyName));
            }
        }

        private static void GuardPropertyIsSettable(PropertyInfo propInfo)
        {
            if (!propInfo.CanWrite)
            {
                throw new InvalidOperationException(
                    ExceptionMessage(Constants.PropertyNotSettable,
                        propInfo.Name, propInfo.DeclaringType));
            }
        }

        private static void GuardPropertyIsNotIndexer(PropertyInfo property)
        {
            if (property.GetIndexParameters().Length > 0)
            {
                throw new InvalidOperationException(
                    ExceptionMessage(Constants.CannotInjectIndexer,
                        property.Name, property.DeclaringType));
            }
        }

        private static void GuardPropertyValueIsCompatible(PropertyInfo property, InjectionParameterValue value)
        {
            if (!value.MatchesType(property.PropertyType))
            {
                throw new InvalidOperationException(
                    ExceptionMessage(Constants.PropertyTypeMismatch,
                                     property.Name,
                                     property.DeclaringType,
                                     property.PropertyType,
                                     value.ParameterTypeName));
            }
        }

        private static string ExceptionMessage(string format, params object[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] is Type)
                {
                    args[i] = ((Type)args[i]).GetTypeInfo().Name;
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static IEnumerable<PropertyInfo> GetPropertiesHierarchical(Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo().DeclaredProperties
                .Concat(GetPropertiesHierarchical(type.GetTypeInfo().BaseType));
        }

#endregion
    }
}
