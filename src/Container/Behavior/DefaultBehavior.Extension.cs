﻿using Unity.Builder;
using Unity.Extension;
using Unity.Policy;
using Unity.Processors;

namespace Unity.Container
{
    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    internal static partial class UnityDefaultBehaviorExtension
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        public static void Initialize(Policies policies)
        {
            #region GetDeclaredMembers() methods, such as GetConstructors(), GetFields(), etc.

            policies.Set(GetConstructors);
            policies.Set(GetFields);
            policies.Set(GetProperties);
            policies.Set(GetMethods);

            #endregion


            #region Injection Info Providers

            policies.Set<FieldInfoProvider>(FieldInjectionInfoProvider);
            policies.Set<MethodInfoProvider>(MethodInjectionInfoProvider);
            policies.Set<PropertyInfoProvider>(PropertyInjectionInfoProvider);
            policies.Set<ParameterInfoProvider>(ParameterInjectionInfoProvider);
            policies.Set<ConstructorInfoProvider>(ConstructorInjectionInfoProvider);

            #endregion


            #region Resolution algorithms

            policies.Set<ResolverPipeline>(typeof(ContainerRegistration), Algorithms.RegisteredAlgorithm);
            policies.Set<ResolverPipeline>(Algorithms.UnregisteredAlgorithm);

            #endregion


            #region Pipeline Factories

            policies.Set<ChainToPipelineConverter>(BuildUpChainConverter.ChainToCompiledBuildUpPipeline);
            policies.Set<ChainToFactoryConverter>(BuildUpChainConverter.ChainToBuildUpCompiledFactory);

            #endregion


            #region Built-in Type factories

            policies.Set<ResolverPipeline>(typeof(Array),        Factories<BuilderContext>.Array);
            policies.Set<FactoryPipeline>(typeof(Lazy<>),        Factories<BuilderContext>.Lazy);
            policies.Set<FactoryPipeline>(typeof(Func<>),        Factories<BuilderContext>.Func);
            policies.Set<FactoryPipeline>(typeof(IEnumerable<>), Factories<BuilderContext>.Enumerable);

            #endregion


            #region Staged chains initialization

            var fieldProcessor       = new FieldProcessor(policies);
            var methodProcessor      = new MethodProcessor(policies);
            var propertyProcessor    = new PropertyProcessor(policies);
            var constructorProcessor = new ConstructorProcessor(policies);

            // Build plan chain initializer
            policies.BuildPlanChain.Add((UnityBuildStage.Creation,   constructorProcessor),
                                        (UnityBuildStage.Fields,     fieldProcessor),
                                        (UnityBuildStage.Properties, propertyProcessor),
                                        (UnityBuildStage.Methods,    methodProcessor));

            // Activation chain initializer
            policies.ActivationChain.Add((UnityActivationStage.Creation,   constructorProcessor.BuildUp),
                                         (UnityActivationStage.Fields,     fieldProcessor.BuildUp),
                                         (UnityActivationStage.Properties, propertyProcessor.BuildUp),
                                         (UnityActivationStage.Methods,    methodProcessor.BuildUp));

            // Factory chain initializer
            policies.FactoryPipeline = FactoryStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Action<IFactoryChain>>((chain) => 
                chain.Add(UnityFactoryStage.Creation, FactoryStrategy<BuilderContext>.FactoryBuilderStrategy));

            // Mapping chain initializer
            policies.MappingPipeline = MappingStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Action<IMappingChain>>((chain) => 
                chain.Add(UnityMappingStage.TypeMapping, MappingStrategy<BuilderContext>.MappingBuilderStrategy));

            // Instance chain initializer
            policies.InstancePipeline = InstanceStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Action<IInstanceChain>>((chain) => 
                chain.Add(UnityInstanceStage.Creation, InstanceStrategy<BuilderContext>.InstanceBuilderStrategy));
            
            #endregion
        }
    }
}