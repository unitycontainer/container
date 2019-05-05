using System;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class DiagnosticBuilder : PipelineBuilder
    {
        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => c.Existing);

            return (ref BuilderContext context) =>
            {
#if !NET40
                var value = GetPerResolveValue(context.Parent, context.Type, context.Name);
                if (LifetimeManager.NoValue != value) return value;
#endif
                try
                {
                    return pipeline(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string>(context.Type, context.Name));

                    throw;
                }
            };
        }

#if !NET40
        [SecuritySafeCritical]
        object GetPerResolveValue(IntPtr parent, Type type, string? name)
        {
            if (IntPtr.Zero == parent) return LifetimeManager.NoValue;

            unsafe
            {
                var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                if (type != parentRef.Type || name != parentRef.Name)
                    return GetPerResolveValue(parentRef.Parent, type, name);

                var lifetimeManager = (LifetimeManager?)parentRef.Get(typeof(LifetimeManager));
                var result = null == lifetimeManager ? LifetimeManager.NoValue : lifetimeManager.GetValue();
                if (LifetimeManager.NoValue != result) return result;

                throw new InvalidOperationException($"Circular reference for Type: {parentRef.Type}, Name: {parentRef.Name}",
                        new CircularDependencyException());
            }
        }
#endif
    }
}
