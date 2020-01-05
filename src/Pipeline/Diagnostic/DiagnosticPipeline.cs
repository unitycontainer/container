using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    [SecuritySafeCritical]
    public class DiagnosticPipeline : Pipeline
    {
        #region Fields

        protected static readonly MethodInfo ValidateMethod =
            typeof(DiagnosticPipeline).GetTypeInfo()
                .GetDeclaredMethod(nameof(DiagnosticPipeline.ValidateCompositionStack))!;

        protected static readonly ConstructorInfo TupleConstructor =
            typeof(Tuple<Type, string?>).GetTypeInfo().DeclaredConstructors.First();

        #endregion


        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            var pipeline = builder.Pipeline() ?? ((ref PipelineContext c) => throw new InvalidRegistrationException("Invalid Pipeline"));

            return (ref PipelineContext context) =>
            {
#if !NET40
                // Check call stack for cyclic references
                ValidateCompositionStack(context.Parent, context.Type, context.Name);
#endif
                try
                {
                    // Execute pipeline
                    return pipeline(ref context);
                }
                catch (Exception ex) when (ex is InvalidRegistrationException || ex is CircularDependencyException)
                {
                    ex.Data.Add(Guid.NewGuid(), null == context.Name
                        ? (object)context.Type
                        : new Tuple<Type, string?>(context.Type, context.Name));

                    throw;
                }
            };
        }

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            var expressions = builder.Express();

            var infoExpr = Expression.Condition(
                Expression.Equal(Expression.Constant(null), PipelineContext.NameExpression),
                Expression.Convert(PipelineContext.TypeExpression, typeof(object)),
                Expression.Convert(Expression.New(TupleConstructor, PipelineContext.TypeExpression, PipelineContext.NameExpression), typeof(object)));

            var filter = Expression.OrElse(
                Expression.TypeIs(ExceptionExpr, typeof(InvalidRegistrationException)), 
                Expression.TypeIs(ExceptionExpr, typeof(CircularDependencyException)));

            var tryBody = Expression.Block(expressions);
            var catchBody = Expression.Block(tryBody.Type,
                Expression.IfThen(filter, Expression.Call(ExceptionDataExpr, AddMethodInfo, Expression.Convert(CallNewGuidExpr, typeof(object)), infoExpr)),
                Expression.Rethrow(tryBody.Type));

            return new Expression[]
            {
                Expression.Call(ValidateMethod, PipelineContext.ParentExpression, PipelineContext.TypeExpression, PipelineContext.NameExpression),
                Expression.TryCatch(tryBody,  Expression.Catch(ExceptionExpr, catchBody))
            };
        }


        #region Validation

#if !NET40
        [SecuritySafeCritical]
        private static void ValidateCompositionStack(IntPtr parent, Type type, string? name)
        {
            if (IntPtr.Zero == parent) return;

            unsafe
            {
                var parentRef = Unsafe.AsRef<PipelineContext>(parent.ToPointer());
                if (type == parentRef.Type && name == parentRef.Name)
                    throw new CircularDependencyException(parentRef.Type, parentRef.Name);

                ValidateCompositionStack(parentRef.Parent, type, name);
            }
        }
#endif

        #endregion
    }
}
