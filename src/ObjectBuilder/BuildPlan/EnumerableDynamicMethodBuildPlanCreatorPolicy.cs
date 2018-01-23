using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan
{
    /// <summary>
    /// An <see cref="IEnumerable{T}"/> implementation
    /// that constructs a build plan for creating <see cref="IBuildPlanCreatorPolicy"/> objects.
    /// </summary>
    public class EnumerableDynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private static readonly MethodInfo ResolveMethod = 
            typeof(EnumerableDynamicMethodBuildPlanCreatorPolicy).GetTypeInfo()
                                                                 .GetDeclaredMethod(nameof(BuildResolveEnumerable));

        private static readonly MethodInfo CastMethod = typeof(System.Linq.Enumerable).GetTypeInfo()
                                                                          .DeclaredMethods
                                                                          .First(m => Equals(m.Name, "Cast"));


        public IBuildPlanPolicy CreatePlan(IBuilderContext context, INamedType buildKey)
        {
            var itemType = (context ?? throw new ArgumentNullException(nameof(context))).BuildKey
                                                                                        .Type
                                                                                        .GetTypeInfo()
                                                                                        .GenericTypeArguments
                                                                                        .First();
            var buildMethod = ResolveMethod.MakeGenericMethod(itemType)
                                           .CreateDelegate(typeof(DynamicBuildPlanMethod));

            return new DynamicMethodBuildPlan((DynamicBuildPlanMethod)buildMethod);
        }

        private static void BuildResolveEnumerable<T>(IBuilderContext context)
        {
            if (null == context.Existing)
            {
                var itemType = typeof(T);
                var itemTypeInfo = itemType.GetTypeInfo();
                var container = context.Container ?? context.NewBuildUp<IUnityContainer>();

                if (itemTypeInfo.IsGenericTypeDefinition)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                Constants.MustHaveOpenGenericType,
                                                itemType.GetTypeInfo().Name));
                }

                var generic = new Lazy<Type>(() => itemType.GetGenericTypeDefinition());
                IEnumerable<object> enumerable = container.Registrations
                                                          .Where(r => r.RegisteredType == itemType || (itemTypeInfo.IsGenericType &&
                                                                      r.RegisteredType.GetTypeInfo().IsGenericTypeDefinition &&
                                                                      r.RegisteredType == generic.Value))
                                                          .Select(r => context.NewBuildUp(new NamedTypeBuildKey(itemType, r.Name)))
                                                          .ToArray();
                context.Existing = CastMethod.MakeGenericMethod(itemType).Invoke(null, new object[] { enumerable });
                context.BuildComplete = true;
            }

            context.SetPerBuildSingleton();
        }
    }
}

