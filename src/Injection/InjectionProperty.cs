// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Policy;
using Unity.Policy;
using Unity.Registration;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public class InjectionProperty : InjectionMember
    {
        private readonly string _propertyName;
        private InjectionParameterValue _parameterValue;

        /// <summary>
        /// Configure the container to inject the given property name,
        /// resolving the value via the container.
        /// </summary>
        /// <param name="propertyName">Name of the property to inject.</param>
        public InjectionProperty(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Configure the container to inject the given property name,
        /// using the value supplied. This value is converted to an
        /// <see cref="InjectionParameterValue"/> object using the
        /// rules defined by the <see cref="InjectionParameterValue.ToParameters"/>
        /// method.
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        /// <param name="propertyValue">Value for property.</param>
        public InjectionProperty(string propertyName, object propertyValue)
        {
            _propertyName = propertyName;
            _parameterValue = InjectionParameterValue.ToParameter(propertyValue);
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Interface being registered, ignored in this implementation.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var propInfo =
                (implementationType ?? throw new ArgumentNullException(nameof(implementationType))).GetPropertiesHierarchical()
                        .FirstOrDefault(p => p.Name == _propertyName &&
                                              !p.GetSetMethod(true).IsStatic);

            GuardPropertyExists(propInfo, implementationType, _propertyName);
            GuardPropertyIsSettable(propInfo);
            GuardPropertyIsNotIndexer(propInfo);
            InitializeParameterValue(propInfo);
            GuardPropertyValueIsCompatible(propInfo, _parameterValue);

            SpecifiedPropertiesSelectorPolicy selector =
                GetSelectorPolicy(policies, serviceType, name);

            selector.AddPropertyAndValue(propInfo, _parameterValue);
        }

        public override bool BuildRequired => true;

        private InjectionParameterValue InitializeParameterValue(PropertyInfo propInfo)
        {
            if (_parameterValue == null)
            {
                _parameterValue = new ResolvedParameter(propInfo.PropertyType);
            }
            return _parameterValue;
        }

        private static SpecifiedPropertiesSelectorPolicy GetSelectorPolicy(IPolicyList policies, Type typeToInject, string name)
        {
            NamedTypeBuildKey key = new NamedTypeBuildKey(typeToInject, name);
            IPropertySelectorPolicy selector = 
                (IPropertySelectorPolicy)policies.Get(typeToInject, name, typeof(IPropertySelectorPolicy), out _);
            if (!(selector is SpecifiedPropertiesSelectorPolicy))
            {
                selector = new SpecifiedPropertiesSelectorPolicy();
                policies.Set(key.Type, key.Name, typeof(IPropertySelectorPolicy), selector);
            }
            return (SpecifiedPropertiesSelectorPolicy)selector;
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
    }
}
