using System;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;

namespace Unity.Policy.BuildPlanCreator
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan for creating <see cref="Lazy{T}"/> objects.
    /// </summary>
    public class GenericLazyBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        #region Fields

        private static readonly MethodInfo BuildResolveLazyMethod =
            typeof(GenericLazyBuildPlanCreatorPolicy).GetTypeInfo()
                .GetDeclaredMethod(nameof(BuildResolveLazy));

        #endregion


        #region IBuildPlanCreatorPolicy

        public IBuildPlanPolicy CreatePlan(ref BuilderContext context, Type type, string name)
        {
            var itemType = context.Type.GetTypeInfo().GenericTypeArguments[0];
            var lazyMethod = BuildResolveLazyMethod.MakeGenericMethod(itemType);

            return new DynamicMethodBuildPlan(lazyMethod.CreateDelegate(typeof(ResolveDelegate<BuilderContext>)));
        }

        #endregion


        #region Implementation

        private static object BuildResolveLazy<T>(ref BuilderContext context)
        {
            var container = context.Container;
            var name = context.Name;
            context.Existing = new Lazy<T>(() => container.Resolve<T>(name));

            var lifetime = BuilderStrategy.GetPolicy<LifetimeManager>(ref context,
                context.RegistrationType, context.RegistrationName);

            if (lifetime is PerResolveLifetimeManager)
            {
                var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
                context.Set(context.RegistrationType, context.RegistrationName, 
                    typeof(LifetimeManager), perBuildLifetime);
            }

            return context.Existing;
        }

        #endregion
    }
}
