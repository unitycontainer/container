using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.BuiltIn;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class PipelineProcessor
    {
        #region Fields

        public static readonly ResolveDelegate<PipelineContext> DefaultPipeline = (ref PipelineContext context) => context.Data;

        //public static readonly LabelTarget ReturnTarget = Expression.Label(typeof(object));

        //protected static readonly ConstructorInfo PerResolveInfo = 
        //    typeof(RuntimePerResolveLifetimeManager).GetTypeInfo()
        //                                            .DeclaredConstructors
        //                                            .First();

        //protected static readonly BinaryExpression NullEqualExisting = 
        //    Expression.Equal(Expression.Constant(null), PipelineContext.ExistingExpression);

        //protected static readonly MethodCallExpression SetPerBuildSingletonExpr =
        //    Expression.Call(PipelineContext.ContextExpression,
        //        PipelineContext.SetMethod,
        //        Expression.Constant(typeof(LifetimeManager), typeof(Type)),
        //        Expression.New(PerResolveInfo, PipelineContext.ExistingExpression));

        //protected static readonly MethodInfo StringFormat =
        //    typeof(string).GetTypeInfo()
        //        .DeclaredMethods
        //        .First(m =>
        //        {
        //            var parameters = m.GetParameters();
        //            return m.Name == nameof(string.Format) &&
        //                   m.GetParameters().Length == 2 &&
        //                   typeof(object) == parameters[1].ParameterType;
        //        });

        //protected static readonly ConstructorInfo InvalidRegistrationExpressionCtor =
        //    typeof(InvalidRegistrationException)
        //        .GetTypeInfo()
        //        .DeclaredConstructors
        //        .First(c =>
        //        {
        //            var parameters = c.GetParameters();
        //            return 1 == parameters.Length &&
        //                   typeof(string) == parameters[0].ParameterType;
        //        });

        //protected static readonly Expression CallNewGuidExpr = Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)))!;

        //protected static readonly PropertyInfo DataPropertyInfo = typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data))!;

        //protected static readonly MethodInfo AddMethodInfo = typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add))!;

        //protected static readonly ParameterExpression ExceptionExpr = Expression.Variable(typeof(Exception), "exception");

        //protected static readonly MemberExpression ExceptionDataExpr = Expression.MakeMemberAccess(ExceptionExpr, DataPropertyInfo);

        #endregion


        #region Public Members

        public virtual object? Build(ref PipelineBuilder<object?> builder) => builder.Build();

        public virtual ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder) => builder.Build();

        public virtual IEnumerable<Expression> Express(ref PipelineBuilder<IEnumerable<Expression>> builder) => builder.Express();

        #endregion
    }
}
