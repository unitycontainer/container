using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo? ArrayFactoryMethod;

        #endregion


        #region Unregistered

        private object? ResolveUnregisteredArray(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (type.GetArrayRank() != 1)  // Verify array is valid
                    return context.Error($"Invalid array {type}. Only arrays of rank 1 are supported");

                var element = type.GetElementType()!;
                var target = ArrayTargetType(element!) ?? element;
                var types = target.IsGenericType
                    ? new[] { target, target.GetGenericTypeDefinition() }
                    : new[] { target };

                pipeline = _policies.AddOrGet(context.Type, ArrayFactoryMethod!.CreatePipeline(element, types));
            }

            return pipeline!(ref context);
        }

        #endregion


        #region Factory

        private static object? ArrayPipelineFactory<TElement>(Type[] types, ref PipelineContext context)
        {
            if (null == context.Registration)
            {
                context.Registration = context.Container._scope.GetCache(in context.Contract);
            }

            if (null == context.Registration.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // TODO: threading

                    // Make sure it is still null and not created while waited for the lock
                    if (null == context.Registration.Pipeline)
                    {
                        context.Registration.Pipeline = Resolver;
                        context.Registration.Category = RegistrationCategory.Cache;
                    }
                }
            }

            return context.Registration.Pipeline(ref context);

            ///////////////////////////////////////////////////////////////////
            // Method
            object? Resolver(ref PipelineContext context)
            {
                var version = context.Container._scope.Version;
                var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;

                if (null == metadata || version != metadata.Version())
                {
                    lock (context.Registration!)
                    {
                        metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
                        if (null == metadata || version != metadata.Version())
                        {
                            metadata = context.Defaults.MetaArray(context.Container._scope, types);
                            context.Registration!.Data = new WeakReference(metadata);
                        }
                    }
                }

                var count = 0;
                var array = new TElement[metadata.Count()];
                
                for (var i = array.Length; i > 0; i--)
                {
                    ref var record = ref metadata[i];
                        var container = context.Container._ancestry[record.Location];
                    ref var registration = ref container._scope[record.Position];
                        var contract = new Contract(typeof(TElement), registration.Internal.Contract.Name);
                        var childContext = context.CreateContext(container, ref contract);

                    try
                    {
                        childContext.Resolve();
                        if (context.IsFaulted) return childContext.Target;
                        
                        array[count] = (TElement)childContext.Target!;
                        count += 1;
                    }
                    catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                    {
                        // Ignore
                    }
                }

                if (count < array.Length) Array.Resize(ref array, count);

                context.Target = array;

                return array;
            }
        }

        #endregion
    }
}
