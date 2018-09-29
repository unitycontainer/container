using System.Linq.Expressions;
using System.Reflection;
using Unity.Build;
using Unity.Delegates;

namespace Unity.Expressions
{
    public class BuildContextExpression<TContext>
        where TContext : IBuildContext
    {
        static BuildContextExpression()
        {
            var typeInfo = typeof(TContext).GetTypeInfo();

            var contextRefType =
                typeof(ResolveDelegate<TContext>).GetTypeInfo()
                    .GetDeclaredMethod("Invoke")
                    .GetParameters()[0]
                    .ParameterType;

            Context   = Expression.Parameter(contextRefType, "context");
            Container = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuildContext.Container)));
            Type      = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuildContext.Type)));
            Name      = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuildContext.Name)));
            Existing  = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuildContext.Existing)));
        }


        #region Public Members

        public static readonly ParameterExpression Context;

        public static readonly MemberExpression Container;

        public static readonly MemberExpression Type;

        public static readonly MemberExpression Name;

        public static readonly MemberExpression Existing;

        #endregion

    }
}
