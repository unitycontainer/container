using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        /// <summary>
        /// Builds up an object without accessible constructor
        /// </summary>
        /// <remarks>Used only for <see cref="IUnityContainer.BuildUp"/></remarks>
        protected virtual object? NoConstructors(ref PipelineBuilder<object?> builder)
        {
            throw new NotImplementedException();
            //// Throw if no object
            //if (null == builder.Context.Element) 
            //    return builder.FromError(NoConstructorError);
                
            //// Build up existing object
            //return builder.Build();
        }

        /// <summary>
        /// Creates pipeline for an object without accessible constructor or an interface
        /// </summary>
        /// <remarks>Used only for <see cref="IUnityContainer.BuildUp"/></remarks>
        protected virtual Pipeline? NoConstructors(ref PipelineBuilder<Pipeline?> builder)
        {
            throw new NotImplementedException();
            //var pipeline = builder.Build();

            //return (ref ResolutionContext context) => 
            //{
            //    // Throw if no object
            //    if (null == context.Element)
            //    { 
            //        context.FromError(NoConstructorError);
            //        return;
            //    }

            //    // Build up existing object
            //    pipeline?.Invoke(ref context);
            //};
        }
    }
}
