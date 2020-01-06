using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity
{
    public abstract partial class MethodBasePipeline<TMemberInfo> : MemberPipeline<TMemberInfo, object[]>
                                        where TMemberInfo : MethodBase
    {
        #region Fields

        protected static readonly UnaryExpression ReThrowExpression =
            Expression.Rethrow(typeof(void));

        protected static readonly Expression GuidToObjectExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        protected static readonly MethodInfo AddMethodExpression =
            typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add))!;

        protected static readonly UnaryExpression ConvertExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        protected static readonly MemberExpression ExceptionDataExpression =
            Expression.MakeMemberAccess(ExceptionVariableExpression, DataPropertyExpression);

        private const string _error = "Invalid 'ref' or 'out' parameter '{0}' ({1})";

        #endregion
    }
}
