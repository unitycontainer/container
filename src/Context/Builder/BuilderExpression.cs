using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Builder
{
    public class BuilderContextExpression : IResolveContextExpression<BuilderContext>
    {
        #region Fields

        public static readonly MethodInfo ResolvePropertyMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        public static readonly MethodInfo ResolveFieldMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        public static readonly MethodInfo ResolveParameterMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        public static readonly MethodInfo SetMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Set))
                .First(m => 2 == m.GetParameters().Length);

        #endregion


        #region Constructor

        static BuilderContextExpression()
        {
            var typeInfo = typeof(BuilderContext).GetTypeInfo();


            Existing  = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(BuilderContext.Existing)));
        }

        #endregion


        #region Public Properties

        public static readonly MemberExpression Existing;

        #endregion
    }
}
