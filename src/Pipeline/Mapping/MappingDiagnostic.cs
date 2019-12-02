using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity
{
    public class MappingDiagnostic : MappingPipeline
    {

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            var requestedType = builder.Type;
            var pipeline = base.Build(ref builder);
            var type = builder.Type;

            if (requestedType == type) return pipeline;

            Debug.Assert(null != pipeline);

            return (ref PipelineContext context) => 
            {
                try
                {
                    return pipeline!(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), new Tuple<Type, Type>(requestedType, type));
                    throw;
                }
            };
        }

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            var requestedType = builder.Type;
            var expressions = base.Express(ref builder);
            var type = builder.Type;

            if (requestedType == type) return expressions;

            var tryBlock = Expression.Block(expressions);
            var catchBlock = Expression.Block(tryBlock.Type,
                    Expression.Call(ExceptionDataExpr, AddMethodInfo,
                        Expression.Convert(CallNewGuidExpr, typeof(object)),
                        Expression.Constant(new Tuple<Type, Type>(requestedType, type), typeof(object))),
                Expression.Rethrow(tryBlock.Type));

            return new[] { Expression.TryCatch(tryBlock, Expression.Catch(ExceptionExpr, catchBlock))};
        }
    }
}
