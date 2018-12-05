using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Build;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.ResolverPolicy;

namespace Unity.Expressions
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    class BuilderContextExpression<TBuilderContext> : BuildContextExpression<TBuilderContext>
        where TBuilderContext : IBuilderContext
    {
        #region Constructor

        static BuilderContextExpression()
        {
            var typeInfo = typeof(TBuilderContext).GetTypeInfo();

            CurrentOperation = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.CurrentOperation)));

            TypeInfo = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.TypeInfo)));
            Existing  = Expression.MakeMemberAccess(Context, typeInfo.GetDeclaredProperty(nameof(IBuilderContext.Existing)));
        }

        #endregion


        #region Public Properties

        public static readonly MemberExpression Existing;

        public static readonly MemberExpression CurrentOperation;

        public static readonly MemberExpression TypeInfo;

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
                        ? Resolve(parameter, attribute.Name, new OptionalDependencyResolverPolicy(parameter.ParameterType, attribute.Name))
                        : Resolve(parameter, attribute.Name, null);
            }
        }

        public static IEnumerable<Expression> GetParameters<TInfo>(SelectedMemberWithParameters<TInfo> member)
            where TInfo : MethodBase
        {
            var parameters = member.MemberInfo.GetParameters();
            var resolvers = member.GetParameterResolvers();

            if (parameters.Length == resolvers.Length)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    // Resolve all DependencyAttributes on this parameter, if any
                    var attribute = parameter.GetCustomAttributes(false)
                        .OfType<DependencyResolutionAttribute>()
                        .FirstOrDefault();

                    yield return Resolve(parameter, attribute?.Name, resolvers[i]);
                }
            }
            else
            {
                foreach (var parameter in parameters)
                {
                    // Resolve all DependencyAttributes on this parameter, if any
                    var attribute = parameter.GetCustomAttributes(false)
                        .OfType<DependencyResolutionAttribute>()
                        .FirstOrDefault();

                    yield return Resolve(parameter, attribute?.Name, null);
                }
            }

        }

        #endregion
    }
}
