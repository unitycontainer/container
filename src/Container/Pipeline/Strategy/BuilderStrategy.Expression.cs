using System.Collections.Generic;
using System.Linq.Expressions;

namespace Unity.Extension
{
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
        public virtual IEnumerable<Expression> ExpressPipeline<TBuilder, TContext>(ref TBuilder builder)
            where TBuilder : IExpressPipeline<TContext>
            where TContext : IBuilderContext
        {
            var pre  = IsPreBuildUp;
            var post = IsPostBuildUp;

            ///////////////////////////////////////////////////////////////////
            // No overridden methods
            if (pre && post) return builder.Express();

            ///////////////////////////////////////////////////////////////////
            // PreBuildUP is overridden
            if (!pre & post)
                return PreBuildExpression<TContext>(builder.Express());

            ///////////////////////////////////////////////////////////////////
            // PostBuildUP is overridden
            if (pre & !post)
                return PostBuildExpression<TContext>(builder.Express());

            ///////////////////////////////////////////////////////////////////
            // Both methods are overridden
            return BuildUpExpression<TContext>(builder.Express());
        }

        #endregion
    }
}
