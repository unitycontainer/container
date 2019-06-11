using System;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    [SecuritySafeCritical]
    public class DiagnosticPipeline : Pipeline
    {
        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            var pipeline = builder.Pipeline() ?? ((ref PipelineContext c) => throw new InvalidRegistrationException("Invalid Pipeline"));

            return (ref PipelineContext context) =>
            {
#if !NET40
                // Check call stack for cyclic references
                ValidateCompositionStack(context.Parent, context.Type, context.Name);
#endif
                try
                {
                    // Execute pipeline
                    return pipeline(ref context);
                }
                catch (Exception ex) when (ex is InvalidRegistrationException || ex is CircularDependencyException)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string?>(context.Type, context.Name));

                    throw;
                }
            };
        }


        #region Validation

#if !NET40
        [SecuritySafeCritical]
        private static void ValidateCompositionStack(IntPtr parent, Type type, string? name)
        {
            if (IntPtr.Zero == parent) return;

            unsafe
            {
                var parentRef = Unsafe.AsRef<PipelineContext>(parent.ToPointer());
                if (type == parentRef.Type && name == parentRef.Name)
                    throw new CircularDependencyException(parentRef.Type, parentRef.Name);

                ValidateCompositionStack(parentRef.Parent, type, name);
            }
        }
#endif

        #endregion
    }
}
