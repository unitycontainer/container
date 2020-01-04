using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;

namespace Unity
{
    public abstract partial class MethodBasePipeline<TMemberInfo> : MemberPipeline<TMemberInfo, object[]>
                                        where TMemberInfo : MethodBase
    {
        #region Fields

        protected static readonly UnaryExpression ReThrowExpression =
            Expression.Rethrow(typeof(void));

        protected static readonly Expression GuidToObjectExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        protected static readonly MethodInfo AddMethodExpression =
            typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add))!;

        protected static readonly UnaryExpression ConvertExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        protected static readonly MemberExpression ExceptionDataExpression =
            Expression.MakeMemberAccess(ExceptionVariableExpression, DataPropertyExpression);

        protected readonly UnityContainer Container;
        private const string _error = "Invalid 'ref' or 'out' parameter '{0}' ({1})";

        #endregion


        #region Constructors

        protected MethodBasePipeline(Type attribute, UnityContainer container)
            : base(container)
        {
            Container = container;
        }

        #endregion


        #region Implementation

        protected ParameterExpression[] VariableExpressions(ParameterInfo[] parameters)
        {
            return parameters.Select(ToVariable)
                             .ToArray();

            ParameterExpression ToVariable(ParameterInfo info)
            {
                if (info.ParameterType.IsByRef)
                    throw  new InvalidRegistrationException(string.Format(_error, info.Name, info.ParameterType), info);

                return Expression.Variable(info.ParameterType, info.Name);
            }
        }

        protected bool CanResolve(ParameterInfo info)
        {
                
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute;
            
            if (null != attribute) return Container.CanResolve(info.ParameterType, attribute.Name);

            return Container.CanResolve(info.ParameterType, null);
        }

        #endregion
    }
}
