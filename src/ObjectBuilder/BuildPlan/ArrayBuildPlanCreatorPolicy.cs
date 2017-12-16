using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan
{
    public class ArrayBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        #region Fields

        private static readonly MethodInfo ResolveMethod = 
            typeof(ArrayBuildPlanCreatorPolicy).GetTypeInfo()
                                               .GetDeclaredMethod(nameof(BuildArray));
        #endregion


        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            Type elementType = context.OriginalBuildKey.Type.GetElementType();
            var buildMethod = ResolveMethod.MakeGenericMethod(elementType).CreateDelegate(typeof(DynamicBuildPlanMethod));

            return new DynamicMethodBuildPlan((DynamicBuildPlanMethod)buildMethod);
        }

        private static void BuildArray<T>(IBuilderContext context)
        {
            if (null != context.Existing) return;

            
            // TODO: This should be done via IContainerContext
            var container = (UnityContainer)(context.Container ?? context.NewBuildUp<IUnityContainer>());

            // TODO: Error
            //if (typeof(T).GetTypeInfo().IsArray)
            //{
            //    //throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
            //    //    Constants.MustHaveOpenGenericType,
            //    //    itemType.GetTypeInfo().Name));
            //}

            // TODO: Replace with compiled expression
            Type elementType = context.BuildKey.Type.GetElementType();
            context.Existing = container.GetRegisteredNames(container, elementType)
                .Where(registration => null != registration)
                .Select(registration => context.NewBuildUp(new NamedTypeBuildKey(elementType, registration)))
                .Cast<T>()
                .ToArray();
            context.BuildComplete = true;

            context.SetPerBuildSingleton();
        }
    }
}
