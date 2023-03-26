using System;
using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

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
        public static void Initialize(Policies<BuilderContext> policies)
        {
            #region GetDeclaredMembers() methods, such as GetConstructors(), GetFields(), etc.

            policies.Set(GetConstructors);
            policies.Set(GetFields);
            policies.Set(GetProperties);
            policies.Set(GetMethods);

            #endregion


            #region Selection algorithms

            policies.Set<ConstructorSelector>(SelectConstructor);

            #endregion


            #region Resolution algorithms

            policies.Set<ResolveDelegate<BuilderContext>>(typeof(ContainerRegistration), Algorithms<BuilderContext>.RegisteredAlgorithm);
            policies.Set<ResolveDelegate<BuilderContext>>(Algorithms<BuilderContext>.UnregisteredAlgorithm);

            #endregion


            #region Pipeline Factories

            Pipelines<BuilderContext>.Initialize(policies);

            #endregion


            #region Built-in Type factories

            policies.Set<PipelineFactory<BuilderContext>>(typeof(Lazy<>),        Factories<BuilderContext>.Lazy);
            policies.Set<PipelineFactory<BuilderContext>>(typeof(Func<>),        Factories<BuilderContext>.Func);
            policies.Set<ResolveDelegate<BuilderContext>>(typeof(Array),         Factories<BuilderContext>.Array);
            policies.Set<PipelineFactory<BuilderContext>>(typeof(IEnumerable<>), Factories<BuilderContext>.Enumerable);

            #endregion


            #region Staged chains initializers

            policies.ActivateChain.Add((UnityActivateStage.Creation, new ConstructorStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Fields, new FieldStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Properties, new PropertyStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Methods, new MethodStrategy(policies).PreBuildUp));

            // Factory chain initializer
            policies.FactoryPipeline = FactoryStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Func<IFactoryChain>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityFactoryStage>
                {
                    (UnityFactoryStage.Creation, FactoryStrategy<BuilderContext>.BuilderStrategyDelegate)
                });

            // Mapping chain initializer
            policies.MappingPipeline = MappingStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Func<IMappingChain>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityMappingStage>
                {
                    (UnityMappingStage.TypeMapping, MappingStrategy<BuilderContext>.BuilderStrategyDelegate)
                });

            // Instance chain initializer
            policies.InstancePipeline = InstanceStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Func<IInstanceChain>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityInstanceStage>
                {
                    (UnityInstanceStage.Creation, InstanceStrategy<BuilderContext>.BuilderStrategyDelegate)
                });
            
            #endregion
        }
    }
}
