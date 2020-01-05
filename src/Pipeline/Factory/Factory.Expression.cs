using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Unity.Registration;

namespace Unity
{
    public partial class FactoryPipeline : Pipeline
    {
        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            // Skip if already have a resolver expression
            if (null != builder.SeedExpression) return builder.Express();

            var registration = builder.Registration as FactoryRegistration ??
                               builder.Factory      as FactoryRegistration;

            Debug.Assert(null != registration);

            var factory = Expression.Constant(registration!.Factory, typeof(Func<IUnityContainer, Type, string?, object?>));
            var expression = Expression.Assign(PipelineContext.ExistingExpression,
                    Expression.Invoke(factory, PipelineContext.ContainerExpression, PipelineContext.TypeExpression, PipelineContext.NameExpression));

            return builder.Express(new[] { expression });
        }
    }
}
