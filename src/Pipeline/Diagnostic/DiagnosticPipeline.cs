using System;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    [SecuritySafeCritical]
    public class DiagnosticPipeline : Pipeline
    {
        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new InvalidRegistrationException("Invalid Pipeline"));

            return (ref BuilderContext context) =>
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
        private void ValidateCompositionStack(IntPtr parent, Type type, string? name)
        {
            if (IntPtr.Zero == parent) return;

            unsafe
            {
                var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                if (type == parentRef.Type && name == parentRef.Name)
                    throw new CircularDependencyException(parentRef.Type, parentRef.Name);

                ValidateCompositionStack(parentRef.Parent, type, name);
            }
        }
#endif

        #endregion
    }
}
