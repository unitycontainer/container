using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    //public class PipelineContextExpression
    public partial struct PipelineContext
    {
        #region Fields

        private static readonly TypeInfo _contextTypeInfo = 
            typeof(PipelineContext).GetTypeInfo();

        private static readonly Type _contextRefType =
            typeof(ResolveDelegate<PipelineContext>).GetTypeInfo()
                                                    .GetDeclaredMethod("Invoke")!
                                                    .GetParameters()[0]
                                                    .ParameterType;
        #endregion


        #region Method Info References

        public static readonly MethodInfo ResolveMethod =
            typeof(IResolveContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(IResolveContext.Resolve))
                .First();


        public static MethodInfo ResolvePropertyMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        public static MethodInfo OverridePropertyMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Override))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });


        public static MethodInfo ResolveFieldMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });

        public static MethodInfo OverrideFieldMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Override))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(FieldInfo) == parameters[0].ParameterType;
                });


        public static MethodInfo ResolveParameterMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        public static MethodInfo OverrideParameterMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Override))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 0 < parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });



        public static readonly MethodInfo SetMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Set))
                .First(m => 2 == m.GetParameters().Length);

        public static readonly MethodInfo GetMethod =
            typeof(PipelineContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(PipelineContext.Get))
                .First(m => 1 == m.GetParameters().Length);

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

        public static readonly MemberExpression ParentExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredField(nameof(PipelineContext.Parent)));

        public static readonly MemberExpression ExistingExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredProperty(nameof(PipelineContext.Existing)));

        public static readonly MemberExpression DeclaringTypeExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredField(nameof(PipelineContext.DeclaringType)));

        public static readonly MemberExpression LifetimeContainerExpression = 
            Expression.MakeMemberAccess(ContextExpression, 
                _contextTypeInfo.GetDeclaredProperty(nameof(PipelineContext.LifetimeContainer)));

        #endregion


        #region Public Methods

        public static void EnableDiagnostic(bool enable = true)
        {
            if (enable)
            {
                ResolvePropertyMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.ResolveDiagnostic))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(PropertyInfo) == parameters[0].ParameterType;
                        });

                OverridePropertyMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.OverrideDiagnostic))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(PropertyInfo) == parameters[0].ParameterType;
                        });


                ResolveFieldMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.ResolveDiagnostic))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(FieldInfo) == parameters[0].ParameterType;
                        });

                OverrideFieldMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.OverrideDiagnostic))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(FieldInfo) == parameters[0].ParameterType;
                        });


                ResolveParameterMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.ResolveDiagnostic))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(ParameterInfo) == parameters[0].ParameterType;
                        });

                OverrideParameterMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.OverrideDiagnostic))
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
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(PropertyInfo) == parameters[0].ParameterType;
                        });

                OverridePropertyMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.Override))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(PropertyInfo) == parameters[0].ParameterType;
                        });


                ResolveFieldMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(FieldInfo) == parameters[0].ParameterType;
                        });

                OverrideFieldMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.Override))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(FieldInfo) == parameters[0].ParameterType;
                        });


                ResolveParameterMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.Resolve))
                        .First(m =>
                        {
                            var parameters = m.GetParameters();
                            return 0 < parameters.Length &&
                                typeof(ParameterInfo) == parameters[0].ParameterType;
                        });

                OverrideParameterMethod =
                    typeof(PipelineContext).GetTypeInfo()
                        .GetDeclaredMethods(nameof(PipelineContext.Override))
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
