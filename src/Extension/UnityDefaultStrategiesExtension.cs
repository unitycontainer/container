using System;
using System.Reflection;
using System.Collections.Generic;
using Unity.Container;
using Unity.Storage;

namespace Unity.Extension
{
    /// <summary>
    /// This extension installs the default strategies and policies into the container
    /// to implement the standard behavior of the Unity container.
    /// </summary>
    public partial class UnityDefaultStrategiesExtension
    {
        /// <summary>
        /// Add the default <see cref="BuilderStrategy"/> strategies and policies to the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            // Set Member Selectors: GetConstructors(), GetFields(), etc.

            var set = (IPolicySet)context.Policies;
            set.Set<GetMembersDelegate<ConstructorInfo>>(DefaultGetConstructors);
            set.Set<GetMembersDelegate<PropertyInfo>>(DefaultGetProperties);
            set.Set<GetMembersDelegate<MethodInfo>>(DefaultGetMethods);
            set.Set<GetMembersDelegate<FieldInfo>>(DefaultGetFields);


            // Build Stages

            // Type Chain
            context.TypePipelineChain.Add(UnityBuildStage.Creation,     new ConstructorProcessor(context.Policies));
            context.TypePipelineChain.Add(UnityBuildStage.Fields,       new FieldProcessor(context.Policies));
            context.TypePipelineChain.Add(UnityBuildStage.Properties,   new PropertyProcessor(context.Policies));
            context.TypePipelineChain.Add(UnityBuildStage.Methods,      new MethodProcessor(context.Policies));
            // Factory Chain
            context.FactoryPipelineChain.Add(UnityBuildStage.Creation,  new FactoryProcessor());
            // Instance Chain
            context.InstancePipelineChain.Add(UnityBuildStage.Creation, new InstanceProcessor());

            
            // Populate pipelines

            context.Policies.Set(typeof(Defaults.TypeCategory),     BuilderStrategy.BuildUp<PipelineContext>(context.TypePipelineChain));
            context.Policies.Set(typeof(Defaults.FactoryCategory),  BuilderStrategy.BuildUp<PipelineContext>(context.FactoryPipelineChain));
            context.Policies.Set(typeof(Defaults.InstanceCategory), BuilderStrategy.BuildUp<PipelineContext>(context.InstancePipelineChain));

            
            // Rebuild when changed
            
            var policies = context.Policies;

            ((INotifyChainChanged)context.TypePipelineChain).ChainChanged     += OnBuildChainChanged;
            ((INotifyChainChanged)context.FactoryPipelineChain).ChainChanged  += OnBuildChainChanged;
            ((INotifyChainChanged)context.InstancePipelineChain).ChainChanged += OnBuildChainChanged;

            void OnBuildChainChanged(object chain, Type type) 
                => policies.Set(type, BuilderStrategy.BuildUp<PipelineContext>((IEnumerable<BuilderStrategy>)chain));
        }


        #region Default Get Members

        /// <summary>
        /// Determines constructors selected by default when 
        /// <see cref="Type.GetConstructors"/> is called
        /// </summary>
        private static ConstructorInfo[] DefaultGetConstructors(Type type)
            => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);


        /// <summary>
        /// Determines methods selected by default when 
        /// <see cref="Type.GetMethods"/> is called
        /// </summary>
        private static MethodInfo[] DefaultGetMethods(Type type)
            => type.GetMethods(BindingFlags.Public           |
                               BindingFlags.Instance         |
                               BindingFlags.FlattenHierarchy |
                               BindingFlags.DeclaredOnly);


        /// <summary>
        /// Determines fields selected by default when 
        /// <see cref="Type.GetFields"/> is called
        /// </summary>
        private static FieldInfo[] DefaultGetFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.Instance);


        /// <summary>
        /// Determines properties selected by default when 
        /// <see cref="Type.GetProperties"/> is called
        /// </summary>
        private static PropertyInfo[] DefaultGetProperties(Type type)
            => type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        #endregion
    }
}
