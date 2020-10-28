using System;
using System.Diagnostics;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {


        #region Implementation

        private static object? ResolveArrayFactory<TElement>(Type[] types, ref PipelineContext context)
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

                var data = context.Registration?.Data as Metadata[];
                if (null == data || context.Container._scope.Version != data.Version())
                {
                    lock (context.Registration!)
                    {
                        data = context.Registration?.Data as Metadata[];
                        if (null == data || context.Container._scope.Version != data.Version())
                        {
                            data = context.Container.GetRegistrations(false, types);
                            context.Registration!.Data = data;
                        }
                    }
                }

                var count = data!.Count();
                var array = new TElement[count];
                var scope = context.Container._scope;
                var index = 0;
                
                for (var i = 1; i <= count && !context.IsFaulted; i++)
                {
                    ref var record = ref data[i];
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

        #endregion
    }
}
