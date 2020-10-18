using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo ArrayMethod =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(GetArray))!;

        private static readonly MethodInfo GenericArrayMethod =
            typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(GetGenericArray))!;

        #endregion


        #region Array

        private static object? ResolveArray(ref Contract contract, ref PipelineContext context)
        {
            throw new NotImplementedException();
            //var context = new ResolveContext(this, in contract, overrides);
            //var resolver = _policies[contract.Type];

            //// Nothing found, requires build
            //if (null == resolver)
            //{
            //    resolver = (ref ResolveContext c) => c.Existing;
            //    _policies[contract.Type] = resolver;
            //}

            //return resolver(ref context);
        }

        /// <summary>
        /// Resolve array
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> the array factory will be stored at</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested array</returns>
        private object? ResolveArray(ref Contract contract, ResolverOverride[] overrides)
        {
            if (contract.Type.GetArrayRank() != 1)
            {
                //var message = $"Invalid array {contract.Type}. Only arrays of rank 1 are supported";
                //return (ref PipelineContext context) => throw new InvalidRegistrationException(message);

                //var resolve = ArrayResolver.Factory(builder.Type, builder.ContainerContext.Container);
                //return builder.Pipeline((ref PipelineContext context) => resolve(ref context));
            }
            else
            {
            }

            var typeArgument = contract.Type.GetElementType();
            Debug.Assert(null != typeArgument);

            var type = ArrayTargetType(typeArgument!);

            ResolveDelegate<PipelineContext> pipeline;

            pipeline = (null != type && typeArgument != type)
                ? (ResolveDelegate<PipelineContext>)GenericArrayMethod.MakeGenericMethod(typeArgument)
                                                                      .CreateDelegate(typeof(ResolveDelegate<PipelineContext>), type)
                : (ResolveDelegate<PipelineContext>)ArrayMethod.MakeGenericMethod(typeArgument)
                                                               .CreateDelegate(typeof(ResolveDelegate<PipelineContext>));

            var request = new RequestInfo(overrides);
            var context = new PipelineContext(this, ref contract, new TransientLifetimeManager(), ref request);

            return pipeline(ref context);
        }

        #endregion

        private static object? GetArray<TElement>(ref PipelineContext context)
        {
            return new TElement[0];
        }

        private static object? GetGenericArray<TElement>(Type type, ref PipelineContext context)
        {
            return new TElement[0];
        }

    }
}
