using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public abstract class Pipeline
    {
        #region Fields

        public static readonly LabelTarget ReturnTarget = Expression.Label(typeof(object));

        protected static readonly ConstructorInfo PerResolveInfo = 
            typeof(RuntimePerResolveLifetimeManager).GetTypeInfo()
                                                    .DeclaredConstructors
                                                    .First();

        protected static readonly BinaryExpression NullEqualExisting = 
            Expression.Equal(Expression.Constant(null), PipelineContextExpression.Existing);

        protected static readonly MethodCallExpression SetPerBuildSingletonExpr =
            Expression.Call(PipelineContextExpression.Context,
                PipelineContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, PipelineContextExpression.Existing));

        #endregion


        #region Public Members

        public virtual ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder) => builder.Pipeline();

        public virtual IEnumerable<Expression> Express(ref PipelineBuilder builder) => builder.Express();

        #endregion
    }
}
