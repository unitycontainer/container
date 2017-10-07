// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
    public class ObjectWithOneDependency
    {
        private object inner;

        public ObjectWithOneDependency(object inner)
        {
            this.inner = inner;
        }

        public object InnerObject
        {
            get { return inner; }
        }

        public void Validate()
        {
            Assert.IsNotNull(inner);
        }
    }
}
