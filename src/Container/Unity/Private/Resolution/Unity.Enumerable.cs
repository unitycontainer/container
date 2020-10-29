using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private static IEnumerable<TElement> EnumeratorPipelineFactory<TElement>(Type type, ref PipelineContext context)
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

                var tape = context.Registration?.Data as Metadata[];
                if (null == tape || context.Container._scope.Version != tape.Version())
                {
                    lock (context.Registration!)
                    {
                        tape = context.Registration?.Data as Metadata[];
                        if (null == tape || context.Container._scope.Version != tape.Version())
                        {
                            // TODO: optimize
                            var types = new[] { typeof(TElement), type };

                            tape = context.Defaults.EnumerationToTape(context.Container._scope, types);
                            context.Registration!.Data = tape;
                        }
                    }
                }

                var count = tape.Length - 1;
                var array = new TElement[count];
                var scope = context.Container._scope;
                var index = 0;

                for (var i = tape.Length - 1; i > 0  && !context.IsFaulted; i--)
                {
                    ref var record = ref tape[i];
                    if (0 == record.Position) continue;

                    ref var registration = ref scope[in record];
                    var contract = registration.Internal.Contract;
                    var local = context.CreateContext(ref contract, registration.Manager!);

                    try
                    {
                        array[index++] = (TElement)local.Resolve()!;
                    }
                    catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                    {
                        // Ignore
                        record = default;
                    }
                }

                if (index < count) Array.Resize(ref array, index);

                return array;
            }
        }
    }
}
