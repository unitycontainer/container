using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Strategies;

namespace Unity.Processors
{
    public class MemberBuildProcessor : BuilderStrategy
    {
        public virtual IEnumerable<Expression> GetEnumerator(ref BuilderContext context) 
            => Enumerable.Empty<Expression>();
    }

    public partial class MemberBuildProcessor<TMemberInfo, TData> : MemberBuildProcessor
                                                where TMemberInfo : MemberInfo
    {
        #region Static Fields

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

        #endregion
    }
}
