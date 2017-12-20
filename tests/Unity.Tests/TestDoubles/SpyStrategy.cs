// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.Tests.TestDoubles;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Policy;

namespace Unity.Tests.TestDoubles
{
    /// <summary>
    /// A small noop strategy that lets us check afterwards to
    /// see if it ran in the strategy chain.
    /// </summary>
    internal class SpyStrategy : BuilderStrategy
    {
        private IBuilderContext context;
        private object buildKey;
        private object existing;
        private bool buildUpWasCalled;

        public override object PreBuildUp(IBuilderContext context)
        {
            buildUpWasCalled = true;
            this.context = context;
            buildKey = context.BuildKey;
            existing = context.Existing;

            UpdateSpyPolicy(context);
            return null;
        }

        public IBuilderContext Context => context;

        public object BuildKey => buildKey;

        public object Existing => existing;

        public bool BuildUpWasCalled => buildUpWasCalled;

        private void UpdateSpyPolicy(IBuilderContext context)
        {
            SpyPolicy policy = context.Policies.Get<SpyPolicy>(context.BuildKey);
            if (policy != null)
            {
                policy.WasSpiedOn = true;
            }
        }
    }
}
