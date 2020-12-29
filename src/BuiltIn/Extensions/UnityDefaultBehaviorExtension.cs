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
            var policies = context.Policies;

            #region Pipelines


            // Unregistered type resolution algorithm
            policies.Set<ResolveDelegate<PipelineContext>>(PipelineProcessor.UnregisteredAlgorithm);

            // Registered type resolution algorithm
            policies.Set<RegistrationManager, ResolveDelegate<PipelineContext>>(PipelineProcessor.RegisteredAlgorithm);
            
            // Pipeline Factories
            PipelineProcessor.PipelineFactories(context);


            #endregion


            #region Various selection predicates


            // Array Target type selector
            policies.Set<Array, SelectorDelegate<Type, Type>>(ArrayTypeSelector.Selector);

            // Set Constructor selector
            policies.Set<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(ConstructorSelector.Selector);
            
            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            policies.Set<ConstructorInfo, MembersSelector<ConstructorInfo>>(MembersSelector.GetConstructors);
            policies.Set<PropertyInfo,    MembersSelector<PropertyInfo>>(MembersSelector.GetProperties);
            policies.Set<MethodInfo,      MembersSelector<MethodInfo>>(MembersSelector.GetMethods);
            policies.Set<FieldInfo,       MembersSelector<FieldInfo>>(MembersSelector.GetFields);


            #endregion


            #region Built-In Type Factories

            EnumFactory.Setup(context);
            LazyFactory.Setup(context);
            FuncFactory.Setup(context);

            #endregion
        }
    }
}
