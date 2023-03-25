using System;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Factories<TContext>
    {
        #region Fields

        private static MethodInfo? FuncPipelineMethodInfo;

        #endregion


        #region Factory

        public static ResolveDelegate<TContext> Func(ref TContext context)
        {
            var target = context.Type.GenericTypeArguments[0];
            
            return (FuncPipelineMethodInfo ??= typeof(Factories<TContext>)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(FuncPipeline))!)
                .CreatePipeline<TContext>(target);
        }

        #endregion


        #region Implementation

        private static object? FuncPipeline<TElement>(ref BuilderContext context)
        {
            var name  = context.Name;
            var scope = context.Container;

            context.PerResolve = (Func<TElement>)(() => (TElement)scope.Resolve(typeof(TElement), name)!);

            return context.Existing;
        }

        #endregion
    }
}
