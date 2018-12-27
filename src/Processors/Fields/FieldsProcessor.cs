using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;

namespace Unity.Processors
{
    public class FieldsProcessor : MemberBuildProcessor<FieldInfo, object>
    {
        #region Fields

        private static readonly MethodInfo ResolveField =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();

        protected override FieldInfo[] DeclaredMembers(Type type)
        {
            return base.DeclaredMembers(type);
        }

        protected override Type MemberType(FieldInfo info) 
            => info.FieldType;

        protected override Expression ResolveExpression(FieldInfo field, string name, object resolver)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveField,
                    Expression.Constant(field, typeof(FieldInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                field.FieldType);
        }

        protected override MemberExpression CreateMemberExpression(FieldInfo info) 
            => Expression.Field(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info);

        #endregion

    }
}
