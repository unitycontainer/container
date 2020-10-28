using System.Diagnostics;
using System.Reflection;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo ArrayMethod;

        #endregion


        #region Array

        private static object? ResolveArrayFactory<TElement, TTarget, TGeneric>(ref PipelineContext context)
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
                            data = context.Container.GetRegistrations<TTarget, TGeneric>(false);
                            context.Registration!.Data = data;
                        }
                    }
                }

                var array = new TElement[data!.Count()];
                
                for (var i = 0; i < array.Length; i++)
                { 
                }

                return array;
            }
        }

        #endregion
    }
}
