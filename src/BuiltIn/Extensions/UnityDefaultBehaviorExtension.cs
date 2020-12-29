using System;
using System.Reflection;
using Unity.BuiltIn;
using Unity.Container;

namespace Unity.Extension
{
    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    public class UnityDefaultBehaviorExtension
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            // Default Pipelines
            context.Policies.Set<ResolveDelegate<PipelineContext>>(PipelineProcessor.UnregisteredPipeline);
            context.Policies.Set<RegistrationManager, ResolveDelegate<PipelineContext>>(PipelineProcessor.RegisteredPipeline);

            // Pipeline Factories
            PipelineProcessor.PipelineFactories(context);

            // Array Target type selector
            context.Policies.Set<Array, SelectorDelegate<Type, Type>>(ArrayTypeSelector.Selector);

            // Set Constructor selector
            context.Policies.Set<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(ConstructorSelector.Selector);

            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            context.Policies.Set<ConstructorInfo, MembersSelector<ConstructorInfo>>(MembersSelector.GetConstructors);
            context.Policies.Set<PropertyInfo,    MembersSelector<PropertyInfo>>(MembersSelector.GetProperties);
            context.Policies.Set<MethodInfo,      MembersSelector<MethodInfo>>(MembersSelector.GetMethods);
            context.Policies.Set<FieldInfo,       MembersSelector<FieldInfo>>(MembersSelector.GetFields);

            
            #region Built-In Factories

            EnumFactory.Setup(context);
            LazyFactory.Setup(context);
            FuncFactory.Setup(context);

            #endregion
        }
    }
}
