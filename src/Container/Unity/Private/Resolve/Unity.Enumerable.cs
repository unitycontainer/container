using System;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private static object? EnumeratorPipelineFactory<TElement>(Type[] types, ref PipelineContext context)
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

            return context.Registration.Pipeline(ref context)!;

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
                            metadata = context.Defaults.MetaEnumeration(context.Container._scope, types);
                            context.Registration!.Data = new WeakReference(metadata);
                        }
                    }
                }

                TElement[] array;
                var count = metadata.Count();

                if (0 < count)
                {
                    array = new TElement[count];
                    count = 0;

                    for (var i = array.Length; i > 0; i--)
                    {
                        ref var record = ref metadata[i];
                        var container = context.Container._ancestry[record.Location];
                        ref var registration = ref container._scope[record.Position];
                        var contract = registration.Internal.Contract;
                        var childContext = context.CreateContext(container, ref contract, registration.Manager!);

                        try
                        {
                            container.ResolveRegistration(ref childContext);
                            if (context.IsFaulted) return childContext.Target;

                            array[count] = (TElement)childContext.Target!;
                            count += 1;
                        }
                        catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                        {
                            // Ignore
                        }
                    }
                }
                else
                {
                    var contract = new Contract(types[0], context.Contract.Name);
                    var error = new ErrorInfo();
                    var childContext = context.CreateContext(ref contract, ref error);
                    
                    //array = new TElement[1];

                    try
                    {
                        var value = context.Container.Resolve(ref childContext)!;

                        if (childContext.IsFaulted)
                            array = new TElement[0];
                        else
                        { 
                            count = 1;
                            array = new TElement[] { (TElement)childContext.Target! };
                        };
                    }
                    catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                    {
                        // Ignore
                        array = new TElement[0];
                    }
                }

                if (count < array.Length) Array.Resize(ref array, count);

                context.Target = array;

                return array;
            }
        }
    }
}
