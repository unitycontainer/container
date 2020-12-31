using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static class FuncFactory
    {
        private static MethodInfo _methodInfo 
            = typeof(FuncFactory).GetTypeInfo()
                                 .GetDeclaredMethod(nameof(Factory))!;

        public static void Setup(ExtensionContext context)
        {
            context.Policies.Set<FromTypeFactory<PipelineContext>>(typeof(Func<>), TypeFactory);
            context.Policies.Set<PipelineFactory<PipelineContext>>(typeof(Func<>), PipelineFactory);
        }

        private static ResolveDelegate<PipelineContext> TypeFactory(Type type)
        {
            var target = type.GenericTypeArguments[0];
            var pipeline = _methodInfo!.CreatePipeline(target);

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

            context.Target = (Func<TElement>)(() => (TElement)scope.Resolve(typeof(TElement), name)!);

            return context.Target;
        }
    }
}
