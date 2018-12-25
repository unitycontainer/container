using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Policy;

namespace Unity.Resolution
{
    public class IResolveContextExpression<TContext>
        where TContext : IResolveContext
    {
        #region Fields

        private static readonly MethodInfo _get =
            typeof(IPolicyList).GetTypeInfo()
                .GetDeclaredMethods(nameof(IPolicyList.Get))
                .First();

        private static readonly MethodInfo _set =
            typeof(IPolicyList).GetTypeInfo()
                .GetDeclaredMethods(nameof(IPolicyList.Set))
                .First();

        private static readonly MethodInfo _resolve =
            typeof(IResolveContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(IResolveContext.Resolve))
                .First(m => 2 == m.GetParameters().Length);

        #endregion


        #region Constructor

        static IResolveContextExpression()
        {
            var typeInfo = typeof(TContext).GetTypeInfo();

            var contextRefType =
                typeof(ResolveDelegate<TContext>).GetTypeInfo()
                                                 .GetDeclaredMethod("Invoke")
                                                 .GetParameters()[0]
                                                 .ParameterType;

            Context = Expression.Parameter(contextRefType, "context");
            Type = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IResolveContext.Type)));
            Name = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IResolveContext.Name)));
            Container = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IResolveContext.Container)));
        }

        #endregion


        #region Properties

        public static readonly ParameterExpression Context;

        public static readonly MemberExpression Container;

        public static readonly MemberExpression Type;

        public static readonly MemberExpression Name;

        #endregion


        #region Methods

        public static Expression Get(Type type, string name, Type policyType)
        {
            return Expression.Call(Context, _get,
                    Expression.Constant(type, typeof(Type)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(policyType, typeof(Type)));
        }

        public static Expression Set(Type type, string name, Type policyType, Expression policy)
        {
            return Expression.Call(Context, _set,
                    Expression.Constant(type, typeof(Type)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(policyType, typeof(Type)),
                    policy);
        }

        public static Expression Resolve(Type type, string name)
        {
            return Expression.Convert(
                Expression.Call(Context, _resolve,
                    Expression.Constant(type, typeof(Type)),
                    Expression.Constant(name, typeof(string))),
                type);
        }

        #endregion
    }
}
