using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public static class Processors
    {
        #region Delegates

        
        #endregion


        internal static void Setup(ExtensionContext context)
        {
            // Default policies
            var defaults = (Defaults)context.Policies;

            #region Member Selectors

            defaults.Set<SupportedMembers<ConstructorInfo>>(DefaultSupportedConstructors);
            defaults.Set<SupportedMembers<MethodInfo>>(DefaultSupportedMethods);
            defaults.Set<SupportedMembers<FieldInfo>>(DefaultSupportedFields);
            defaults.Set<SupportedMembers<PropertyInfo>>(DefaultSupportedProperties);

            #endregion


            #region Processors

            // Create processors
            var field       = new FieldProcessor(defaults);
            var method      = new MethodProcessor(defaults);
            var factory     = new FactoryProcessor(defaults);
            var property    = new PropertyProcessor(defaults);
            var instance    = new InstanceProcessor(defaults);
            var constructor = new ConstructorProcessor(defaults);

            #endregion


            #region Chains

            // Initialize Type Chain
            ((StagedChain<UnityBuildStage, BuilderStrategy>)context.TypePipelineChain)
                .Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,   constructor),
                     new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Fields,     field),
                     new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Properties, property),
                     new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Methods,    method));

            // Initialize Factory Chain
            ((StagedChain<UnityBuildStage, BuilderStrategy>)context.FactoryPipelineChain)
                .Add(UnityBuildStage.Creation,    factory);

            // Initialize Instance Chain
            ((StagedChain<UnityBuildStage, BuilderStrategy>)context.InstancePipelineChain)
                .Add(UnityBuildStage.Creation,    instance);

            #endregion

        }


        private static ConstructorInfo[] DefaultSupportedConstructors(Type type) 
            => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        private static MethodInfo[] DefaultSupportedMethods(Type type)
            => type.GetMethods(BindingFlags.Public           |
                               BindingFlags.Instance         |
                               BindingFlags.FlattenHierarchy |
                               BindingFlags.DeclaredOnly);

        private static FieldInfo[] DefaultSupportedFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        private static PropertyInfo[] DefaultSupportedProperties(Type type) 
            => type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }
}
