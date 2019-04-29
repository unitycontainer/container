using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract class PipelineProcessor
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

        protected static readonly Expression InvalidRegistrationExpression = Expression.New(typeof(InvalidRegistrationException));

        protected static readonly Expression NewGuid = Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)));

        protected static readonly PropertyInfo DataProperty = typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data));

        protected static readonly MethodInfo AddMethod = typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add));

        #endregion


        #region Public Methods

        public abstract IEnumerable<Expression> GetExpressions(Type type, ImplicitRegistration registration);

        public abstract ResolveDelegate<BuilderContext>? GetResolver(Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed);

        #endregion
    }
}
