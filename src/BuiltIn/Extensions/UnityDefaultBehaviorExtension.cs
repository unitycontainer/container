using System;
using System.Collections.Generic;
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

            #region Algorithms

            // Algorithms have built-in support in Policies
            // and could be addressed directly with:
            //
            // Policies.ResolveRegistered
            // Policies.ResolveUnregistered
            // Policies.ResolveArray

            // Unregistered type resolution algorithm
            policies.Set<ResolveDelegate<PipelineContext>>(
                    Algorithms.UnregisteredAlgorithm);

            // Registered type resolution algorithm
            policies.Set<ContainerRegistration, ResolveDelegate<PipelineContext>>(
                Algorithms.RegisteredAlgorithm);

            // Array resolution algorithm
            policies.Set<Array, ResolveDelegate<PipelineContext>>(
                Algorithms.ArrayResolutionAlgorithm);

            #endregion


            // TODO: Proper placement?
            // Pipeline Factories
            PipelineProcessor.PipelineFactories(context);

            #region Various selection predicates

            // Set Constructor selector
            policies.Set<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(ConstructorSelector.Selector);

            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            policies.Set<ConstructorInfo, MembersSelector<ConstructorInfo>>(MembersSelector.GetConstructors);
            policies.Set<PropertyInfo, MembersSelector<PropertyInfo>>(MembersSelector.GetProperties);
            policies.Set<MethodInfo, MembersSelector<MethodInfo>>(MembersSelector.GetMethods);
            policies.Set<FieldInfo, MembersSelector<FieldInfo>>(MembersSelector.GetFields);


            #endregion


            #region Built-In Factories

            policies.Set<FromTypeFactory<PipelineContext>>(typeof(IEnumerable<>), EnumFactory.Factory);
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(Lazy<>), LazyFactory.Factory);
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(Func<>), FuncFactory.Factory);

            #endregion
        }
    }
}
