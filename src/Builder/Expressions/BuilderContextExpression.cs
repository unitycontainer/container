using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;
using Unity.ResolverPolicy;

namespace Unity.Builder.Expressions
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    class BuilderContextExpression : IResolveContextExpression<BuilderContext>
    {
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


        #region Methods

        public static IEnumerable<Expression> GetParameters(MethodBase methodBase)
        {
            foreach (var parameter in methodBase.GetParameters())
            {
                // Resolve all DependencyAttributes on this parameter, if any
                var attribute = parameter.GetCustomAttributes(false)
                    .OfType<DependencyResolutionAttribute>()
                    .FirstOrDefault();

                if (null == attribute)
                    yield return Resolve(parameter, null, null);
                else
                    yield return attribute is OptionalDependencyAttribute
                        ? Resolve(parameter, attribute.Name, new OptionalDependencyResolvePolicy(parameter.ParameterType, attribute.Name))
                        : Resolve(parameter, attribute.Name, null);
            }
        }

        public static IEnumerable<Expression> GetParameters(ConstructorInfo info, object[] values)
        {
            var parameters = info.GetParameters();
            var resolvers = null != values && 0 == values.Length ? null : values;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                // Resolve all DependencyAttributes on this parameter, if any
                var attribute = parameter.GetCustomAttributes(false)
                    .OfType<DependencyResolutionAttribute>()
                    .FirstOrDefault();

                yield return Resolve(parameter, attribute?.Name, resolvers?[i]);
            }
        }

        #endregion
    }
}
