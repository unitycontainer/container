using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class FuncFactory
    {
        private static Defaults? _policies;
        private static MethodInfo _methodInfo 
            = typeof(FuncFactory).GetTypeInfo()
                                 .GetDeclaredMethod(nameof(Factory))!;

        public static void Setup(ExtensionContext context)
        {
            _policies = (Defaults)context.Policies;
            _policies.Set<PipelineFactory>(typeof(Func<>), TypeFactory);
        }

        private static ResolveDelegate<PipelineContext> TypeFactory(Type type)
        {
            if (!_policies!.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                var target = type.GenericTypeArguments[0];

                pipeline = _policies!.AddOrGet(type, _methodInfo!.CreatePipeline(target));
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
