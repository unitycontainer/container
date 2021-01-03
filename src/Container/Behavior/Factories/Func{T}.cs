using System;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class UnityDefaultBehaviorExtension<TContext>
    {
        #region Fields

        private static MethodInfo? FuncPipelineMethodInfo;

        #endregion


        #region Factory

        public static ResolveDelegate<TContext> FuncFactory(ref TContext context)
        {
            var target = context.Type.GenericTypeArguments[0];
            
            return (FuncPipelineMethodInfo ??= typeof(UnityDefaultBehaviorExtension<TContext>)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(FuncPipeline))!)
                .CreatePipeline<TContext>(target);
        }

        #endregion


        #region Implementation

        private static object? FuncPipeline<TElement>(ref PipelineContext context)
        {
            var name  = context.Name;
            var scope = context.Container;

            context.Target = (Func<TElement>)(() => (TElement)scope.Resolve(typeof(TElement), name)!);

            return context.Target;
        }

        #endregion
    }
}
