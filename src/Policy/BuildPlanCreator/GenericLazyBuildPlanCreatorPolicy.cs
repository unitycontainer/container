// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;

namespace Unity.Policy.BuildPlanCreator
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan for creating <see cref="Lazy{T}"/> objects.
    /// </summary>
    public class GenericLazyBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private static readonly MethodInfo BuildResolveLazyMethod;
        private static readonly MethodInfo BuildResolveAllLazyMethod;

        static GenericLazyBuildPlanCreatorPolicy()
        {
            var info = typeof(GenericLazyBuildPlanCreatorPolicy).GetTypeInfo();

            BuildResolveLazyMethod =
                info.GetDeclaredMethod(nameof(BuildResolveLazy));

            BuildResolveAllLazyMethod =
                info.GetDeclaredMethod(nameof(BuildResolveAllLazy));
        }

        /// <summary>
        /// Creates a build plan using the given context and build key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="buildKey">Current build key.</param>
        /// <returns>
        /// The build plan.
        /// </returns>
        public IBuildPlanPolicy CreatePlan(IBuilderContext context, INamedType buildKey)
        {
            return new DynamicMethodBuildPlan(CreateBuildPlanMethod((buildKey ?? throw new ArgumentNullException(nameof(buildKey))).Type));
        }

        private static DynamicBuildPlanMethod CreateBuildPlanMethod(Type lazyType)
        {
            var itemType = lazyType.GetTypeInfo().GenericTypeArguments[0];

            MethodInfo lazyMethod;

            if (itemType.GetTypeInfo().IsGenericType && itemType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                lazyMethod = BuildResolveAllLazyMethod.MakeGenericMethod(itemType.GetTypeInfo().GenericTypeArguments[0]);
            }
            else
            {
                lazyMethod = BuildResolveLazyMethod.MakeGenericMethod(itemType);
            }

            return (DynamicBuildPlanMethod)lazyMethod.CreateDelegate(typeof(DynamicBuildPlanMethod));
        }

        private static void BuildResolveLazy<T>(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = new Lazy<T>(() => context.Container.Resolve<T>(context.BuildKey.Name));
            }

            // match the behavior of DynamicMethodConstructorStrategy
            context.SetPerBuildSingleton();
        }

        private static void BuildResolveAllLazy<T>(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = new Lazy<IEnumerable<T>>(() => context.Container.ResolveAll<T>());
            }

            // match the behavior of DynamicMethodConstructorStrategy
            context.SetPerBuildSingleton();
        }
    }
}
