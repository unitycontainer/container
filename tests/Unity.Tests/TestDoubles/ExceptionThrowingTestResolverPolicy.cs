// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity;
using Unity.Builder;
using Unity.Policy;

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles
{
    public class ExceptionThrowingTestResolverPolicy : IResolverPolicy
    {
        private Exception exceptionToThrow;

        public ExceptionThrowingTestResolverPolicy(Exception exceptionToThrow)
        {
            this.exceptionToThrow = exceptionToThrow;
        }

        public object Resolve(IBuilderContext context)
        {
            throw this.exceptionToThrow;
        }
    }
}
