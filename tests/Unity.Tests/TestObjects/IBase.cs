// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.Unity;

namespace Unity.Tests.TestObjects
{
    public interface IBase
    {
        IService Service { get; set; }
    }

    public interface ILazyDependency
    {
        Lazy<EmailService> Service { get; set; }
    }

    public class Base : IBase
    {
        [Dependency]
        public IService Service { get; set; }
    }

    public class LazyDependency : ILazyDependency
    {
        [Dependency]
        public Lazy<EmailService> Service { get; set; }
    }

    public class LazyDependencyConstructor
    {
        private Lazy<EmailService> service = null;
        
        public LazyDependencyConstructor(Lazy<EmailService> s)
        {
            service = s;
        }
    }
}