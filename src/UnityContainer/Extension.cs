using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Extension;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    // Extension Management
    public partial class UnityContainer
    {
        #region Fields

        private ExtensionContext _context;
        private List<UnityContainerExtension> _extensions = new List<UnityContainerExtension>();

        private StagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline { get; } 
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
        private StagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline { get; } 
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
        private StagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline { get; } 
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();

        private event RegistrationEvent TypeRegistered;
        private event RegistrationEvent InstanceRegistered;
        private event RegistrationEvent FactoryRegistered;
        private event ChildCreatedEvent ChildContainerCreated;

        #endregion


        #region Extension Context Implementation

        /// <summary>
        /// Implementation of the ExtensionContext that is used extension management.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the container that 
        /// would otherwise be inaccessible.
        /// </remarks>
        [DebuggerTypeProxy(typeof(ExtensionContext))]
        private class ExtensionContextImpl : ExtensionContext
        {
            #region Constructors

            public ExtensionContextImpl(UnityContainer container) => 
                Container = container;

            #endregion


            #region ExtensionContext

            public override IPolicyList Policies => throw new NotImplementedException();

            public override UnityContainer Container { get; }

            #endregion


            #region Pipelines

            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline 
                => Container.FactoryPipeline;
            
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline 
                => Container.InstancePipeline;
            
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline 
                => Container.TypePipeline;

            #endregion


            #region Events

            public override event RegistrationEvent TypeRegistered
            {
                add => Container.TypeRegistered += value;
                remove => Container.TypeRegistered -= value;
            }

            public override event RegistrationEvent InstanceRegistered
            {
                add => Container.InstanceRegistered += value;
                remove => Container.InstanceRegistered -= value;
            }

            public override event RegistrationEvent FactoryRegistered
            {
                add => Container.FactoryRegistered += value;
                remove => Container.FactoryRegistered -= value;
            }

            public override event ChildCreatedEvent ChildContainerCreated 
            {
                add => Container.ChildContainerCreated += value;
                remove => Container.ChildContainerCreated -= value;
            }

            #endregion
        }

        #endregion
    }
}
