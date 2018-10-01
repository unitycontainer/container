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
        #region Fields

        protected static readonly MethodInfo ResolvePropertyMethod =
            typeof(IBuildContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(IBuildContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();

                    return 2 <= parameters.Length &&
                           typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        protected static readonly MethodInfo ResolveParameterMethod =
            typeof(IBuildContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(IBuildContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();

                    return 2 <= parameters.Length &&
                           typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        #endregion


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
            Existing  = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuildContext.Existing)));
        }

        #endregion


        #region Properties

        public static readonly ParameterExpression Context;

        public static readonly MemberExpression Container;

        public static readonly MemberExpression Type;

        public static readonly MemberExpression Name;

        public static readonly MemberExpression Existing;

        #endregion


        #region Methods

        public static Expression Resolve(PropertyInfo property, string name, ResolveDelegate<TContext> resolver)
        {
            return Expression.Convert(
                Expression.Call(
                    Context,
                    ResolvePropertyMethod,
                    Expression.Constant(property, typeof(PropertyInfo)),
                    Expression.Constant(name, typeof(string))), 
                property.PropertyType);
        }

        public static Expression Resolve(ParameterInfo parameter, string name, ResolveDelegate<TContext> resolver)
        {
            return Expression.Convert(
                Expression.Call(
                    Context,
                    ResolveParameterMethod,
                    Expression.Constant(parameter, typeof(ParameterInfo)),
                    Expression.Constant(name, typeof(string))),
                parameter.ParameterType);
        }

        #endregion
    }
}
