using Unity.Extension;

namespace Unity.Container
{
    public partial class MethodStrategy
    {
        //public override ResolveDelegate<TContext>? Build<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
        //{
        //    // Closures
        //    var pipeline = builder.Build(ref context);

        //    return (ref TContext context) =>
        //    {
        //        // PreBuildUP
        //        PreBuildUp(ref context);

        //        // Run downstream pipeline
        //        if (!context.IsFaulted &&
        //            pipeline is not null) pipeline(ref context);

        //        return context.Existing;
        //    };
        //}
    }
}
