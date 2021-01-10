using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class ConstructorStrategy
    {
        public override ResolveDelegate<TContext>? Build<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
        {
            // Closures
            var pipeline = builder.Build(ref context);

            return (ref TContext context) =>
            {
                // PreBuildUP
                PreBuildUp(ref context);

                // Run downstream pipeline
                if (!context.IsFaulted &&
                    pipeline is not null) pipeline(ref context);

                return context.Existing;
            };
        }
    }
}
