// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using Unity.Builder.Strategy;
using Unity.Policy;

namespace Unity.Strategy
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public interface IStrategyChain : IEnumerable<BuilderStrategy>, IBuildPlanPolicy
    {
    }
}
