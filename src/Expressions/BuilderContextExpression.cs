using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Policy;

namespace Unity.Expressions
{
    class BuilderContextExpression<TBuilderContext> : BuildContextExpression<TBuilderContext>
        where TBuilderContext : IBuilderContext
    {
        #region Constructor

        static BuilderContextExpression()
        {
            var typeInfo = typeof(TBuilderContext).GetTypeInfo();

            CurrentOperation = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.CurrentOperation)));

            TypeInfo = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.TypeInfo)));
        }

        #endregion


        #region Public Properties
                                                                                                       
        public static readonly MemberExpression CurrentOperation;

        public static readonly MemberExpression TypeInfo;

        #endregion



        #region Methods

        public static Expression Resolve(SelectedProperty property, string name)
        {
            throw new NotImplementedException();
            //return Expression.Convert(
            //    Expression.Call(
            //        Context,
            //        ResolvePropertyMethod,
            //        Expression.Constant(property, typeof(PropertyInfo)),
            //        Expression.Constant(name, typeof(string))),
            //    property.PropertyType);
        }

        public static Expression Resolve(IResolverPolicy resolver, ParameterInfo parameter, string name)
        {
            throw new NotImplementedException();
            //return Expression.Convert(
            //    Expression.Call(
            //        Context,
            //        ResolveParameterMethod,
            //        Expression.Constant(parameter, typeof(ParameterInfo)),
            //        Expression.Constant(name, typeof(string))),
            //    parameter.ParameterType);
        }

        #endregion
    }
}
