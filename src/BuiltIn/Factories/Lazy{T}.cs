using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static class LazyFactory
    {
        #region Fields

        private static MethodInfo? _methodInfo;

        #endregion


        #region Factory

        public static ResolveDelegate<PipelineContext> Factory(Type type)
        {
            var target = type.GenericTypeArguments[0];
            
            return (_methodInfo ??= typeof(LazyFactory)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(Pipeline))!)
                .CreatePipeline(target);
        }

        #endregion


        #region Implementation

        private static object? Pipeline<TElement>(ref PipelineContext context)
        {
            var name  = context.Name;
            var scope = context.Container;

            context.Target = new Lazy<TElement>(ResolverMethod);

            return context.Target;

            // Func<TElement>
            TElement ResolverMethod() => (TElement)scope.Resolve(typeof(TElement), name)!;
        }
        
        #endregion
    }
}
