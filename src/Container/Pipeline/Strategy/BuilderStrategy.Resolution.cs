namespace Unity.Extension
{
    public abstract partial class BuilderStrategy
    {
        /// <summary>
        /// Builds resolution pipeline
        /// </summary>
        /// <remarks>
        /// <para>
        /// This methods, when overridden, allows to build a resolving (delegate based) pipeline. 
        /// By default, when not overridden, it simply calls <see cref="PreBuildUp"/> and/or <see cref="PostBuildUp"/>
        /// of the implemented strategy.
        /// </para>
        /// <para>
        /// This is, sort of, a compromise solution when developer only implements PreBuildUp and PostBuildUp methods. This
        /// method will wrap them into delegate and would still allow some performance optimization.
        /// </para>
        /// </remarks>
        /// <typeparam name="TContext"><see cref="Type"/> implementing <see cref="IBuilderContext"/></typeparam>
        /// <typeparam name="TBuilder"><see cref="Type"/> implementing <see cref="IBuildPipeline<TContext>"/></typeparam>
        /// <param name="builder">The pipeline builder</param>
        /// <returns>Returns built pipeline</returns>
        public virtual ResolveDelegate<TContext>? Build<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
            where TBuilder : IBuildPipeline<TContext>
            where TContext : IBuilderContext
        {
            var pre  = IsPreBuildUp;
            var post = IsPostBuildUp;
            
            // Closures
            var pipeline = builder.Build(ref context);

            ///////////////////////////////////////////////////////////////////
            // No overridden methods
            if (pre && post) return pipeline;


            ///////////////////////////////////////////////////////////////////
            // PreBuildUP is overridden
            if (!pre & post)
            {
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

            ///////////////////////////////////////////////////////////////////
            // PostBuildUP is overridden
            if (pre & !post)
            {
                return (ref TContext context) =>
                {
                    // Run downstream pipeline
                    if (pipeline is not null) pipeline(ref context);

                    // Abort on error
                    if (!context.IsFaulted) PostBuildUp(ref context);

                    return context.Existing;
                };
            }

            ///////////////////////////////////////////////////////////////////
            // Both methods are overridden
            return (ref TContext context) => 
            {
                // PreBuildUP
                PreBuildUp(ref context);
                
                // Abort on error
                if (context.IsFaulted) return context.Existing;

                // Run downstream pipeline
                if (pipeline is not null) pipeline(ref context);

                // Abort on error
                if (context.IsFaulted) return context.Existing;

                // PostBuildUP
                PostBuildUp(ref context);

                return context.Existing;
            };
        }
    }
}
