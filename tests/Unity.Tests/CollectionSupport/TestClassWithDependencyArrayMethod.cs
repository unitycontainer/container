// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity;
using Unity.Attributes;

namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithDependencyArrayMethod
    {
        public TestClass[] Dependency { get; set; }

        [InjectionMethod]
        public void Injector(TestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}