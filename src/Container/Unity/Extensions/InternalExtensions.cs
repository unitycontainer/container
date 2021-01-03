using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Extension;

namespace Unity
{
    internal static class InternalExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type) where T : Delegate
            => (T)method.CreateDelegate(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type, object? target) where T : Delegate
            => (T)method.CreateDelegate(type, target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method)
            => (ResolveDelegate<PipelineContext>)method.CreateDelegate(typeof(ResolveDelegate<PipelineContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method, object? target)
            => (ResolveDelegate<PipelineContext>)method.CreateDelegate(typeof(ResolveDelegate<PipelineContext>), target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method, Type type)
            => (ResolveDelegate<PipelineContext>)method.MakeGenericMethod(type)
                                                       .CreateDelegate(typeof(ResolveDelegate<PipelineContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method, Type type, object? target)
            => (ResolveDelegate<PipelineContext>)method.MakeGenericMethod(type)
                                                       .CreateDelegate(typeof(ResolveDelegate<PipelineContext>), target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method, Type element, Type service, object? target)
            => (ResolveDelegate<PipelineContext>)method.MakeGenericMethod(element, service)
                                                       .CreateDelegate(typeof(ResolveDelegate<PipelineContext>), target);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<TContext> CreatePipeline<TContext>(this MethodInfo method, Type type)
            where TContext : IBuilderContext => (ResolveDelegate<TContext>)method.MakeGenericMethod(type)
                                                                                 .CreateDelegate(typeof(ResolveDelegate<TContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<TContext> CreatePipeline<TContext>(this MethodInfo method, Type type, object? target)
            where TContext : IBuilderContext 
            => (ResolveDelegate<TContext>)method.MakeGenericMethod(type)
                                                .CreateDelegate(typeof(ResolveDelegate<TContext>), target);
    }
}
