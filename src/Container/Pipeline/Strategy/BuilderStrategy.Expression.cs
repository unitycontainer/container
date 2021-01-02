namespace Unity.Extension
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    public abstract partial class BuilderStrategy
    {
        #region Expression

        /// <summary>
        /// Builds and compiles pipeline
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TBuilder"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual ResolveDelegate<TContext>? Express<TContext, TBuilder>(ref TBuilder builder)
            where TBuilder : IExpressPipeline<TContext>
            where TContext : IBuilderContext
            => null;

        #endregion
    }
}
