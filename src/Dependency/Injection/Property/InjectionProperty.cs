using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Resolution;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public class InjectionProperty : InjectionMember,
                                     IEquatable<PropertyInfo>,
                                     IResolver
    {
        #region Fields

        protected object Value;
        protected PropertyInfo Info;
        protected readonly string Name;

        #endregion


        #region Constructors


        /// <summary>
        /// Configure the container to inject the given property name,
        /// resolving the value via the container.
        /// </summary>
        /// <param name="propertyName">Name of the property to inject.</param>
        public InjectionProperty(string propertyName)
        {
            Name = propertyName;
            Value = this;
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
        {
            Name = propertyName;
            Value = propertyValue;
        }

        #endregion


        #region InjectionMember

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Interface being registered, ignored in this implementation.</param>
        /// <param name="mappedToType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            Info =
                (mappedToType ?? throw new ArgumentNullException(nameof(mappedToType))).GetPropertiesHierarchical()
                        .FirstOrDefault(p => p.Name == Name &&
                                              !p.GetSetMethod(true).IsStatic);

            GuardPropertyExists(Info, mappedToType, Name);
            GuardPropertyIsSettable(Info);
            GuardPropertyIsNotIndexer(Info);
            //InitializeParameterValue(propInfo);
            //GuardPropertyValueIsCompatible(propInfo, Value);
            // TODO: Optimize
        }

        public override bool BuildRequired => true;

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyInfo other)
            {
                return other.Name == Name;
            }

            return false;
        }

        public bool Equals(PropertyInfo other)
        {
#if NETSTANDARD1_0
            return other.Name == Name;
#else
            return other?.MetadataToken == Info.MetadataToken;
#endif
        }

        #endregion


        #region IResolverPolicy


        public object Resolve<TContext>(ref TContext context) where TContext : IResolveContext
        {
            if (ReferenceEquals(Value, this))
            {
                Value = new ResolvedParameter(Info.PropertyType);
            }

            if (Value is IResolver policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(context.Type);
                return resolveDelegate(ref context);
            }

            return Value;
        }

        #endregion

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
    }
}
