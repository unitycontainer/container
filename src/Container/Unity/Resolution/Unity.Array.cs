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

        private static readonly MethodInfo? ArrayFactoryMethod;

        #endregion


        #region Unregistered

        private object? ResolveUnregisteredArray(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            if (!Policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (type.GetArrayRank() != 1)  // Verify array is valid
                    return context.Error($"Invalid array {type}. Only arrays of rank 1 are supported");

                var element = type.GetElementType()!;
                var target = Policies.GerTargetType(this, element!);
                var types = target.IsGenericType
                    ? new[] { target, target.GetGenericTypeDefinition() }
                    : new[] { target };

                pipeline = ArrayFactoryMethod!.CreatePipeline(element, types);
                Policies.Set<ResolveDelegate<PipelineContext>>(context.Type, pipeline);
            }

            return pipeline!(ref context);
        }

        #endregion


        #region Factory

        private static object? ArrayPipelineFactory<TElement>(Type[] types, ref PipelineContext context)
        {
            // Get Registration
            if (context.Registration is null)
            {
                context.Registration = context.Container.Scope.GetCache(in context.Contract);
            }

            // Get Pipeline
            if (context.Registration.Pipeline is null)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // TODO: threading

                    // Make sure it is still null and not created while waited for the lock
                    if (context.Registration.Pipeline is null)
                    {
                        context.Registration.Pipeline = Resolver;
                        context.Registration.Category = RegistrationCategory.Cache;
                    }
                }
            }

            // Execute
            return context.Registration.Pipeline(ref context);

            ///////////////////////////////////////////////////////////////////
            // Method
            object? Resolver(ref PipelineContext context)
            {
                var version = context.Container.Scope.Version;
                var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;

                if (metadata is null || version != metadata.Version())
                {
                    lock (context.Registration!)
                    {
                        metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
                        if (metadata is null || version != metadata.Version())
                        {
                            metadata = context.Container.Scope.ToArraySet(types);
                            context.Registration!.Data = new WeakReference(metadata);
                        }
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
                            return UnityContainer.NoValue;
                        }
                    }

                    array[count++] = (TElement)value!;
                }

                if (count < array.Length) Array.Resize(ref array, count);

                context.Target = array;

                return array;
            }
        }

        #endregion


        #region Target Type

        private static Type GetArrayTargetType(UnityContainer container, Type argType)
        {
            Type? next;
            Type? type = argType;

            do
            {
                if (type.IsGenericType)
                {
                    if (container.Scope.Contains(type)) return type!;

                    var definition = type.GetGenericTypeDefinition();
                    if (container.Scope.Contains(definition)) return definition;

                    next = type.GenericTypeArguments[0]!;
                    if (container.Scope.Contains(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType()!;
                    if (container.Scope.Contains(next)) return next;
                }
                else
                {
                    return type!;
                }
            }
            while (null != (type = next));

            return argType;
        }

        #endregion
    }
}
