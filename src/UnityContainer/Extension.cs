using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    // Extension Management
    public partial class UnityContainer : IEnumerable
    {
        #region Fields

        private ExtensionContext _context;
        private List<object> _extensions;

        private StagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline { get; } 
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
        private StagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline { get; } 
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
        private StagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline { get; } 
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();

        private event RegistrationEvent? TypeRegistered;
        private event RegistrationEvent? InstanceRegistered;
        private event RegistrationEvent? FactoryRegistered;
        private event ChildCreatedEvent? ChildContainerCreated;

        #endregion


        #region IEnumerable


        /// <summary>
        /// This method returns <see cref="IEnumerator<Type>"/> with types of installed extensions
        /// </summary>
        /// <returns><see cref="Type"/> enumerator of extensions</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (var extension in _extensions)
            {
                if (!(extension is UnityContainerExtension)) continue;
                yield return extension.GetType();
            }
        }

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
        private class RootExtensionContext : ExtensionContext
        {
            #region Constructors

            public RootExtensionContext(UnityContainer container) => 
                Container = container;

            #endregion


            #region Root Container

            public override IPolicyList Policies => throw new NotImplementedException();

            public override UnityContainer Container { get; }

            public override ILifetimeContainer Lifetime => throw new NotImplementedException();

            #endregion


            #region Pipelines

            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline 
                => Container.FactoryPipeline;
            
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline 
                => Container.InstancePipeline;
            
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline 
                => Container.TypePipeline;

            #endregion


            #region Lifetimes

            public override ITypeLifetimeManager DefaultTypeLifetime 
            { 
                get => throw new NotImplementedException(); 
                set => throw new NotImplementedException(); 
            }

            public override IFactoryLifetimeManager DefaultFactoryLifetime
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
            
            public override IInstanceLifetimeManager DefaultInstanceLifetime
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

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
