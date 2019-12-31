using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Builder
{
    // TODO: Move IResolveContextExpression<BuilderContext> here
    public class BuilderContextExpression : IResolveContextExpression<BuilderContext>
    {
        #region Fields

        public static MethodInfo ResolvePropertyMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        public static MethodInfo OverridePropertyMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Override))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });


        public static MethodInfo ResolveFieldMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        public static MethodInfo OverrideFieldMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Override))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });


        public static MethodInfo ResolveParameterMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        public static MethodInfo OverrideParameterMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Override))
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


        #region Public Methods

        public static void EnableDiagnostic()
        { 
            ResolvePropertyMethod =
                typeof(BuilderContext).GetTypeInfo()
                    .GetDeclaredMethods(nameof(BuilderContext.ResolveDiagnostic))
                    .First(m =>
                    {
                        var parameters = m.GetParameters();
                        return 0 < parameters.Length &&
                            typeof(PropertyInfo) == parameters[0].ParameterType;
                    });

            OverridePropertyMethod =
                typeof(BuilderContext).GetTypeInfo()
                    .GetDeclaredMethods(nameof(BuilderContext.OverrideDiagnostic))
                    .First(m =>
                    {
                        var parameters = m.GetParameters();
                        return 0 < parameters.Length &&
                            typeof(PropertyInfo) == parameters[0].ParameterType;
                    });


            ResolveFieldMethod =
                typeof(BuilderContext).GetTypeInfo()
                    .GetDeclaredMethods(nameof(BuilderContext.ResolveDiagnostic))
                    .First(m =>
                    {
                        var parameters = m.GetParameters();
                        return 0 < parameters.Length &&
                            typeof(FieldInfo) == parameters[0].ParameterType;
                    });

            OverrideFieldMethod =
                typeof(BuilderContext).GetTypeInfo()
                    .GetDeclaredMethods(nameof(BuilderContext.OverrideDiagnostic))
                    .First(m =>
                    {
                        var parameters = m.GetParameters();
                        return 0 < parameters.Length &&
                            typeof(FieldInfo) == parameters[0].ParameterType;
                    });


            ResolveParameterMethod =
                typeof(BuilderContext).GetTypeInfo()
                    .GetDeclaredMethods(nameof(BuilderContext.ResolveDiagnostic))
                    .First(m =>
                    {
                        var parameters = m.GetParameters();
                        return 0 < parameters.Length &&
                            typeof(ParameterInfo) == parameters[0].ParameterType;
                    });

            OverrideParameterMethod =
                typeof(BuilderContext).GetTypeInfo()
                    .GetDeclaredMethods(nameof(BuilderContext.OverrideDiagnostic))
                    .First(m =>
                    {
                        var parameters = m.GetParameters();
                        return 0 < parameters.Length &&
                            typeof(ParameterInfo) == parameters[0].ParameterType;
                    });
        }

        #endregion
    }
}
