using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity.Extension
{
    public abstract partial class BuilderStrategy
    {
        /// <summary>
        /// Builds and compiles pipeline
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TBuilder"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual IEnumerable<Expression> ExpressPipeline<TBuilder, TContext>(ref TBuilder builder, object? analytics)
            where TBuilder : IExpressPipeline<TContext>
            where TContext : IBuilderContext
        {
            ///////////////////////////////////////////////////////////////////
            // No overridden methods
            if (IsPreBuildUp && IsPostBuildUp) 
                return builder.Express();

            ///////////////////////////////////////////////////////////////////
            // PreBuildUP is overridden
            if (!IsPreBuildUp & IsPostBuildUp)
                return (_preBuildExpr ??= GetPreBuildExpr<TBuilder, TContext>(ref builder))
                    .Concat(builder.Express());

            ///////////////////////////////////////////////////////////////////
            // PostBuildUP is overridden
            if (IsPreBuildUp & !IsPostBuildUp)
                return builder.Express()
                    .Concat(_postBuildExpr ??= GetPostBuildExpr<TBuilder, TContext>(ref builder));

            ///////////////////////////////////////////////////////////////////
            // Both methods are overridden
            return (_preBuildExpr ??= GetPreBuildExpr<TBuilder, TContext>(ref builder))
                .Concat(builder.Express())
                .Concat(_postBuildExpr ??= GetPostBuildExpr<TBuilder, TContext>(ref builder));
        }
    }
}
