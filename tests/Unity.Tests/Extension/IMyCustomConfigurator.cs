// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


using Microsoft.Practices.Unity;
using Unity.Extension;

namespace Unity.Tests.Extension
{
    public interface IMyCustomConfigurator : IUnityContainerExtensionConfigurator
    {
        IMyCustomConfigurator AddPolicy();
    }
}