using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;

namespace Unity.Expressions
{
    class BuilderContextExpression<TBuilderContext> : BuildContextExpression<TBuilderContext>
        where TBuilderContext : IBuilderContext
    {
        #region Constructor

        static BuilderContextExpression()
        {
            var typeInfo = typeof(TBuilderContext).GetTypeInfo();

            CurrentOperation = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.CurrentOperation)));

            TypeInfo = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.TypeInfo)));
        }

        #endregion


        #region Public Members
                                                                                                       
        public static readonly MemberExpression CurrentOperation;

        public static readonly MemberExpression TypeInfo;

        #endregion
    }
}
