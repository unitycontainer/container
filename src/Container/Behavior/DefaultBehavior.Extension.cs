using Unity.Builder;
using Unity.Extension;

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

            policies.Set<ResolverPipeline>(typeof(ContainerRegistration), Algorithms<BuilderContext>.RegisteredAlgorithm);
            policies.Set<ResolverPipeline>(Algorithms<BuilderContext>.UnregisteredAlgorithm);

            #endregion


            #region Pipeline Factories

            // Converter to compile staged chain of strategies into resolver pipeline
            policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.CompiledChainToPipelineFactory);

            // Converter to compile staged chain of strategies into pipeline factory
            policies.Set<ChainToFactoryConverter>(Pipelines<BuilderContext>.DefaultCompileProcessorFactory);

            #endregion


            #region Built-in Type factories

            policies.Set<ResolverPipeline>(typeof(Array),        Factories<BuilderContext>.Array);
            policies.Set<FactoryPipeline>(typeof(Lazy<>),        Factories<BuilderContext>.Lazy);
            policies.Set<FactoryPipeline>(typeof(Func<>),        Factories<BuilderContext>.Func);
            policies.Set<FactoryPipeline>(typeof(IEnumerable<>), Factories<BuilderContext>.Enumerable);

            #endregion


            #region Staged chains initialization

            policies.ActivateChain.Add((UnityActivateStage.Creation,   new ConstructorStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Fields,     new FieldStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Properties, new PropertyStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Methods,    new MethodStrategy(policies).PreBuildUp));

            // Factory chain initializer
            policies.FactoryPipeline = FactoryStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Action<IFactoryChain>>((chain) => 
                chain.Add(UnityFactoryStage.Creation, FactoryStrategy<BuilderContext>.BuilderStrategyDelegate));

            // Mapping chain initializer
            policies.MappingPipeline = MappingStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Action<IMappingChain>>((chain) => 
                chain.Add(UnityMappingStage.TypeMapping, MappingStrategy<BuilderContext>.BuilderStrategyDelegate));

            // Instance chain initializer
            policies.InstancePipeline = InstanceStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Action<IInstanceChain>>((chain) => 
                chain.Add(UnityInstanceStage.Creation, InstanceStrategy<BuilderContext>.BuilderStrategyDelegate));
            
            #endregion
        }
    }
}
