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
        private readonly IPolicyList _policies;

        private static readonly MethodInfo BuildResolveLazyMethod;
        private static readonly MethodInfo BuildResolveAllLazyMethod;
        private static readonly MethodInfo BuildResolveLazyEnumerableMethod;

        static GenericLazyBuildPlanCreatorPolicy()
        {
            var info = typeof(GenericLazyBuildPlanCreatorPolicy).GetTypeInfo();

            BuildResolveLazyMethod =
                info.GetDeclaredMethod(nameof(BuildResolveLazy));

            BuildResolveAllLazyMethod =
                info.GetDeclaredMethod(nameof(BuildResolveAllLazy));

            BuildResolveLazyEnumerableMethod =
                info.GetDeclaredMethod(nameof(BuildResolveLazyEnumerable));
        }

        public GenericLazyBuildPlanCreatorPolicy()
        {
            
        }

        public GenericLazyBuildPlanCreatorPolicy(IPolicyList policies)
        {
            _policies = policies;
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
            var method = CreateBuildPlanMethod(buildKey.Type);
            var plan = new DynamicMethodBuildPlan(method);
            
            // Register BuildPlan policy with the container to optimize performance
            _policies?.Set(buildKey.Type, string.Empty, typeof(IBuildPlanPolicy), plan);

            return plan;
        }

        private static DynamicBuildPlanMethod CreateBuildPlanMethod(Type lazyType)
        {
            var itemType = lazyType.GetTypeInfo().GenericTypeArguments[0];

            MethodInfo lazyMethod;

            var info = itemType.GetTypeInfo();
            if (info.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                lazyMethod = BuildResolveLazyEnumerableMethod.MakeGenericMethod(itemType.GetTypeInfo().GenericTypeArguments[0]);
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

            context.SetPerBuildSingleton();
        }

        private static void BuildResolveLazyEnumerable<T>(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = new Lazy<IEnumerable<T>>(() => context.Container.Resolve<IEnumerable<T>>());
            }

            context.SetPerBuildSingleton();
        }

        private static void BuildResolveAllLazy<T>(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = new Lazy<T[]>(() => (T[])context.Container.ResolveAll<T>());
            }

            context.SetPerBuildSingleton();
        }
    }
}
