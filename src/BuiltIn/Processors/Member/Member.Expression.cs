using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        #region Fields

        protected static readonly MethodCallExpression NewGuidExpression =
            Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)))!;

        protected static readonly PropertyInfo DataPropertyExpression =
            typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data))!;

        protected static readonly ParameterExpression ExceptionVariableExpression =
            Expression.Variable(typeof(Exception));

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder<IEnumerable<Expression>> builder)
        {
            if (null != builder.Target) return builder.Express();

            var expressions = builder.Express();

            throw new NotImplementedException();
            // TODO:             var members = (IReadOnlyCollection<object>)Select(ref builder);

            //return expressions.Concat(ExpressionsFromSelection(builder.Type, members));
        }

        #endregion


        #region Selection Processing

        protected virtual IEnumerable<Expression> ExpressionsFromSelection(Type type, IEnumerable<object> members)
        {
            throw new NotImplementedException();
            //
            //foreach (var member in members)
            //{
            //    switch (member)
            //    {
            //        // TMemberInfo
            //        case TMemberInfo info:
            //            yield return GetResolverExpression(info);
            //            break;

            //        // Injection Member
            //        case InjectionMember<TMemberInfo, TData> injectionMember:
            //            yield return GetResolverExpression(injectionMember.MemberInfo(type),
            //                                               injectionMember.Data);
            //            break;
                    
            //        case Exception exception:
            //            yield return Expression.Throw(Expression.Constant(exception));
            //            yield break;

            //        // Unknown
            //        default:
            //            throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
            //    }
            //}
        }

        #endregion


        #region Implementation


        protected virtual Expression GetResolverExpression(TMemberInfo info)
        {
            throw new NotImplementedException();
        }

        protected abstract Expression GetResolverExpression(TMemberInfo info, object? data);

        #endregion
    }
}
