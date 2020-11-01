using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

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
            _policies.Set<PipelineFactory<Type>>(typeof(Lazy<>), TypeFactory);
        }

        private static ResolveDelegate<PipelineContext> TypeFactory(ref Type type)
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

            // TODO: Requires verification
            context.Target = new Lazy<TElement>(() => (TElement)scope.Resolve(typeof(TElement), name)!);

            // TODO: PerResolveLifetimeManager
            //var lifetime = BuilderStrategy.GetPolicy<LifetimeManager>(ref context);

            //if (lifetime is PerResolveLifetimeManager)
            //{
            //    var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
            //    context.Set(typeof(LifetimeManager), perBuildLifetime);
            //}

            return context.Target;
        }
    }
}
