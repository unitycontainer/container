using System;
using System.Diagnostics;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
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
                Debug.Assert(null != context.Registration);

                var tape = context.Registration?.Data as Metadata[];
                if (null == tape || context.Container._scope.Version != tape.Version())
                {
                    lock (context.Registration!)
                    {
                        tape = context.Registration?.Data as Metadata[];
                        if (null == tape || context.Container._scope.Version != tape.Version())
                        {
                            tape = context.Defaults.ArrayToTape(context.Container._scope, types);
                            context.Registration!.Data = tape;
                        }
                    }
                }

                var index = 0;
                var array = new TElement[tape.Length - 1];
                
                for (var i = array.Length; i > 0; i--)
                {
                    ref var record = ref tape[i];
                    if (0 == record.Position) continue;

                    var container = context.Container._ancestry[record.Location];
                    ref var registration = ref container._scope[record.Position];
                    var contract = registration.Internal.Contract;
                    var childContext = context.CreateContext(container, ref contract, registration.Manager!);

                    try
                    {
                        array[index++] = (TElement)container.ResolveRegistration(ref childContext)!;
                    }
                    catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                    {
                        // Ignore
                        record = default;
                    }

                    if (context.IsFaulted) return childContext.Target;
                }

                if (index < array.Length) Array.Resize(ref array, index);

                return array;
            }
        }
    }
}
