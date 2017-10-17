// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Unity.Builder.Strategy
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    public interface IBuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        void PreBuildUp(IBuilderContext context);

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        void PostBuildUp(IBuilderContext context);
    }
}
