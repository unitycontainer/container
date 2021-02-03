using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial class StagedStrategyChain
    {
        public ResolveDelegate<TContext> BuildUpPipeline<TContext>()
            where TContext : IBuilderContext
        {
            var processors = Values.ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Existing;
            };
        }


        public ResolveDelegate<TContext> BuildUpCompiled<TContext>()
            where TContext : IBuilderContext
        {
            var exec = Expression.Block(BuildUp<TContext>(0));
            var body = Expression.Block(exec, Context<TContext>.Label, Context<TContext>.ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(body, Context<TContext>.ContextExpression);

            return lambda.Compile();
        }


        private IEnumerable<Expression> BuildUp<TContext>(int level)
            where TContext : IBuilderContext
        {
            if (_size <= level) return EmptyExpression;

            ref var entry = ref _stages[level];

            if (entry.Strategy is null) return BuildUp<TContext>(level + 1);

            var strategy = entry.Strategy;

            if (entry.PreBuildUp is not null && entry.PostBuildUp is not null)
            { 
                return entry.PreBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PreBuildUp, strategy)
                    .Concat(BuildUp<TContext>(level + 1))
                    .Concat(entry.PostBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PostBuildUp, strategy));
            }

            if (entry.PreBuildUp is not null)
            { 
                return entry.PreBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PreBuildUp, strategy)
                    .Concat(BuildUp<TContext>(level + 1));
            }

            if (entry.PostBuildUp is not null)
            { 
                return BuildUp<TContext>(level + 1)
                    .Concat(entry.PostBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PostBuildUp!, strategy));
            }

            return BuildUp<TContext>(level + 1);
        }


        private IEnumerable<Expression> ExpressBuildUp<TContext>(MethodInfo method, BuilderStrategy strategy)
            where TContext : IBuilderContext
        {
            return new Expression[] 
            {
                Expression.Call(Expression.Constant(strategy),
                    method.MakeGenericMethod(typeof(TContext)),
                    Context<TContext>.ContextExpression),

                Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), Context<TContext>.IsFaultedExpression),
                    Expression.Return(Context<TContext>.ExitLabel))
            };
        }
    }
}
