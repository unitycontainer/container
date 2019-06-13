using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity
{
    public class FieldPipeline : MemberPipeline<FieldInfo, object>
    {
        #region Constructors

        public FieldPipeline(UnityContainer container)
            : base(container)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
            return type.GetDeclaredFields()
                       .Where(member => !member.IsFamily && !member.IsPrivate &&
                                        !member.IsInitOnly && !member.IsStatic);
        }

        protected override Type MemberType(FieldInfo info) => info.FieldType;

        public override MemberSelector<FieldInfo> GetOrDefault(IPolicySet? registration) => 
            registration?.Get<MemberSelector<FieldInfo>>() ?? Defaults.SelectField;

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(FieldInfo info, object? resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref PipelineContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, value));
                return context.Existing;
            };
        }

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(FieldInfo info, object? resolver)
        {
            return Expression.Assign(
                Expression.Field(Expression.Convert(PipelineContextExpression.Existing, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(PipelineContextExpression.Context,
                        PipelineContextExpression.ResolveFieldMethod,
                        Expression.Constant(info, typeof(FieldInfo)),
                        Expression.Constant(PreProcessResolver(info, resolver), typeof(object))),
                    info.FieldType));
        }

        #endregion
    }
}
