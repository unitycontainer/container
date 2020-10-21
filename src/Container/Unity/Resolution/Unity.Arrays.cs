using System.Diagnostics;
using System.Reflection;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly int INITIAL_COLLECTION_SIZE = 1;

        private static readonly MethodInfo ArrayMethod =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(ElementArray))!;

        private static readonly MethodInfo ArrayGeneric =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(GenericArray))!;

        #endregion


        #region Array

        private static object? ElementArray<TElement, TTarget>(ref PipelineContext context)
        {
            var container = context.Container;
            var version = container._scope.Version;

            if (null == context.Registration)
            {
                context.Registration = container._scope.GetOrCreate(in context.Contract);
            }

            if (null == context.Registration.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (null == context.Registration.Pipeline)
                    {
                        context.Registration.Pipeline = Resolver;
                        context.Registration.Category = RegistrationCategory.Cache;
                    }
                }
            }

            return context.Registration.Pipeline(ref context);

            object? Resolver(ref PipelineContext context)
            {
                Debug.Assert(null != context.Registration);

                if (context.Container._scope.Version != version || null == context.Registration!.Data)
                {
                    lock (context.Registration!)
                    {
                        if (null == context.Registration.Data || context.Container._scope.Version != version)
                        {
                            version = context.Container._scope.Version;
                            // Rebuild metadata
                            context.Registration.Data = new object();
                        }
                    }
                }
                
                var array = new TElement[INITIAL_COLLECTION_SIZE];


                return array;
            }
        }

        private static TElement[] GenericArray<TElement, TTarget, TGeneric>(ref PipelineContext context)
        {
            int count = 0;
            var array = new TElement[INITIAL_COLLECTION_SIZE];


            return array;
        }

        #endregion
    }
}
