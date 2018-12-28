using System.Linq.Expressions;
using System.Reflection;
using Unity.Policy;

namespace Unity.Resolution
{
    public class IResolveContextExpression<TContext>
        where TContext : IResolveContext
    {
        #region Constructor

        static IResolveContextExpression()
        {
            var typeInfo = typeof(TContext).GetTypeInfo();

            var contextRefType =
                typeof(ResolveDelegate<TContext>).GetTypeInfo()
                                                 .GetDeclaredMethod("Invoke")
                                                 .GetParameters()[0]
                                                 .ParameterType;

            Context   = Expression.Parameter(contextRefType, "context");
            Type      = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IResolveContext.Type)));
            Name      = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IResolveContext.Name)));
            Container = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IResolveContext.Container)));
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
