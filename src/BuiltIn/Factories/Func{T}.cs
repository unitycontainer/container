using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        #region Fields

        private static MethodInfo? _funcPipelineMethodInfo;

        #endregion


        #region Factory

        public static ResolveDelegate<PipelineContext> FuncFactory(Type type)
        {
            var target = type.GenericTypeArguments[0];
            
            return (_funcPipelineMethodInfo ??= typeof(Factories)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(FuncPipeline))!)
                .CreatePipeline(target);
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
