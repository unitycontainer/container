// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace Unity
{
    /// <summary>
    /// Another reflection helper class that has extra methods
    /// for dealing with ParameterInfo.
    /// </summary>
    public class ParameterReflectionHelper : ReflectionHelper
    {
        /// <summary>
        /// Create a new instance of <see cref="ParameterReflectionHelper"/> that
        /// lets you query information about the given ParameterInfo object.
        /// </summary>
        /// <param name="parameter">Parameter to query.</param>
        public ParameterReflectionHelper(ParameterInfo parameter) :
            base(TypeFromParameterInfo(parameter))
        {
        }

        private static Type TypeFromParameterInfo(ParameterInfo parameter)
        {
            return (parameter ?? throw new ArgumentNullException(nameof(parameter))).ParameterType;
        }
    }
}
