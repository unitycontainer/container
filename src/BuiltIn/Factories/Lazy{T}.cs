using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        #region Fields

        private static MethodInfo? _lazyPipelineMethodInfo;

        #endregion


        #region Factory

        public static ResolveDelegate<PipelineContext> LazyFactory(Type type)
        {
            var target = type.GenericTypeArguments[0];
            
            return (_lazyPipelineMethodInfo ??= typeof(Factories)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(LazyPipeline))!)
                .CreatePipeline(target);
        }

        #endregion


        #region Implementation

        private static object? LazyPipeline<TElement>(ref PipelineContext context)
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
