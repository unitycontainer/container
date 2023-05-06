using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Resolution;

namespace Unity
{
    internal static class InternalExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type, object? target) where T : Delegate
            => (T)method.CreateDelegate(type, target);


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
