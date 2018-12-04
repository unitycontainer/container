using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.ConstructorInvoke
{
    public class ConstructorInvokeBuildPlan : IBuildPlanPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            var targetTypeInfo = context.BuildKey.Type.GetTypeInfo();

            ValidateTargetTypeInfo(context, targetTypeInfo);

            var selectedConstructor = SelectConstructor(context, targetTypeInfo);

            var resolvedParameters = ResolveParameters(context, selectedConstructor);
         
            context.Existing = selectedConstructor
                .Constructor
                .Invoke(resolvedParameters.ToArray());
        }

        private static IEnumerable<object> ResolveParameters(IBuilderContext context, SelectedConstructor selectedConstructor)
        {
            return Enumerable.Zip(
                selectedConstructor.Constructor.GetParameters(),
                selectedConstructor.GetParameterResolvers(),
                (parameter, parameterResolver) => ResolveParameter(context, parameter, parameterResolver));
        }

        private static object ResolveParameter(IBuilderContext context, ParameterInfo parameterInfo, IResolverPolicy parameterResolver)
        {
            var overridden = context.GetOverriddenResolver(parameterInfo.ParameterType);

            return (overridden ?? parameterResolver).Resolve(context);
        }

        private static SelectedConstructor SelectConstructor(IBuilderContext context, TypeInfo targetTypeInfo)
        {
            var selector = context
                .Policies
                .GetPolicy<IConstructorSelectorPolicy>(context.OriginalBuildKey, out var resolverPolicy);

            var selectedConstructor = selector.SelectConstructor(context, resolverPolicy);

            if (selectedConstructor == null)
            {
                ObjectBuilderExceptions.ThrowForNullExistingObject(context);
            }

            var signature = ConstructorHelper.CreateSignatureString(selectedConstructor.Constructor);

            if (selectedConstructor.Constructor.GetParameters().Any(pi => pi.ParameterType.IsByRef))
            {
                ObjectBuilderExceptions.ThrowForNullExistingObjectWithInvalidConstructor(context, signature);
            }

            if (ConstructorHelper.IsInvalidConstructor(targetTypeInfo, context, selectedConstructor))
            {
                ObjectBuilderExceptions.ThrowForReferenceItselfConstructor(context, signature);
            }

            return selectedConstructor;
        }

        private static void ValidateTargetTypeInfo(IBuilderContext context, TypeInfo typeInfo)
        {
            if (typeInfo.IsInterface)
            {
                ObjectBuilderExceptions.ThrowForAttemptingToConstructInterface(context);
            }

            if (typeInfo.IsAbstract)
            {
                ObjectBuilderExceptions.ThrowForAttemptingToConstructAbstractClass(context);
            }

            if (typeInfo.IsSubclassOf(typeof(Delegate)))
            {
                ObjectBuilderExceptions.ThrowForAttemptingToConstructDelegate(context);
            }
        }
    }
}
