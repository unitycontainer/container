// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Container.Registration;

namespace Unity.ObjectBuilder.Policies
{
    internal class RegisteredNamesPolicy : IRegisteredNamesPolicy
    {
        private readonly NamedTypesRegistry _registry;

        public RegisteredNamesPolicy(NamedTypesRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<string> GetRegisteredNames(Type type)
        {
            return _registry.GetKeys(type).Where(s => !string.IsNullOrEmpty(s));
        }
    }
}
