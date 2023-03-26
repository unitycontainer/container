using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Factories<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private static MethodInfo? LazyPipelineMethodInfo;

        #endregion


        #region Factory


        public static ResolveDelegate<TContext> Lazy(ref TContext context)
        {
            var target = context.Type.GenericTypeArguments[0];
            
            return (LazyPipelineMethodInfo ??= typeof(Factories<TContext>)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(LazyPipeline))!)
                .CreatePipeline<TContext>(target);
        }

        #endregion


        #region Implementation

        private static object? LazyPipeline<TElement>(ref BuilderContext context)
        {
            var name  = context.Name;
            var scope = context.Container;

            context.PerResolve = new Lazy<TElement>(ResolverMethod);

            return context.Existing;

            // Func<TElement>
            TElement ResolverMethod() => (TElement)scope.Resolve(typeof(TElement), name)!;
        }
        
        #endregion
    }
}
