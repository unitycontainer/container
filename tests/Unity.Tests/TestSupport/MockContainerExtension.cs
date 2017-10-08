// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Extension;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class MockContainerExtension : UnityContainerExtension, IMockConfiguration
    {
        private bool initializeWasCalled = false;

        public bool InitializeWasCalled
        {
            get { return this.initializeWasCalled; }
        }

        public new ExtensionContext Context
        {
            get { return base.Context; }
        }

        protected override void Initialize()
        {
            this.initializeWasCalled = true;
        }
    }

    public interface IMockConfiguration : IUnityContainerExtensionConfigurator
    {
    }
}
