using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo? ArrayFactoryMethod;
        private static readonly MethodInfo? EnumerableFactoryMethod;

        #endregion


        #region POCO

        // TODO: Contract??
        private static object? ResolveUnregistered(ref Contract contract, ref PipelineContext parent)
        {
            return parent.Error("NotImplementedException");
            //throw new NotImplementedException();
            //// Check if resolver already exist
            //var resolver = _policies[contract.Type];

            //// Nothing found, requires build
            //if (null == resolver)
            //{
            //    // Build new and try to save it
            //    resolver = _policies.UnregisteredPipelineFactory(in contract);
            //    resolver = _policies.GetOrAdd(contract.Type, resolver);
            //}

            //var context = new ResolveContext(this, in contract, overrides);
            //return resolver(ref context);
        }

        #endregion


        #region Generic

        private object? ResolveUnregisteredGeneric(ref Contract generic, ref PipelineContext context)
        {
            if (!_policies.TryGet(context.Contract.Type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (!_policies.TryGet(generic.Type, out PipelineFactory<Type>? factory))
                    return ResolveUnregistered(ref context.Contract, ref context);

                var type = context.Contract.Type;
                pipeline = factory!(ref type);
            }

            return pipeline!(ref context);
        }

        #endregion


        #region Enumerable

        private ResolveDelegate<PipelineContext> ResolveUnregisteredEnumerable(ref Type type)
        {
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                var target  = type.GenericTypeArguments[0];
                
                var types = target.IsGenericType 
                    ? new[] { target, target.GetGenericTypeDefinition() } 
                    : new[] { target };
                
                pipeline = _policies.Pipeline(type, EnumerableFactoryMethod!.CreatePipeline(target, types));
            }

            return pipeline!;
        }

        #endregion


        #region Array

        private object? ResolveUnregisteredArray(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (type.GetArrayRank() != 1)  // Verify array is valid
                    return context.Error($"Invalid array {type}. Only arrays of rank 1 are supported");

                var element = type.GetElementType()!;
                var target  = ArrayTargetType(element!) ?? element;
                var types   = target.IsGenericType 
                    ? new[] { target, target.GetGenericTypeDefinition() } 
                    : new[] { target };

                pipeline = _policies.Pipeline(context.Type, ArrayFactoryMethod!.CreatePipeline(element, types));
            }

            return pipeline!(ref context);
        }

        #endregion
    }
}
