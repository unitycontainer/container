using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Build;
using Unity.Builder;
using Unity.Delegates;

namespace Unity.Expressions
{
    public class BuildContextExpression<TContext>
        where TContext : IBuildContext
    {
        #region Constructor

        static BuildContextExpression()
        {
            var typeInfo = typeof(TContext).GetTypeInfo();

            var contextRefType =
                typeof(ResolveDelegate<TContext>).GetTypeInfo()
                                                 .GetDeclaredMethod("Invoke")
                                                 .GetParameters()[0]
                                                 .ParameterType;

            Context   = Expression.Parameter(contextRefType, "context");
            Type      = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(INamedType.Type)));
            Name      = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(INamedType.Name)));
            Container = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuildContext.Container)));
        }

        #endregion


        #region Properties

        public static readonly ParameterExpression Context;

        public static readonly MemberExpression Container;

        public static readonly MemberExpression Type;

        public static readonly MemberExpression Name;

        #endregion
    }
}
