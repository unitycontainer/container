using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Build;
using Unity.Builder.Selection;
using Unity.Injection;

namespace Unity.Policy
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> which returns
    /// the set of specific properties that the selector was configured with.
    /// </summary>
    public class SpecifiedPropertiesSelectorPolicy : IPropertySelectorPolicy
    {
        private readonly List<Tuple<PropertyInfo, InjectionParameterValue>> _propertiesAndValues =
            new List<Tuple<PropertyInfo, InjectionParameterValue>>();

        public void AddPropertyAndValue(PropertyInfo property, InjectionParameterValue value)
        {
            _propertiesAndValues.Add(new Tuple<PropertyInfo, InjectionParameterValue>(property, value));
        }

        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public IEnumerable<object> SelectProperties<TContext>(ref TContext context)
            where TContext : IBuildContext
        {
            Type typeToBuild = context.Type;
            var list = new List<SelectedProperty>();
            foreach (Tuple<PropertyInfo, InjectionParameterValue> pair in _propertiesAndValues)
            {
                var currentProperty = pair.Item1;
                var info = pair.Item1.DeclaringType.GetTypeInfo();

                // Is this the property info on the open generic? If so, get the one
                // for the current closed generic.
                if (info.IsGenericType && info.ContainsGenericParameters)
                {
                    currentProperty = context.Type.GetTypeInfo().GetDeclaredProperty(currentProperty.Name);
                }

                list.Add(new SelectedProperty(currentProperty, pair.Item2.GetResolverPolicy(typeToBuild)));
            }

            return list;
        }
    }
}
