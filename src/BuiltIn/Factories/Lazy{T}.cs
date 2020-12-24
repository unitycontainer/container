using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static class LazyFactory
    {
        private static Defaults? _policies;
        private static MethodInfo _methodInfo
            = typeof(LazyFactory).GetTypeInfo()
                                 .GetDeclaredMethod(nameof(Factory))!;

        public static void Setup(ExtensionContext context)
        {
            _policies = (Defaults)context.Policies;
            _policies.Set<ResolverFactory<PipelineContext>>(typeof(Lazy<>), TypeFactory);
            _policies.Set<PipelineFactory<PipelineContext>>(typeof(Lazy<>), PipelineFactory);
        }

        private static ResolveDelegate<PipelineContext> TypeFactory(Type type)
        {
            if (!_policies!.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                var target = type.GenericTypeArguments[0];

                pipeline = _methodInfo!.CreatePipeline(target);
                _policies!.Set<ResolveDelegate<PipelineContext>>(type, pipeline);
            }

            return pipeline!;
        }


        private static ResolveDelegate<TContext> PipelineFactory<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            var type = context.Type;
            var pipeline = context.Policies.Get<ResolveDelegate<TContext>>(type);

            if (pipeline is null)
            {
                var target = type.GenericTypeArguments[0];

                pipeline = _methodInfo!.CreatePipeline<TContext>(target);
                context.Policies.Set(type, pipeline);
            }

            return pipeline!;
        }


        private static object? Factory<TElement>(ref PipelineContext context)
        {
            var name  = context.Name;
            var scope = context.Container;

            context.Target = new Lazy<TElement>(ResolverMethod);

            return context.Target;

            // Func<TElement>
            TElement ResolverMethod() => (TElement)scope.Resolve(typeof(TElement), name)!;
        }
    }
}
