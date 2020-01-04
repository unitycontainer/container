using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData> 
    {
        #region Fields

        protected static readonly MethodInfo StringFormat =
            typeof(string).GetTypeInfo()
                .DeclaredMethods
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return m.Name == nameof(string.Format) &&
                           m.GetParameters().Length == 2 &&
                           typeof(object) == parameters[1].ParameterType;
                });

        protected static readonly NewExpression InvalidRegistrationExpression =
            Expression.New(typeof(InvalidRegistrationException));

        protected static readonly MethodCallExpression NewGuidExpression =
            Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)))!;

        protected static readonly PropertyInfo DataPropertyExpression =
            typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data))!;
        protected static readonly ParameterExpression ExceptionVariableExpression =
            Expression.Variable(typeof(Exception));

        #endregion
    }
}
