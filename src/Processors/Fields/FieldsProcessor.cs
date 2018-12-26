using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;

namespace Unity.Processors
{
    public class FieldsProcessor : MemberBuildProcessor<FieldInfo, object>
    {
        #region Overrides

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();

        #endregion


        #region Implementation

        protected override Type MemberType(FieldInfo info) 
            => info.FieldType;

        protected override Expression ResolveExpression(FieldInfo info, string name, object resolver)
            => BuilderContextExpression.Resolve(info, name, resolver);

        protected override MemberExpression CreateMemberExpression(ParameterExpression variable, FieldInfo info) 
            => Expression.Field(variable, info);

        #endregion

    }
}
