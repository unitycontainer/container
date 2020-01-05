using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        #region Fields
        
        private static TypeInfo _contextTypeInfo = 
            typeof(BuilderContext).GetTypeInfo();

        private static Type _contextRefType =
            typeof(ResolveDelegate<BuilderContext>).GetTypeInfo()
                                             .GetDeclaredMethod("Invoke")!
                                             .GetParameters()[0]
                                             .ParameterType;
        #endregion


        #region Public Fields
        
        public static readonly MethodInfo ResolveMethod =
            typeof(IResolveContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(IResolveContext.Resolve))
                .First();


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


        #region Public Properties

        public static readonly ParameterExpression ContextExpression = 
            Expression.Parameter(_contextRefType, "context");

        public static readonly MemberExpression TypeExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredProperty(nameof(IResolveContext.Type)));

        public static readonly MemberExpression NameExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredProperty(nameof(IResolveContext.Name)));

        public static readonly MemberExpression ContainerExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredProperty(nameof(IResolveContext.Container)));

        public static readonly MemberExpression ExistingExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredProperty(nameof(BuilderContext.Existing)));

        #endregion


        #region Public Methods

        public static void EnableDiagnostic(bool enable = true)
        {
            if (enable)
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
            else
            {
                ResolvePropertyMethod =
                    typeof(BuilderContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(PropertyInfo) == parameters[0].ParameterType;
                        });

                OverridePropertyMethod =
                    typeof(BuilderContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(BuilderContext.Override))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(PropertyInfo) == parameters[0].ParameterType;
                        });


                ResolveFieldMethod =
                    typeof(BuilderContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(FieldInfo) == parameters[0].ParameterType;
                        });

                OverrideFieldMethod =
                    typeof(BuilderContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(BuilderContext.Override))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(FieldInfo) == parameters[0].ParameterType;
                        });


                ResolveParameterMethod =
                    typeof(BuilderContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(ParameterInfo) == parameters[0].ParameterType;
                        });

                OverrideParameterMethod =
                    typeof(BuilderContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(BuilderContext.Override))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(ParameterInfo) == parameters[0].ParameterType;
                        });
            }
        }

        #endregion
    }
}
