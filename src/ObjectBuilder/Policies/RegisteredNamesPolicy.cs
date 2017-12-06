// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Unity.ObjectBuilder.Policies
{
    internal class RegisteredNamesPolicy : IRegisteredNamesPolicy
    {
        private readonly UnityContainer _container;

        public RegisteredNamesPolicy(UnityContainer container)
        {
            _container = container;
        }

        public IEnumerable<string> GetRegisteredNames(Type type)
        {
            return _container. .GetKeys(type).Where(s => !string.IsNullOrEmpty(s));
        }
    }
}
