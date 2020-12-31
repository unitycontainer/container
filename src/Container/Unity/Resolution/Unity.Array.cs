using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo ArrayFactoryMethod 
            = typeof(UnityContainer).GetTypeInfo()
                                    .GetDeclaredMethod(nameof(ArrayPipeline))!;
        #endregion


        #region Unregistered

        private object? ResolveArray(ref PipelineContext context)
        {
            var type = context.Contract.Type;

            if (Policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
                return pipeline!(ref context);

            if (type.GetArrayRank() != 1)  // Verify array is valid
                return context.Error($"Invalid array {type}. Only arrays of rank 1 are supported");

            var element = type.GetElementType()!;
            var target = Policies.ArrayTargetType(this, element!);
            var state = target.IsGenericType
                ? new State(target, target.GetGenericTypeDefinition())
                : new State(target);

            state.Pipeline = ArrayFactoryMethod!.CreatePipeline(element, state);
            Policies.Set<ResolveDelegate<PipelineContext>>(context.Type, state.Pipeline);

            return state.Pipeline!(ref context);
        }

        #endregion



        #region Pipeline

        private static object? ArrayPipeline<TElement>(State state, ref PipelineContext context)
        {
            var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
            if (metadata is null || context.Container.Scope.Version != metadata.Version())
            {
                var manager = context.Container.Scope.GetCache(in context.Contract);

                lock (manager)
                {
                    metadata = (Metadata[]?)(manager.Data as WeakReference)?.Target;
                    if (metadata is null || context.Container.Scope.Version != metadata.Version())
                    {
                        metadata = context.Container.Scope.ToArraySet(state.Types);
                        manager.Data = new WeakReference(metadata);
                    }

                    if (!ReferenceEquals(context.Registration, manager))
                        manager.SetPipeline(context.Container.Scope, state.Pipeline);
                }
            }

            var count = 0;
            var array = new TElement[metadata.Count()];
            var container = context.Container;
            var hash = typeof(TElement).GetHashCode();

            ErrorInfo errorInfo = default;
            Contract contract = default;

            var local = context.CreateContext(ref contract, ref errorInfo);

            for (var i = array.Length; i > 0; i--)
            {
                local.Reset();

                var name = container.Scope[in metadata[i]].Internal.Contract.Name;
                contract = new Contract(Contract.GetHashCode(hash, name!.GetHashCode()), typeof(TElement), name);

                var value = container.Resolve(ref local);

                if (errorInfo.IsFaulted)
                {
                    if (errorInfo.Exception is ArgumentException ex && ex.InnerException is TypeLoadException)
                    {
                        continue; // Ignore
                    }
                    else
                    {
                        context.ErrorInfo = errorInfo;
                        return NoValue;
                    }
                }

                array[count++] = (TElement)value!;
            }

            if (count < array.Length) Array.Resize(ref array, count);

            context.Target = array;

            return array;
        }

        #endregion


        #region Nested State

        private class State
        {
            public readonly Type[] Types;
            public ResolveDelegate<PipelineContext>? Pipeline;
            public State(params Type[] types) => Types = types;

        }
        
        #endregion
    }
}
