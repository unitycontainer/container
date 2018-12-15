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
    public class InjectionProperty : InjectionMember<PropertyInfo, object>,
                                     IResolve
    {
        #region Fields

        protected readonly string Name;
        private static readonly object Value = new object();

        #endregion


        #region Constructors


        /// <summary>
        /// Configure the container to inject the given property name,
        /// resolving the value via the container.
        /// </summary>
        /// <param name="propertyName">Name of the property to inject.</param>
        public InjectionProperty(string propertyName)
            : base(Value)
        {
            Name = propertyName;
        }

        /// <summary>
        /// Configure the container to inject the given property name,
        /// using the value supplied. This value is converted to an
        /// <see cref="InjectionParameterValue"/> object using the
        /// rules defined by the <see cref="InjectionParameterValue.ToParameters"/>
        /// method.
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        /// <param name="propertyValue">InjectionParameterValue for property.</param>
        public InjectionProperty(string propertyName, object propertyValue)
            : base(propertyValue)
        {
            Name = propertyName;
        }

        #endregion


        #region InjectionMember

        public override bool BuildRequired => true;

        #endregion


        #region IResolverPolicy


        public object Resolve<TContext>(ref TContext context) where TContext : IResolveContext
        {
            if (ReferenceEquals(Data, Value))
            {
                Data = new ResolvedParameter(MemberInfo.PropertyType);
            }

            if (Data is IResolve policy)
                return policy.Resolve(ref context);

            if (Data is IResolverFactory factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(context.Type);
                return resolveDelegate(ref context);
            }

            return Data;
        }

        #endregion


        #region Implementation

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

        protected override bool MemberInfoMatch(PropertyInfo info, object data)
        {
#if NET40
            return info.Name == Name && !info.GetSetMethod(true).IsStatic;
#else
            return info.Name == Name && info.CanWrite && !info.SetMethod.IsStatic;
#endif
        }

        #endregion


        #region Guards

        protected override void OnValidate(Type type)
        {
            base.OnValidate(type);

            // TODO: Optimize
            GuardPropertyExists(MemberInfo, type, Name);
            GuardPropertyIsSettable(MemberInfo);
            GuardPropertyIsNotIndexer(MemberInfo);
            //InitializeParameterValue(propMemberInfo);
            //GuardPropertyValueIsCompatible(propMemberInfo, Value);
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


        #region Platform Compatibility


#if NETSTANDARD1_0
        public override bool Equals(PropertyInfo other)
        {
            return null != other && other.Name == Name;
        }
#endif

        #endregion
    }
}
