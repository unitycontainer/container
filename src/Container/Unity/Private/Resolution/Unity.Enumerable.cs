using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private static IEnumerable<TElement> EnumeratorPipelineFactory<TElement>(Type[] types, ref PipelineContext context)
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

            return (IEnumerable<TElement>)context.Registration.Pipeline(ref context)!;

            ///////////////////////////////////////////////////////////////////
            // Method
            IEnumerable<TElement> Resolver(ref PipelineContext context)
            {
                Debug.Assert(null != context.Registration);

                var metadata = context.Registration?.Data as Metadata[];
                if (null == metadata || context.Container._scope.Version != metadata.Version())
                {
                    lock (context.Registration!)
                    {
                        metadata = context.Registration?.Data as Metadata[];
                        if (null == metadata || context.Container._scope.Version != metadata.Version())
                        {
                            metadata = context.Defaults.MetaEnumeration(context.Container._scope, types);
                            context.Registration!.Data = metadata;
                        }
                    }
                }

                var index = 0;
                var array = new TElement[metadata.Length - 1];

                for (var i = array.Length; i > 0 && !context.IsFaulted; i--)
                {
                    ref var record = ref metadata[i];
                    if (0 == record.Position) continue;

                    var container = context.Container._ancestry[record.Location];
                    ref var registration = ref container._scope[record.Position];

                    try
                    {
                        var contract = registration.Internal.Contract;
                        var childContext = context.CreateContext(container, ref contract, registration.Manager!);

                        array[index++] = (TElement)container.ResolveRegistration(ref childContext)!;
                    }
                    catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                    {
                        // Ignore
                        record = default;
                    }
                }

                if (index < array.Length) Array.Resize(ref array, index);

                return array;
            }
        }
    }
}
